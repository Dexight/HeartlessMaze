using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] arrows;
    public int currentButtonIndex = 0;

    public bool inSettingMenu = false;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private settings settingsMenu;

    void Start()
    {
        settingsMenu.gameObject.SetActive(false);
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
                gotoSettings();
                break;
            case 2:
                Exit();
                break;

        }
    }

    public void Exit()
    {
        if (playerMovement._isPaused && !inSettingMenu)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    public void gotoSettings()
    {
        inSettingMenu = true;
        settingsMenu.currentButtonIndex = 0;
        settingsMenu.gameObject.SetActive(true);
    }
}
