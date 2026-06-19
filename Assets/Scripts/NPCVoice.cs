using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine.SceneManagement;

public class NPCVoice : MonoBehaviour
{
    [Header("Voice Settings")]
    [SerializeField] private string voiceModel = "en_GB-cori-medium.onnx"; 
    [SerializeField] private float lengthScale = 1.0f;

    private Process currentProcess;
    private string piperPath;
    private string voicePath;
    private string outputPath;

    private void Awake()
    {
        piperPath = Path.Combine(Application.streamingAssetsPath, "Piper", "piper.exe");
        voicePath = Path.Combine(Application.streamingAssetsPath, "Piper", "Voices", voiceModel);
        outputPath = Path.Combine(Application.temporaryCachePath, "tts_output.wav");

        UnityEngine.Debug.Log($"Piper path: {piperPath} | Exists: {File.Exists(piperPath)}");
        UnityEngine.Debug.Log($"Voice path: {voicePath} | Exists: {File.Exists(voicePath)}");
        UnityEngine.Debug.Log($"Output path: {outputPath}");
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Stop();
    private void OnDestroy() => Stop();

    public void Speak(string text)
    {
        Stop();
        text = text.Replace("[PASS]", "").Replace("[STRIKE]", "").Trim();
        if (string.IsNullOrEmpty(text)) return; 
        StartCoroutine(SpeakCoroutine(text));
    }

    private System.Collections.IEnumerator SpeakCoroutine(string text)
    {
        string textPath = Path.Combine(Application.temporaryCachePath, "tts_input.txt");
        File.WriteAllText(textPath, text);
        UnityEngine.Debug.Log($"Speaking: {text}");

        currentProcess = new Process();
        currentProcess.StartInfo = new ProcessStartInfo
        {
            FileName = piperPath,
            Arguments = $"--model \"{voicePath}\" " +
                        $"--length-scale {lengthScale} " +
                        $"--output-file \"{outputPath}\" " +
                        $"--input-file \"{textPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        currentProcess.Start();

        while (!currentProcess.HasExited)
            yield return null;

        UnityEngine.Debug.Log($"Piper exited. Output exists: {File.Exists(outputPath)}");

        if (File.Exists(outputPath))
        {
            string url = "file://" + outputPath;
            using var www = UnityEngine.Networking.UnityWebRequestMultimedia
                .GetAudioClip(url, AudioType.WAV);
            yield return www.SendWebRequest();

            UnityEngine.Debug.Log($"Audio load result: {www.result}");

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            }
            else
            {
                UnityEngine.Debug.LogError($"Audio load failed: {www.error}");
            }
        }
    }

    public void Stop()
    {
        if (currentProcess != null && !currentProcess.HasExited)
        {
            currentProcess.Kill();
            currentProcess.Dispose();
        }
        currentProcess = null;
        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            source.Stop();
    }
}