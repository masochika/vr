using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//BGMOnClick()をコピー、数字を変える
//Buttonreset()に追加

public class BGM : MonoBehaviour {
    public AudioClip BGMSound1;
    public AudioClip BGMSound2;
    public AudioClip BGMSound3;
    private AudioSource audioSource;
    int button1=0;
    int button2 = 0;
    int button3 = 0;
    // Use this for initialization
    public void BGM1OnClick()//１曲目再生
     {
        if (button1 == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(BGMSound1);
            Debug.Log("BGM1On");
            Buttonreset();
            button1 = 1;
        }
        }
    public void BGM2OnClick()//２曲目再生
    {
        if (button2 == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(BGMSound2);
            Debug.Log("BGM2On");
            Buttonreset();
            button2 = 1;
        }
    }
    public void BGM3OnClick()//２曲目再生
    {
        if (button2 == 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(BGMSound3);
            Debug.Log("BGM3On");
            Buttonreset();
            button3 = 1;
        }
    }
    public void Buttonreset(){//曲が重複しないようにする
        button1 = 0;
        button2 = 0;
        button3 = 0;
    }
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
       
    }
}
