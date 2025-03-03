using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Collections;


public class MicrophoneListener : MonoBehaviour
{
    public float volume; 

    public int selectedMicroId = 0;
    private string selectedMicrophone;
    private AudioClip microphoneClip;
    public int clipDelay = 5;
    public int sampleWindow = 64;//количество анализируемых сэмплов

    public float threshold = 0.01f; // Порог громкости
    public Image microphoneIndicator; // Проверочная линия

    public bool isSaving = false;

    public bool shouldShowDropdown = true;
    public TMP_Dropdown microphoneDropdown;

    string tempPath;
    int audioCounter = 0;

    public ShowTimeDelay showTimeDelay;
    void Start()
    {
        tempPath = Path.Combine(Application.persistentDataPath, "Audio");
        Debug.Log("Путь к аудио: " + tempPath);

        microphoneIndicator.color = Color.red;

        List<string> microphoneDevices = new(Microphone.devices);

        if (shouldShowDropdown)
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

        StartMicrophone();
    }

    void Update()
    {
        if (microphoneClip == null) return;

        volume = GetVolumeFromMicrophone();

        microphoneIndicator.color = volume > threshold ? Color.green : Color.red;

        if (volume > threshold && !isSaving)
        {
            isSaving = true;
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

        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }
        return sum / sampleWindow;
    }

    IEnumerator Listening()
    {
        if (showTimeDelay) {
            showTimeDelay.startTimer();
        }

        yield return new WaitForSeconds(5);
        saveAudioClip();
        RestartRecord();
    }

    void saveAudioClip()
    {
        ++audioCounter;
        string filePath = Path.Combine(tempPath, "record" + audioCounter + ".wav");
        SavWav.Save(filePath, microphoneClip);
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
            microphoneClip = Microphone.Start(selectedMicrophone, true, clipDelay, 44100);
        }
        else
        {
            Debug.LogError("Микрофоны не найдены.");
        }
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
