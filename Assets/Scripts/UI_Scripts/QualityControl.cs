using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QualityControl : MonoBehaviour
{
    public TMP_Dropdown dropdownQuality;

    // Start is called before the first frame update
    void Start()
    {
     
        dropdownQuality.AddOptions(new List<string>(QualitySettings.names));

        dropdownQuality.value = QualitySettings.GetQualityLevel();
    }

    public void SetQuality(int Index)
    {
        QualitySettings.SetQualityLevel(Index, true);
    }
}
