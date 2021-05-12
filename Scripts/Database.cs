using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    
    public static AudioClip song;
    public static HSVstore hsv;
    static Database db;
    void Awake()
    {
        if (db != null) Destroy(this.gameObject);
        else db = this;
        DontDestroyOnLoad(transform.gameObject);
        hsv = new HSVstore();
        hsv.LowHue=PlayerPrefs.GetInt("lowHue", 50);
        hsv.HighHue=PlayerPrefs.GetInt("highHue", 110);
        hsv.LowSat= PlayerPrefs.GetInt("lowSat", 160);
        hsv.HighSat=PlayerPrefs.GetInt("highSat", 255);
        hsv.LowVal=PlayerPrefs.GetInt("lowVal", 80);
        hsv.HighVal= PlayerPrefs.GetInt("highVal", 255);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SaveHSV()
    {
        PlayerPrefs.SetInt("lowHue", hsv.LowHue);
        PlayerPrefs.SetInt("highHue", hsv.HighHue);
        PlayerPrefs.SetInt("lowSat", hsv.LowSat);
        PlayerPrefs.SetInt("highSat", hsv.HighSat);
        PlayerPrefs.SetInt("lowVal", hsv.LowVal);
        PlayerPrefs.SetInt("highVal", hsv.HighVal);

    }

    

}
