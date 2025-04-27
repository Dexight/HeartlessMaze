using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Collections;
using UnityEditor;

public class MicrophoneListener : MonoBehaviour
{
    #region Dropdown GUI 
    [Header("GUI(Optional)")]
    
    [Space]

    [Tooltip("Есть ли dropdown на экране. (выставляется вручную)")]
    public bool dropdownOnScreen = true;

    [Tooltip("Dropdown список аудио входов.")]
    [ReadOnlyProperty][SerializeField] private TMP_Dropdown microphoneDropdown;
    #endregion

    #region Options
    [Header("Options")]

    [Space]

    [Tooltip("Длина записываемого клипа в секундах.")]
    public int clipDelay = 2;

    [Tooltip("Количество анализируемых сэмплов.")]
    public int sampleWindow = 64;
    
    [Tooltip("Порог громкости.")]
    public float threshold = 0.1f;

    public float sensibility = 100f;
    #endregion

    #region Optional
    [Header("Optional")]

    [Space]

    [Tooltip("Выбранный ID микрофона.")]
    public int selectedMicroId = 0;

    [Tooltip("Громкость микрофона.")]
    [ReadOnlyProperty] public float volume;

    [Tooltip("Идет ли сохранение файла.")]
    [ReadOnlyProperty] public bool isSaving = false;

    [Tooltip("Проверочная линия. (зелёный если голос есть, красный если голоса нет)")]
    [ReadOnlyProperty][SerializeField] private Image microphoneIndicator;

    [Tooltip("Скрипт ShowTimeDelay")]
    [ReadOnlyProperty][SerializeField] private ShowTimeDelay showTimeDelay;

    public static MicrophoneListener Instance { get; private set; } // Статическая ссылка на единственный экземпляр (для синглтона)
    #endregion

    private string selectedMicrophone;
    private AudioClip microphoneClip;
    private string tempPath;
    private int audioCounter = 0;
    [ReadOnlyProperty][SerializeField] private Client client;
    [ReadOnlyProperty][SerializeField] private WaitCircle waitCircle;

    private void Awake()
    {
        // Если экземпляр уже существует и это не текущий объект, уничтожаем его
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняем между сценами
        }
    }

    void Start()
    {
        client = GetComponent<Client>();
        tempPath = Path.Combine(Application.persistentDataPath, "Audio");
        Directory.CreateDirectory(tempPath); // Создаст папку, если её нет
        Debug.Log("Путь к аудио: " + tempPath);

        if (microphoneIndicator)
            microphoneIndicator.color = Color.red;

        List<string> microphoneDevices = new(Microphone.devices);

        if (dropdownOnScreen)
        {
            if (microphoneDevices.Count > 0)
            {
                microphoneDropdown.AddOptions(microphoneDevices);

                microphoneDropdown.value = selectedMicroId;
                selectedMicrophone = Microphone.devices[selectedMicroId];
                Debug.Log("Выбран микрофон: " + selectedMicrophone);
                
                microphoneDropdown.RefreshShownValue();

                microphoneDropdown.onValueChanged.AddListener(OnMicrophoneDropdownChanged);
            }
            else
            {
                microphoneDropdown.AddOptions(new List<string> { "Микрофон не найден" });
            }
        }
        else
        {
            selectedMicrophone = Microphone.devices[selectedMicroId];
            Debug.Log("Выбран микрофон: " + selectedMicrophone);
        }
        StartMicrophone();
    }

    void Update()
    {
        if (microphoneClip == null) return;

        volume = GetVolumeFromMicrophone()*sensibility;

        if (microphoneIndicator)
            microphoneIndicator.color = volume > threshold ? Color.green : Color.red;

        if (volume >= threshold && !isSaving && client.canSend && (waitCircle? !waitCircle.isWorked : true))
        {
            isSaving = true;
            Debug.Log("Volume = " + volume);
            StartCoroutine(Listening());
        }
    }

    // Получения уровня громкости с микрофона
    private float GetVolumeFromMicrophone()
    {
        float[] samples = new float[sampleWindow];

        int micPosition = Microphone.GetPosition(selectedMicrophone) - sampleWindow;

        if (micPosition < 0) return 0;

        microphoneClip.GetData(samples, micPosition);

        float loudness = 0;
        foreach (float sample in samples)
        {
            loudness += Mathf.Abs(sample);
        }

        return loudness/sampleWindow;
    }

    IEnumerator Listening()
    {
        if (showTimeDelay) {
            showTimeDelay.startTimer();
        }

        yield return new WaitForSeconds(clipDelay);
        SaveAudioClip();
        RestartRecord();
    }

    void SaveAudioClip()
    {
        audioCounter = audioCounter < 100? ++audioCounter : 0;
        string filePath = Path.Combine(tempPath, "record" + audioCounter + ".wav");
        SavWav.Save(filePath, microphoneClip);
        client.SendAudioPathToPython(filePath);
    }

    void RestartRecord()
    {
        Microphone.End(selectedMicrophone);
        StartMicrophone();
        isSaving = false;
        if (showTimeDelay)
        {
            showTimeDelay.stopTimer();
        }
    }

    void OnMicrophoneDropdownChanged(int selectedIndex)
    {
        selectedMicroId = selectedIndex;
        selectedMicrophone = Microphone.devices[selectedMicroId];
        Debug.Log("Выбран микрофон: " + selectedMicrophone);

        // Останавливаем текущую запись и начинаем новую
        if (Microphone.IsRecording(selectedMicrophone))
        {
            Microphone.End(selectedMicrophone);
        }

        StartMicrophone();
    }

    void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneClip = Microphone.Start(selectedMicrophone, true, clipDelay+1, 44100);
        }
        else
        {
            Debug.LogError("Микрофоны не найдены.");
        }
    }

    public void setDropdown(TMP_Dropdown dropdown)
    {
        microphoneDropdown = dropdown;
    }

    public void setClient(Client c)
    {
        client = c;
    }

    public void setShowTimeDelay(ShowTimeDelay std)
    {
        showTimeDelay = std;
    }

    public void setWaitCircle(WaitCircle wc)
    {
        waitCircle = wc;
    }

    public void setIndicator(Image img)
    {
        microphoneIndicator = img;
    }

    void OnDestroy()
    {
        if (Microphone.IsRecording(selectedMicrophone))
        {
            Microphone.End(selectedMicrophone);
        }

        if (microphoneClip != null)
        {
            microphoneClip = null;
        }
    }
}
public class ReadOnlyProperty : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyProperty))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Отключаем редактирование поля
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Включаем редактирование для остальных полей
    }
}