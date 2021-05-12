using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;

public class faceDetector : MonoBehaviour
{
    WebCamTexture _webCamTexture;
   
    
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
       
    }

    // Update is called once per frame
    void Update()
    {
       // GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);

        DoShit(frame);
            

        //FindNewFace(frame);
        //display(frame);

    }

    void DoShit(Mat frame)
    {


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
            //MaxArea = 500,

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



        Mat imgHSV = new Mat();
        ColorConversionCodes colorType = (ColorConversionCodes)Enum.Parse(typeof(ColorConversionCodes), "BGR2HSV");
        Cv2.CvtColor(frame, imgHSV, colorType);
        Mat imgThresholded = new Mat();

        Cv2.InRange(imgHSV, new Scalar(100, 100, 140), new Scalar(135, 255,255), imgThresholded); //Threshold the image

        Cv2.BitwiseAnd(imgHSV,imgHSV,imgHSV,imgThresholded);




        SimpleBlobDetector simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
        KeyPoint[] keyPoints = simpleBlobDetector.Detect(imgThresholded);
        Debug.Log("keyPoints: "+ keyPoints.Length);
        KeyPoint kp=new KeyPoint();
        foreach (var keyPoint in keyPoints)
        {
            if (keyPoint.Size > kp.Size) kp = keyPoint;
            // Debug.Log("X: "+ keyPoint.Pt.X + ", Y: " +  keyPoint.Pt.Y);
         // frame.Circle(keyPoint.Pt,(int)keyPoint.Size, new Scalar( 0, 0,255),2);
            //frame.Rectangle(new OpenCvSharp.Rect(keyPoint.Pt.,), new Scalar(255, 255, 255), 1);
        }
        frame.Circle(kp.Pt, (int)kp.Size, new Scalar(0, 0, 255), 1);

        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newTexture;
        
    }


}
