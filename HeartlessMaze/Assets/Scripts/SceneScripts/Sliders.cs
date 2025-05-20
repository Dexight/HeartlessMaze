using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Sliders : MonoBehaviour
{
    public MicrophoneListener listener;

    public Slider ThresholdSlider;
    public Slider SensibilitySlider;

    public TMP_Text thresholdValue;
    public TMP_Text sensibilityValue;

    void Start()
    {
        thresholdValue.text = listener.threshold.ToString();
        sensibilityValue.text = Mathf.RoundToInt(listener.sensibility/10).ToString();
    }

    public void OnThresholdChanged()
    {
        listener.threshold = ThresholdSlider.value / 10;
        thresholdValue.text = (ThresholdSlider.value / 10).ToString();
    }

    public void OnSensibilityChanged()
    {
        listener.sensibility = SensibilitySlider.value * 10;
        sensibilityValue.text = ((int)SensibilitySlider.value).ToString();
    }
}
