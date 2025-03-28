using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour
{

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    public bool canSend = false;

    void Start()
    {
        //ConnectToPythonServer(); // ����������� � �������
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
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"������ �������� ������: {e}");
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    UnityEngine.Debug.Log($"������� ���������: {result}");
                }
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
        // �������� ���������� ��� ����������
        stream?.Close();
        client?.Close();
        receiveThread?.Abort();
    }
}
