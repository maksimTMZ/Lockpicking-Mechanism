using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float time;

    [SerializeField] Text timerText;

    static public TimerScript Instance { get; set; }
    
    void Start()
    {
        Instance = this;
    }
    
    void Update()
    {
        time -= Time.deltaTime;

        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ResetTimer()
    {
        time = 60;
    }


}
