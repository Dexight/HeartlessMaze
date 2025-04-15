using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour
{

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private string receivedData = "";
    private bool hasNewData = false;

    public bool canSend = false;

    [Tooltip("������")]
    public WaitCircle waitCircle;

    private CancellationTokenSource cts = new CancellationTokenSource();
    void Start()
    {
    }

    private void Update()
    {
        if (hasNewData)
        {
            string dataToProcess;
            lock (this)
            {
                dataToProcess = receivedData;
                hasNewData = false;
            }

            UnityEngine.Debug.Log($"������� ���������: {dataToProcess}");
            if (waitCircle)
                waitCircle.stopWaitCircle();
        }
    }

    public void ConnectToPythonServer()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 12345); // ����� � ���� �������
            stream = client.GetStream();
            UnityEngine.Debug.Log("���������� � Python-�������.");

            // ����� ��� ��������� ������ � �������
            receiveThread = new Thread(new ThreadStart(ReceiveData))
            {
                IsBackground = true
            };

            receiveThread.Start();
            canSend = true;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"������ �����������: {e}");
        }
    }

    // �������� ������� �������� ����� �� python ������
    public void SendAudioPathToPython(string data)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            stream.Write(bytes, 0, bytes.Length);
            UnityEngine.Debug.Log($"���������� ������: {data}");
            if (waitCircle)
                waitCircle.startWaitCircle();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"������ �������� ������: {e}");
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    lock (this) // ���������� ��� ������������������
                    {
                        receivedData = result;
                        hasNewData = true;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Debug.Log("����� ��� ������������� �������.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"������ ��������� ������: {e}");
                break;
            }
            
        }
    }

    void OnApplicationQuit()
    {
        SendAudioPathToPython("stop");
        try {
            cts.Cancel(); // �������� ���������� ������
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join(1000); // ���� ������ 1 ������� �� ����������
                if (receiveThread.IsAlive)
                    receiveThread.Abort(); // ���� ����� �� ����������, ��������� ���
            }

            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }

            if (client != null)
            {
                client.Close();
            }

            UnityEngine.Debug.Log("������� ������� ����������� (���������)");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"������ ��� ������������ �������� �������: {e}");
        }
        UnityEngine.Debug.Log("������ ���������� ���������.");
    }
}
