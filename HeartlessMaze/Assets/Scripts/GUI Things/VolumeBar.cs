using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    public List<Image> volumeBarImages;

    [ReadOnlyProperty][SerializeField] private MicrophoneListener microphoneListener;

    [UnityEngine.Range(0, 1)] public float bars_threshold;
    
    void Start()
    {
        microphoneListener = MicrophoneListener.Instance;
    }

    void Update()
    {
        float volume = microphoneListener.volume;
        foreach (Image barImage in volumeBarImages)
        {
            // ��������� �����
            if (volume > bars_threshold)
                 barImage.fillAmount = volume;
            else barImage.fillAmount = volume = 0;
            
            // ��������� �����
            if (volume >= microphoneListener.threshold)
            {
                barImage.color = Color.red;
            }
            else if (volume >= microphoneListener.threshold / 2)
            {
                barImage.color = new Color(255, 123, 0);
            }
            else 
            { 
                barImage.color = Color.white; 
            }
        }
    }
}
