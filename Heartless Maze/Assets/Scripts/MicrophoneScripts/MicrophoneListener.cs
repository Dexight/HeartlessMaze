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
    [Header("Dropdown GUI")]
    
    [Space]

    [Tooltip("���� �� dropdown �� ������. (������������ �������)")]
    public bool shouldShowDropdown = true;

    [Tooltip("Dropdown ������ ����� ������.")]
    public TMP_Dropdown microphoneDropdown;
    #endregion

    #region Options
    [Header("Options")]

    [Space]

    [Tooltip("����� ������������� �����.")]
    public int clipDelay = 5;

    [Tooltip("���������� ������������� �������.")]
    public int sampleWindow = 64;
    
    [Tooltip("����� ���������.")]
    public float threshold = 0.01f;
    #endregion

    #region Debug
    [Header("Debug")]

    [Space]

    [Tooltip("��������� ���������.")]
    public float volume;

    [Tooltip("���� �� ���������� �����.")]
    [ReadOnlyProperty] public bool isSaving = false;

    [Tooltip("��������� ID ���������.")]
    [ReadOnlyProperty] public int selectedMicroId = 0;

    [Tooltip("����������� �����. (������ ���� ����� ����, ������� ���� ������ ���)")]
    public Image microphoneIndicator;

    [Tooltip("������ ShowTimeDelay")]
    public ShowTimeDelay showTimeDelay; // ����� - ���������� ShowTimeDelay � MicrophoneListener �������
    #endregion

    private string selectedMicrophone;
    private AudioClip microphoneClip;
    private string tempPath;
    private int audioCounter = 0;

    void Start()
    {
        tempPath = Path.Combine(Application.persistentDataPath, "Audio");
        Debug.Log("���� � �����: " + tempPath);

        if (microphoneIndicator)
            microphoneIndicator.color = Color.red;

        List<string> microphoneDevices = new(Microphone.devices);

        if (shouldShowDropdown)
        {
            if (microphoneDevices.Count > 0)
            {
                microphoneDropdown.AddOptions(microphoneDevices);

                microphoneDropdown.value = selectedMicroId;
                selectedMicrophone = Microphone.devices[selectedMicroId];
                Debug.Log("������ ��������: " + selectedMicrophone);
                microphoneDropdown.RefreshShownValue();

                microphoneDropdown.onValueChanged.AddListener(OnMicrophoneDropdownChanged);
            }
            else
            {
                microphoneDropdown.AddOptions(new List<string> { "�������� �� ������" });
            }
        }

        StartMicrophone();
    }

    void Update()
    {
        if (microphoneClip == null) return;

        volume = GetVolumeFromMicrophone();

        if (microphoneIndicator)
            microphoneIndicator.color = volume > threshold ? Color.green : Color.red;

        if (volume > threshold && !isSaving)
        {
            isSaving = true;
            StartCoroutine(Listening());
        }
    }

    // ��������� ������ ��������� � ���������
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
        SaveAudioClip();
        RestartRecord();
    }

    void SaveAudioClip()
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
        Debug.Log("������ ��������: " + selectedMicrophone);

        // ������������� ������� ������ � �������� �����
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
            Debug.LogError("��������� �� �������.");
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
public class ReadOnlyProperty : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyProperty))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // ��������� �������������� ����
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // �������� �������������� ��� ��������� �����
    }
}