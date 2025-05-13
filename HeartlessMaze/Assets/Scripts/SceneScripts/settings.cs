using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] Image audioBackgorund;

    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private PlayerMovement playerMovement;
    
    public GameObject[] arrows;
    public int currentButtonIndex = 0;

    public void nextButton()
    {
        arrows[currentButtonIndex].gameObject.SetActive(false);
        currentButtonIndex++;
        if (currentButtonIndex == 2)
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
            currentButtonIndex = 1;
        }
        arrows[currentButtonIndex].gameObject.SetActive(true);
    }

    public void pressButton()
    {
        if (currentButtonIndex == 1)
        {
            playerMovement.pressEscape();
        }
    }

    public void audioChangeLeft()
    {
        audioSource.volume -= 0.1f;
        audioBackgorund.fillAmount = audioSource.volume;
    }

    public void audioChangeRight()
    {
        audioSource.volume += 0.1f;
        audioBackgorund.fillAmount = audioSource.volume;
    }
}
