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

    [Tooltip("Другое")]
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

            UnityEngine.Debug.Log($"Получен результат: {dataToProcess}");
            if (waitCircle)
                waitCircle.stopWaitCircle();
        }
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
            if (waitCircle)
                waitCircle.startWaitCircle();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Ошибка отправки данных: {e}");
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
                    lock (this) // Блокировка для потокобезопасности
                    {
                        receivedData = result;
                        hasNewData = true;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Debug.Log("Поток был принудительно прерван.");
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
        SendAudioPathToPython("stop");
        try {
            cts.Cancel(); // Отменяем выполнение потока
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join(1000); // Даем потоку 1 секунду на завершение
                if (receiveThread.IsAlive)
                    receiveThread.Abort(); // Если поток не завершился, прерываем его
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

            UnityEngine.Debug.Log("Ресурсы клиента освобождены (корректно)");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Ошибка при освобождении ресурсов клиента: {e}");
        }
        UnityEngine.Debug.Log("Работа приложения завершена.");
    }
}
