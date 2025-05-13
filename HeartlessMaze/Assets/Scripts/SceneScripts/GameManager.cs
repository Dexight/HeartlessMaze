using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public string currentScene = "Loading";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void LoadingScene()
    {
        currentScene = "Loading";
        SceneManager.LoadScene("Loading");
    }

    public void GameScene()
    {
        currentScene = "Game";
        SceneManager.LoadScene("Game");
    }
}
