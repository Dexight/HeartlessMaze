using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Server : MonoBehaviour
{
    private Process _serverProcess;
    private Client client;
    private bool serverReadyFlag = false;
    //string modelPath = Path.Combine(Application.streamingAssetsPath, "model.onnx");
    //string vocabPath = Path.Combine(Application.streamingAssetsPath, "vocab.json");

    void Start()
    {
        client = GetComponent<Client>();
        //string exePath = Path.Combine(Application.streamingAssetsPath, "model_script.exe");
        string exePath = Path.Combine(Application.streamingAssetsPath, "test_server.exe");

        // Проверка существования файла
        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError($"Server file not found: {exePath}");
            return;
        }

        //UnityEngine.Debug.Log(modelPath);
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            //Arguments = modelPath + " " + vocabPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Запуск сервера
        _serverProcess = new Process { StartInfo = startInfo };
        _serverProcess.Start();

        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();
        _serverProcess.OutputDataReceived += (sender, args) => { 
            UnityEngine.Debug.Log($"Server: {args.Data}");
            if (args.Data == "...py-server ready...") serverReadyFlag = true;  
        };
        _serverProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError($"Server (error): {args.Data}");

        InvokeRepeating(nameof(CheckServerStatus), 1f, 1f);
    }
    void CheckServerStatus()
    {
        if (serverReadyFlag)
        {
            UnityEngine.Debug.Log("Сервер запущен");
            CancelInvoke(nameof(CheckServerStatus)); // Останавливаем проверку
            client.ConnectToPythonServer();
        }
        else
        {
            UnityEngine.Debug.Log("Waiting to start server");
        }
    }
    void OnApplicationQuit()
    {
        // Убиваем процесс сервера
        if (_serverProcess != null && !_serverProcess.HasExited)
            _serverProcess.Kill();
    }
}
