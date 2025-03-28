using TMPro;
using UnityEngine;

public class ShowTimeDelay : MonoBehaviour
{
    public TMP_Text timer;
    public MicrophoneListener microphoneListener;
    public float timeRemaining;

    public bool showTime;
    void Start()
    {
        stopTimer();
    }

    void Update()
    {
        if (showTime)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining >= 0 )
                DisplayTime(timeRemaining);
        }
    }

    public void startTimer()
    {
        showTime = true;
    }

    void DisplayTime(float timeToDisplay)
    {
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;
        timer.text = string.Format("{0:00}:{1:000}", seconds, milliSeconds);
    }

    public void stopTimer()
    {
        timer.text = "";
        timeRemaining = microphoneListener.clipDelay;
        showTime = false;
    }
}
