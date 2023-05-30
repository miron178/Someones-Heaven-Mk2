using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionControl : MonoBehaviour
{
    public TMP_Dropdown dropdownRes;

    // Start is called before the first frame update
    void Start()
    {
        List<string> resolutions = new List<string>();
        foreach (Resolution R in Screen.resolutions)
            resolutions.Add(R.ToString());
        dropdownRes.AddOptions(resolutions);

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution R = Screen.resolutions[i];
            if (Equals(R, Screen.currentResolution))
                dropdownRes.value = i;
        } 
    }

    public void SetResolution (int Index)
    {
        Resolution SelectedResolution = Screen.resolutions[Index];
        Screen.SetResolution(SelectedResolution.width, SelectedResolution.height, true, SelectedResolution.refreshRate);
    }
}
