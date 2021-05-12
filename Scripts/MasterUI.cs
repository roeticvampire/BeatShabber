using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MasterUI : MonoBehaviour
{
    public bool checkingOCV = false;
    WebCamTexture _webCamTexture;
    public Renderer OCVBnW;
    public Renderer OCVLive;
    public Canvas mainMenu;
    public Canvas calibrateOCV;
    public Canvas songSelection;
    public Transform intialCam;
    public Transform OCVCam;
    public Transform songsCam;
    public float lerpTime;

    public Slider lowHue;
    public Slider lowSat;
    public Slider lowVal;
    public Slider highHue;
    public Slider highSat;
    public Slider highVal;
    static int count=0;

    public AudioClip withoutYou;
    public AudioClip goodThingsFallApart;
    public AudioClip dontLookDown;
    public AudioClip warrior;
    Mat imgHSV;
    Mat frame;
    KeyPoint kp;
    KeyPoint[] keyPoints;
    SimpleBlobDetector simpleBlobDetector;
    Scalar lowp=new Scalar();
    Scalar highp=new Scalar();






    // Start is called before the first frame update
    void Start()
    {
        mainMenu.enabled = true;
        calibrateOCV.enabled = false;
        songSelection.enabled = false;
        lowVal.value = Database.hsv.LowVal;
        highVal.value = Database.hsv.HighVal;
        lowSat.value = Database.hsv.LowSat;
        highSat.value = Database.hsv.HighSat;
        highHue.value = Database.hsv.HighHue;
        lowHue.value = Database.hsv.LowHue;


    }

    // Update is called once per frame
    void Update()
    {
        if ( checkingOCV)
        {
            frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture); 
            
            renderOCVframes(frame);
        }


    }

    private void FixedUpdate()
    {
    }



    private void renderOCVframes(Mat frame)
    {

        imgHSV = new Mat(); 

        Cv2.CvtColor(frame, imgHSV, (ColorConversionCodes)Enum.Parse(typeof(ColorConversionCodes), "BGR2HSV"));
        lowp.Val0 = Database.hsv.LowHue;
        lowp.Val1=Database.hsv.LowSat;
        lowp.Val2= Database.hsv.LowVal;

        highp.Val0 = Database.hsv.HighHue;
        highp.Val1 = Database.hsv.HighSat;
        highp.Val2 = Database.hsv.HighVal;

        // Cv2.InRange(imgHSV, new Scalar(Database.hsv.LowHue, Database.hsv.LowSat, Database.hsv.LowVal), new Scalar(Database.hsv.HighHue, Database.hsv.HighSat, Database.hsv.HighVal), imgThresholded); //Threshold the image
        Cv2.InRange(imgHSV, lowp, highp, imgHSV);
            //Cv2.BitwiseAnd(imgHSV, imgHSV, imgHSV, imgThresholded);
        
            keyPoints = simpleBlobDetector.Detect(imgHSV);
           // Debug.Log("keyPoints: " + keyPoints.Length);
            kp = new KeyPoint();
            foreach (var keyPoint in keyPoints)
            {
                if (keyPoint.Size > kp.Size) kp = keyPoint;
                // Debug.Log("X: "+ keyPoint.Pt.X + ", Y: " +  keyPoint.Pt.Y);
                // frame.Circle(keyPoint.Pt,(int)keyPoint.Size, new Scalar( 0, 0,255),2);
                //frame.Rectangle(new OpenCvSharp.Rect(keyPoint.Pt.,), new Scalar(255, 255, 255), 1);
            }
            frame.Circle(kp.Pt, (int)kp.Size, new Scalar(0, 0, 255), 4);

       
        OCVBnW.material.mainTexture = OpenCvSharp.Unity.MatToTexture(imgHSV);

        OCVLive.material.mainTexture = OpenCvSharp.Unity.MatToTexture(frame);

        imgHSV.Release();
        frame.Release();
        
       // imgThresholded.Dispose();
        //imgHSV.Dispose();
        //frame.Dispose();

    }

    //let's see the buttons we need to work with
    //
    //Start
    //Configure
    //Exit

    //within Songs
    //Without You
    //Dont look Down
    //Good things fall Apart

    //Caliberate screen
    //
    //6 sliders, each changing the current value of hsv xD
    //Confirm button

    public void StartButton()
    {
        mainMenu.enabled = false;
        songSelection.enabled = true;
        StartCoroutine(LerpPosition(songsCam.position,songsCam.rotation, lerpTime));
        //Change the cam to view that canvas not this
    }

    IEnumerator LerpPosition(Vector3 targetPosition, Quaternion endValue, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        Quaternion startValue = transform.rotation;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
    public void ConfigButton()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        mainMenu.enabled = false;
        calibrateOCV.enabled=true;
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

        simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
        StartCoroutine(LerpPosition(OCVCam.position, OCVCam.rotation, lerpTime));
        checkingOCV = true;
        //Change the cam to view that canvas not this
    }

    public void ConfirmButton()
    {
        _webCamTexture.Stop();
        mainMenu.enabled = true;
        songSelection.enabled = false;
        Database.SaveHSV();
        StartCoroutine(LerpPosition(intialCam.position, intialCam.rotation, lerpTime));

    }

    public void setSongWithoutYou()
    {
        Database.song = withoutYou;
        startGame();
    }
    public void setSongGoodThingsFallApart()
    {
        Database.song = goodThingsFallApart;
        startGame();
    }
    public void setSongDontLookDown()
    {
        Database.song = dontLookDown;
        startGame();
    }


    public void setSongWarrior()
    {
        Database.song = warrior;
        startGame();
    }


    void startGame() {
        
        SceneManager.LoadScene(1);

    }

    public void ExitButton()
    {
        Application.Quit();
    }


    public void sliderLowVal()
    {
        Database.hsv.LowVal = (int)lowVal.value;
        lowVal.value = Database.hsv.LowVal;
    }
    public void sliderHighVal()
    {
        Database.hsv.HighVal = (int)highVal.value;
        highVal.value = Database.hsv.HighVal;
    }
    public void sliderLowSat()
    {
        Database.hsv.LowSat = (int)lowSat.value;
        lowSat.value = Database.hsv.LowSat;
    }
    public void sliderHighSat()
    {
        Database.hsv.HighSat = (int)highSat.value;
        highSat.value = Database.hsv.HighSat;
    }
    public void sliderLowHue()
    {
        Database.hsv.LowHue = (int)lowHue.value;
        lowHue.value = Database.hsv.LowHue;
    }
    public void sliderHighHue()
    {
        Database.hsv.HighHue = (int)highHue.value;
        highHue.value = Database.hsv.HighHue;
    }















}
