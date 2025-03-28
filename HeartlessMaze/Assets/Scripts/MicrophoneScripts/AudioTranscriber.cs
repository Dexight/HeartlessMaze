using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Newtonsoft.Json;

public class AudioTranscriber : MonoBehaviour
{
    public bool processing = false;

    private string onnxModelPath;
    private string vocabPath;
    private string audiosPath;

    private InferenceSession session;
    private Dictionary<int, string> idxToToken;

    private void Awake()
    {
        onnxModelPath = Path.Combine(Application.streamingAssetsPath, "model.onnx");
        vocabPath = Path.Combine(Application.streamingAssetsPath, "vocab.json");
        audiosPath = Path.Combine(Application.persistentDataPath, "test_wav_audios");
    }

    void Start()
    {
        session = new InferenceSession(onnxModelPath);//�������� ������

        //���������� ������� �������
        idxToToken = LoadVocab(vocabPath);

        //ProcessAudios();
    }

    private Dictionary<int, string> LoadVocab(string path)
    {
        // ������ JSON �����
        string json = File.ReadAllText(path);

        // �������������� JSON � Dictionary<string, int>
        var tempDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

        // �������������� Dictionary<string, int> � Dictionary<int, string>
        Dictionary<int, string> vocab = new Dictionary<int, string>();
        foreach (var kvp in tempDict)
        {
            vocab[kvp.Value] = kvp.Key;
        }

        return vocab;
    }

    public void ProcessAudio(string filename)
    {
        processing = true;

        string file_path = Path.Combine(audiosPath, filename);

        var transcription = TranscribeAudio(file_path);
        Debug.Log($"File: {Path.GetFileName(file_path)}, Transcription: {transcription}");
    }

    float[] PrepareInput(string audioPath)
    {
        var audioClip = LoadAudioClip(audioPath);
        var samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);
        return samples;
    }

    AudioClip LoadAudioClip(string path)
    {
        using (var www = new WWW(path))
        {
            while (!www.isDone) { }
            return www.GetAudioClip();
        }
    }

    public string TranscribeAudio(string audioPath)
    {
        // 1. ���������� ������� ������
        float[] audioSamples = PrepareInput(audioPath);

        // 2. �������������� ����� � ������ � ���������� ������������
        var inputTensor = new DenseTensor<float>(audioSamples, new[] { 1, audioSamples.Length });

        // 3. �������� ������� ������ ��� ������
        // "input" - ��� �������� ���� ������
        var inputs = new List<NamedOnnxValue>{ NamedOnnxValue.CreateFromTensor("input", inputTensor) };

        // 4. �������� ������
        using (var results = session.Run(inputs))
        {
            // 5. ��������� �������� ������ ������
            var outputTensor = results.First().AsTensor<float>();

            // 6. ������������� ������ ������ � �����
            var transcription = DecodeOutput(outputTensor);

            return transcription;
        }
    }

    private string DecodeOutput(Tensor<float> outputTensor)
    {
        var outputArray = outputTensor.ToArray();
        var transcriptionTokens = new List<string>();

        // �����������, ��� ����� ������ - ��� ������������������ �������� �������
        for (int i = 0; i < outputArray.Length; i++)
        {
            int tokenIndex = (int)outputArray[i];
            if (idxToToken.TryGetValue(tokenIndex, out var token))
            {
                transcriptionTokens.Add(token);
            }
        }

        // ����������� ������� � ������
        return string.Join("", transcriptionTokens);
    }
}