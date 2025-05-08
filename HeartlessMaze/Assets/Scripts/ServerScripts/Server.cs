using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Server : MonoBehaviour
{
    private Process _serverProcess;
    private Client client;
    [ReadOnlyProperty][SerializeField] private bool serverReadyFlag = false;
    string modelPath = $"{Application.streamingAssetsPath}/model.onnx";
    string vocabPath = $"{Application.streamingAssetsPath}/vocab.json";
    string logsPath = $"{Application.streamingAssetsPath}/serverLogs.txt";

    private void Awake()
    {
        File.Delete(logsPath);
    }

    void Start()
    {
        Application.logMessageReceived += DisplayUnityLog;
        client = GetComponent<Client>();
        
        //string exePath = Path.Combine(Application.streamingAssetsPath, "model_script.exe");
        string exePath = $"{Application.streamingAssetsPath}/test_server.exe";

        // Проверка существования файла
        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError($"Server file not found: {exePath}");
            return;
        }
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = modelPath + " " + vocabPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Запуск сервера
        _serverProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        _serverProcess.OutputDataReceived += (sender, args) => {
            if (args.Data != null)
            {
                UnityEngine.Debug.Log($"Server: {args.Data}");
                File.AppendAllText(logsPath, $"[{System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] Server: {args.Data}\n");
            }

            if (args.Data == "...py-server ready...")
                serverReadyFlag = true;
        };
        _serverProcess.ErrorDataReceived += (sender, args) => {
            if (args.Data != null)
                UnityEngine.Debug.LogError($"Server (error): {args.Data}");
                File.AppendAllText(logsPath, $"[{System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] Server (error): {args.Data}\n");
        };

        _serverProcess.Start();

        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();

        InvokeRepeating(nameof(CheckServerStatus), 1f, 5f);// Вызывает функцию каждые 5 секунд (в моём случае пока сервер не поднимется)
    }


    private void CheckServerStatus()
    {
        if (serverReadyFlag)
        {
            UnityEngine.Debug.Log($"Server was started.");
            CancelInvoke(nameof(CheckServerStatus)); // Останавливаем проверку
            client.ConnectToPythonServer();
        }
        else
        {
            File.AppendAllText(logsPath, $"[{System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] Waiting to start server\n");
            //UnityEngine.Debug.Log("Waiting to start server");
        }
    }

    public bool IsServerReady()
    {
        return serverReadyFlag;
    }

    void DisplayUnityLog(string logString, string stackTrace, LogType type)
    {
        File.AppendAllText(logsPath, $"==============Unity debug==============\n[{System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] {logString}\n\n{stackTrace}==============Server debug=============\n");
    }

    void OnDisable()
    {
        Application.logMessageReceived -= DisplayUnityLog;
    }
}
