using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public StateManager state;
    public TextManager text;

    private void Update()
    {
        text.SetText(SecondsToString(state.GetInGameTimer()));
    }
    
    private string SecondsToString(float seconds)
    {
        int ms = Mathf.FloorToInt((seconds % 1f) * 1000f);
        int s = Mathf.FloorToInt(seconds % 60f);
        seconds /= 60f;
        int m = Mathf.FloorToInt(seconds % 60f);
        seconds /= 60f;
        int h = Mathf.FloorToInt(seconds);

        string msString = "" + ms;
        string sString = "" + s;
        string mString = "" + m;
        string hString = "" + h;

        if (ms < 10) msString = "00" + msString;
        else if (ms < 100) msString = "0" + sString;

        if (s < 10) sString = "0" + sString;

        if (m < 10) mString = "0" + mString;

        if (h < 10) hString = "0" + hString;

        return hString + ":" + mString + ":" + sString + "." + msString;
    }
}
