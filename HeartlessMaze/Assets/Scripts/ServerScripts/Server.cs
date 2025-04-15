using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Server : MonoBehaviour
{
    private Process _serverProcess;
    private Client client;
    private bool serverReadyFlag = false;
    string modelPath = $"{Application.streamingAssetsPath}/model.onnx";
    string vocabPath = $"{Application.streamingAssetsPath}/vocab.json";

    void Start()
    {
        client = GetComponent<Client>();
        //string exePath = Path.Combine(Application.streamingAssetsPath, "model_script.exe");
        string exePath = $"{Application.streamingAssetsPath}/test_server.exe";

        // �������� ������������� �����
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
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // ������ �������
        _serverProcess = new Process { StartInfo = startInfo };
        _serverProcess.Start();

        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();
        _serverProcess.OutputDataReceived += (sender, args) => { 
            if (args.Data != null)
                UnityEngine.Debug.Log($"Server: {args.Data}");
            if (args.Data == "...py-server ready...") serverReadyFlag = true;  
        };
        _serverProcess.ErrorDataReceived += (sender, args) => {
            if (args.Data != null)
                UnityEngine.Debug.LogError($"Server (error): {args.Data}");
            };

        InvokeRepeating(nameof(CheckServerStatus), 1f, 1f);// �������� ������� ������ ������� (� ��� ������ ���� ������ �� ����������)
    }
    void CheckServerStatus()
    {
        if (serverReadyFlag)
        {
            UnityEngine.Debug.Log($"������ �������. PID = {_serverProcess.Id}");
            CancelInvoke(nameof(CheckServerStatus)); // ������������� ��������
            client.ConnectToPythonServer();
        }
        else
        {
            UnityEngine.Debug.Log("Waiting to start server");
        }
    }
}
