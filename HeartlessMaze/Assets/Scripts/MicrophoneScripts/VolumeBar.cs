using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    public List<Image> volumeBarImages;

    [SerializeField] private MicrophoneListener MicrophoneListener;

    [UnityEngine.Range(0, 1)] public float bars_threshold;
    
    void Start()
    {
    }

    void Update()
    {
        float volume = MicrophoneListener.volume;
        foreach (Image barImage in volumeBarImages)
        {
            // Изменение шкалы
            if (volume > bars_threshold)
                 barImage.fillAmount = volume;
            else barImage.fillAmount = volume = 0;
            
            // Изменение цвета
            if (volume >= MicrophoneListener.threshold)
            {
                barImage.color = Color.red;
            }
            else if (volume >= MicrophoneListener.threshold / 2)
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
