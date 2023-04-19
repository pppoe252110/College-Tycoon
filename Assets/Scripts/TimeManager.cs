using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timer;
    public float timeBoost = 60;
    private float time = 43200;

    void Start()
    {
        
    }

    void Update()
    {
        time = Mathf.Repeat(time + Time.deltaTime * 60f, 86400);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timer.text = timeSpan.ToString(@"hh\:mm");
    }
}
