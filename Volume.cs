using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{

    public UnityEngine.Audio.AudioMixer mixer;

    public void masterVol(Slider slider)
    {
        mixer.SetFloat("master", slider.value);
    }
}
