using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class LogPanel : MonoBehaviour
{
    public GameObject logPanel;
    public GameObject testPanel;
    public GameObject loadingText;

    string logsPath = $"{Application.streamingAssetsPath}/serverLogs.txt";

    //public Color normalTextColor;
    //public Color WarningTextColor;
    //public Color ErrorTextColor;

    public TMP_Text logText;
    
    [ReadOnlyProperty][SerializeField] private Client client;

    private void Awake()
    {
        logText.text = "";
    }

    void Start()
    {
        client = MicrophoneListener.Instance.GetComponent<Client>();
    }

    void Update()
    {
        DisplayServerLog();
        if (client.canSend)
        {
            testPanel.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(false);
            logPanel.gameObject.SetActive(false);
        }
    }

    void DisplayServerLog()
    {
        logText.text = File.ReadAllText(logsPath);
    }
}
