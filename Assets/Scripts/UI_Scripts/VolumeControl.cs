using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string ParameterName;
    public TextMeshProUGUI text;

    public void SetVolume(float volume)
    {
        text.text = volume.ToString("P0");
        volume = -80f * (1 - volume);
        audioMixer.SetFloat(ParameterName, volume);
    }




    // Start is called before the first frame update
    void Start()
    {
       
    }
}
