using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedRunTimer : MonoBehaviour
{
    public static SpeedRunTimer timerInstance;
    public Text timeCounter;
    public DialogueTrigger dialogueTrigger;
    public bool showtimer = true;

    private TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;

    private string timePlayingStr ="";

    private void Awake()
    {
        timerInstance = this;
    }

    void Start()
    {
        timeCounter.text = "";
        timerGoing = false;

        BeginTimer();
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(StartTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
        dialogueTrigger.addTime(timePlayingStr);
    }

    private IEnumerator StartTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            timePlayingStr = timePlaying.ToString("mm':'ss'.'ff");

            if (showtimer)
            {
                timeCounter.text = timePlayingStr;
            } else
            {
                timeCounter.text = "";
            }

            yield return null;
        }
    }

    public void TimerOn(bool value) { showtimer = value; }
}
