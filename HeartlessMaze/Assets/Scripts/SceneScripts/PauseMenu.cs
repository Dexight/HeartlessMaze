using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] arrows;
    public int currentButtonIndex = 0;

    [SerializeField] private PlayerMovement playerMovement;
    void Start()
    {
        
    }

    public void nextButton()
    {
        arrows[currentButtonIndex].gameObject.SetActive(false);
        currentButtonIndex++;
        if (currentButtonIndex == buttons.Length)
        {
            currentButtonIndex = 0;
        }
        arrows[currentButtonIndex].gameObject.SetActive(true);
        buttons[currentButtonIndex].Select();
    }

    public void previousButton()
    {
        arrows[currentButtonIndex].gameObject.SetActive(false);
        currentButtonIndex--;
        if (currentButtonIndex < 0)
        {
            currentButtonIndex = buttons.Length - 1;
        }
        arrows[currentButtonIndex].gameObject.SetActive(true);
        buttons[currentButtonIndex].Select();
    }

    public void pressButton()
    {
        switch (currentButtonIndex)
        {
            case 0:
                playerMovement.pressEscape();
                break;
            case 1:
                // TODO
                break;
            case 2:
                //TODO
                break;
            case 3:
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
                break;
        }
    }
}
