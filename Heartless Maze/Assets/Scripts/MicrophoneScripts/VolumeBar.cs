using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    public List<Image> volumeBarImages;

    [SerializeField] private MicrophoneListener MicrophoneListener;

    void Start()
    {
    }

    void Update()
    {
        foreach (Image barImage in volumeBarImages)
            barImage.fillAmount = MicrophoneListener.volume;
    }
}
