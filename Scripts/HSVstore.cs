using System.Collections;
using System.Collections.Generic;

public class HSVstore
{
    int lowHue,highHue,lowSat,highSat,lowVal,highVal;

    public HSVstore()
    {
        LowHue = 0;
        LowSat = 0;
        LowVal = 0;
        HighHue = 179;
        HighSat = 255;
        HighVal = 255;
    }

    public int LowHue { get => lowHue; set => lowHue = value<=highHue?value:highHue; }
    public int HighHue { get => highHue; set => highHue = value >= lowHue ? value : lowHue; }
    public int LowSat { get => lowSat; set => lowSat = value <= highSat ? value : highSat; }
    public int HighSat { get => highSat; set => highSat = value >= lowSat? value : lowSat; }
    public int LowVal { get => lowVal; set => lowVal = value <= highVal ? value : highVal; }
    public int HighVal { get => highVal; set => highVal = value >= lowVal ? value : lowVal; }
}
