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
        session = new InferenceSession(onnxModelPath);//загрузка модели

        //считывание словаря токенов
        idxToToken = LoadVocab(vocabPath);

        //ProcessAudios();
    }

    private Dictionary<int, string> LoadVocab(string path)
    {
        // Чтение JSON файла
        string json = File.ReadAllText(path);

        // Десериализация JSON в Dictionary<string, int>
        var tempDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

        // Преобразование Dictionary<string, int> в Dictionary<int, string>
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
        // 1. Подготовка входных данных
        float[] audioSamples = PrepareInput(audioPath);

        // 2. Преобразование аудио в тензор с правильной размерностью
        var inputTensor = new DenseTensor<float>(audioSamples, new[] { 1, audioSamples.Length });

        // 3. Создание входных данных для модели
        // "input" - имя входного узла модели
        var inputs = new List<NamedOnnxValue>{ NamedOnnxValue.CreateFromTensor("input", inputTensor) };

        // 4. Инференс модели
        using (var results = session.Run(inputs))
        {
            // 5. Получение выходных данных модели
            var outputTensor = results.First().AsTensor<float>();

            // 6. Декодирование выхода модели в текст
            var transcription = DecodeOutput(outputTensor);

            return transcription;
        }
    }

    private string DecodeOutput(Tensor<float> outputTensor)
    {
        var outputArray = outputTensor.ToArray();
        var transcriptionTokens = new List<string>();

        // Предположим, что выход модели - это последовательность индексов токенов
        for (int i = 0; i < outputArray.Length; i++)
        {
            int tokenIndex = (int)outputArray[i];
            if (idxToToken.TryGetValue(tokenIndex, out var token))
            {
                transcriptionTokens.Add(token);
            }
        }

        // Объединение токенов в строку
        return string.Join("", transcriptionTokens);
    }
}