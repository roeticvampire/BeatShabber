using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    static bool mouseInput;


    public static bool shouldRender;
    public Renderer rend;
    public float speed=10;
    SimpleBlobDetector simpleBlobDetector;
    WebCamTexture _webCamTexture;
    KeyPoint currKeypoint;
    Mat imgHSV;
    Mat frame;
    KeyPoint kp;
    // Start is called before the first frame update
    void Start()
    {
        mouseInput = true;
        shouldRender = false;
        var detectorParams = new SimpleBlobDetector.Params
        {
            MinDistBetweenBlobs = 50, // 10 pixels between blobs
            //MinRepeatability = 1,

            MinThreshold = 100,
            MaxThreshold = 255,
            ThresholdStep = 5,

            //FilterByArea = false,
            FilterByArea = true,
            MinArea = 0.1f, // 100 shayad pixels squared
            MaxArea = 500f,

            FilterByCircularity = false,
            //FilterByCircularity = true,
            //MinCircularity = 0.001f,

            FilterByConvexity = false,
            //FilterByConvexity = true,
            //MinConvexity = 0.001f,
            //MaxConvexity = 10,

            FilterByInertia = false,
            //FilterByInertia = true,
            //MinInertiaRatio = 0.001f,

            FilterByColor = false
            //FilterByColor = true,
            //BlobColor = 255 // to extract light blobs
        }; 
        simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        // essentially agar 0 se 1920 hai range and we got 1100, it should be (1100-1920/2)/1920 * 4.5
        //Same for verticality, but x-1080/2/ 1080 *3
        transform.position=new Vector3(0,0,5);
        shouldRender = true;


    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mouseInput = !mouseInput;
            Cursor.visible = !mouseInput;
        }
        if (mouseInput)
        {
            StartCoroutine(LerpPosition(new Vector3(((Input.mousePosition.x - Screen.width / 2) / Screen.width * 1f) * 8f, ((Input.mousePosition.y - Screen.height / 2) / Screen.height * 1f) * 6f, 5), Time.deltaTime));

        }
        else
        {
            if (!shouldRender) return;
            currKeypoint = renderOCVframes(OpenCvSharp.Unity.TextureToMat(_webCamTexture));
            // transform.position = new Vector3(((currKeypoint.Pt.X - _webCamTexture.width / 2) / _webCamTexture.width * 1f) * 8f, ((currKeypoint.Pt.Y - _webCamTexture.height / 2) / _webCamTexture.height * -1f) * 6f, 5);
            StartCoroutine(LerpPosition(new Vector3(((currKeypoint.Pt.X - _webCamTexture.width / 2) / _webCamTexture.width * 1f) * 8f, ((currKeypoint.Pt.Y - _webCamTexture.height / 2) / _webCamTexture.height * -1f) * 6f, 5), Time.deltaTime));
            Debug.Log("X: " + currKeypoint.Pt.X + ", Y: " + currKeypoint.Pt.Y);
            Debug.Log("X: " + ((currKeypoint.Pt.X - _webCamTexture.width / 2) / _webCamTexture.width * 1f) * 8f + ", Y: " + ((currKeypoint.Pt.Y - _webCamTexture.height / 2) / _webCamTexture.height * -1f) * 6f);
        }

    }
    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }




    private KeyPoint renderOCVframes(Mat frame)
    {

        imgHSV = new Mat();
        ColorConversionCodes colorType = (ColorConversionCodes)Enum.Parse(typeof(ColorConversionCodes), "BGR2HSV");
        Cv2.CvtColor(frame, imgHSV, colorType);
        

        Cv2.InRange(imgHSV, new Scalar(Database.hsv.LowHue, Database.hsv.LowSat, Database.hsv.LowVal), new Scalar(Database.hsv.HighHue, Database.hsv.HighSat, Database.hsv.HighVal), imgHSV); //Threshold the image

        
        KeyPoint[] keyPoints = simpleBlobDetector.Detect(imgHSV);
        // Debug.Log("keyPoints: " + keyPoints.Length);
        kp = new KeyPoint();
        foreach (var keyPoint in keyPoints)
        {
            if (keyPoint.Size > kp.Size) kp = keyPoint;
            // Debug.Log("X: "+ keyPoint.Pt.X + ", Y: " +  keyPoint.Pt.Y);
            // frame.Circle(keyPoint.Pt,(int)keyPoint.Size, new Scalar( 0, 0,255),2);
            //frame.Rectangle(new OpenCvSharp.Rect(keyPoint.Pt.,), new Scalar(255, 255, 255), 1);
        }
        frame.Circle(kp.Pt, (int)kp.Size, new Scalar(0, 0, 255), 1);

        

        rend.material.mainTexture = OpenCvSharp.Unity.MatToTexture(frame);
        imgHSV.Release();
        frame.Release();
        return kp;


    }

    private void LateUpdate()
    {
        // get the current position
        Vector3 clampedPosition = transform.position;
        // limit the x and y positions to be between the area's min and max x and y.
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -4, 4);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -3, 3);
        // z remains unchanged
        // apply the clamped position
        transform.position = clampedPosition;
    }
}
