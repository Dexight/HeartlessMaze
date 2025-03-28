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
        //ConnectToPythonServer(); // Подключение к серверу
    }

    public void ConnectToPythonServer()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 12345); // Адрес и порт сервера
            stream = client.GetStream();
            UnityEngine.Debug.Log("Подключено к Python-серверу.");

            // Поток для получения данных с сервера
            receiveThread = new Thread(new ThreadStart(ReceiveData))
            {
                IsBackground = true
            };

            receiveThread.Start();
            canSend = true;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Ошибка подключения: {e}");
        }
    }

    // Основная функция отправки аудио на python сервер
    public void SendAudioPathToPython(string data)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            stream.Write(bytes, 0, bytes.Length);
            UnityEngine.Debug.Log($"Отправлены данные: {data}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Ошибка отправки данных: {e}");
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
                    UnityEngine.Debug.Log($"Получен результат: {result}");
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Ошибка получения данных: {e}");
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        // Закрытие соединения при завершении
        stream?.Close();
        client?.Close();
        receiveThread?.Abort();
    }
}
