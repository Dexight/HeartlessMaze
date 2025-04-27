using System.Text;
using TMPro;
using UnityEngine;

public class LogPanel : MonoBehaviour
{
    public GameObject logPanel;
    public GameObject testPanel;
    public GameObject loadingText;

    //public Color normalTextColor;
    //public Color WarningTextColor;
    //public Color ErrorTextColor;

    public TMP_Text logText;
    
    [ReadOnlyProperty][SerializeField] private Client client;
    
    void Start()
    {
        client = MicrophoneListener.Instance.GetComponent<Client>();
        logText.text = "";
    }

    void Update()
    {
        if (client.canSend)
        {
            testPanel.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(false);
            logPanel.gameObject.SetActive(false);
        }
    }

    void DisplayLog(string logString, string stackTrace, LogType type)
    {
        logText.text += logString + '\n';
    }

    void OnEnable()
    {
        Application.logMessageReceived += DisplayLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= DisplayLog;
    }
}
