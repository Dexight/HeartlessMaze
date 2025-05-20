using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class settings : MonoBehaviour
{
    private MicrophoneListener microphoneListener;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] Image audioBackgorund;
    [SerializeField] Image thresholdBackground;
    [SerializeField] Image sensibilityBackground;

    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private TMP_Text thresholdText;
    [SerializeField] private TMP_Text sensibilityText;

    public GameObject[] arrows;
    [SerializeField] private float curThreshold;
    [SerializeField] private int curSensibility;
    public int currentButtonIndex = 0;

    private void Start()
    {
        microphoneListener = MicrophoneListener.Instance;

        curThreshold = Mathf.RoundToInt(microphoneListener.threshold * 10f);
        curSensibility = Mathf.RoundToInt(microphoneListener.sensibility / 10f);

        thresholdText.text = (curThreshold/10f).ToString();
        sensibilityText.text = curSensibility.ToString();

        thresholdBackground.fillAmount = curThreshold/30f;
        sensibilityBackground.fillAmount = curSensibility/20f;
    }

    public void nextButton()
    {
        arrows[currentButtonIndex].gameObject.SetActive(false);
        currentButtonIndex++;
        if (currentButtonIndex == 4)
        {
            currentButtonIndex = 0;
        }
        arrows[currentButtonIndex].gameObject.SetActive(true);
    }

    public void previousButton()
    {
        arrows[currentButtonIndex].gameObject.SetActive(false);
        currentButtonIndex--;
        if (currentButtonIndex < 0)
        {
            currentButtonIndex = 3;
        }
        arrows[currentButtonIndex].gameObject.SetActive(true);
    }

    public void pressButton()
    {
        if (currentButtonIndex == 3)
        {
            playerMovement.pressEscape();
        }
    }

    public void audioChangeLeft()
    {
        switch (currentButtonIndex)
        {
            case 0://audio
                audioSource.volume -= 0.1f;
                audioBackgorund.fillAmount = audioSource.volume; 
                break;
            case 1://threshold
                if (curThreshold > 1)
                {
                    curThreshold -= 1f;
                    microphoneListener.threshold = curThreshold / 10f;
                    thresholdText.text = (curThreshold / 10f).ToString();
                    thresholdBackground.fillAmount = curThreshold / 30f;
                }
                break;
            case 2://sensibility
                if (curSensibility > 1)
                {
                    curSensibility -= 1;
                    microphoneListener.sensibility = curSensibility * 10;
                    sensibilityText.text = curSensibility.ToString();
                    sensibilityBackground.fillAmount = curSensibility / 20f;
                }
                break;
            default: break;
        }
    }

    public void audioChangeRight()
    {
        switch (currentButtonIndex)
        {
            case 0://audio
                audioSource.volume += 0.1f;
                audioBackgorund.fillAmount = audioSource.volume;
                break;
            case 1://threshold
                if (curThreshold < 30)
                {
                    curThreshold += 1f;
                    microphoneListener.threshold = curThreshold / 10f;
                    thresholdText.text = (curThreshold / 10f).ToString();
                    thresholdBackground.fillAmount = curThreshold / 30f;
                }
                break;
            case 2://sensibility
                if (curSensibility < 20)
                {
                    curSensibility += 1;
                    microphoneListener.sensibility = curSensibility * 10;
                    sensibilityText.text = curSensibility.ToString();
                    sensibilityBackground.fillAmount = curSensibility / 20f;
                }
                break;
            default: break;
        }
    }
}
