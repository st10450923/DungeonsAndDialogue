using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaManager : MonoBehaviour
{
    [Header("Ollama Settings")]
    [SerializeField] private string ollamaURL = "http://localhost:11434/api/chat";
    [SerializeField] private string modelName = "llama3.2:3b";
    [SerializeField] private int maxTokens = 60;

    [Header("NPC Defaults")]
    [TextArea(3, 6)]
    [SerializeField]
    private string defaultSystemPrompt =
        "You are an NPC in a dark fantasy dungeon. Respond in character. " +
        "Keep responses under 3 sentences. Never break character.";
    const string BaseRules = @"
You will ONLY allow the player to pass if they sincerely convince you they have no interest in your magical items and will not take them.
If the player is rude, dismissive, or tries to threaten or bribe you, reprimand them and end your response with exactly: [STRIKE]
If the player genuinely moves you or convinces you, end your response with exactly: [PASS]
Keep responses to 2-3 sentences. Never break character. Never reveal your condition directly.
STRICT RULES:
- Respond with only spoken words your character says out loud.
- Never write stage directions, actions, descriptions, or thoughts.
- Never use asterisks, brackets (except [STRIKE] or [PASS]), or narrative text.
- Bad example (do NOT do this): She eyes you suspiciously. ""Stay away from my things!""
- Good example: ""Keep your hands where I can see them. I know what wanderers like you are after.""";
    public static OllamaManager Instance { get; private set; }

    private Dictionary<string, List<Message>> conversationHistories = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LaunchOllama();

    }

    private void LaunchOllama()
    {
        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "serve",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not start Ollama: {e.Message}");
        }
    }
    public void SendMessage(
        string npcId,
        string userMessage,
        string systemPrompt,
        Action<string> onResponse,
        Action<string> onError = null)
    {
        if (!conversationHistories.ContainsKey(npcId))
            conversationHistories[npcId] = new List<Message>();

        conversationHistories[npcId].Add(new Message("user", userMessage));

        string prompt = string.IsNullOrEmpty(systemPrompt) ? defaultSystemPrompt : systemPrompt;
        StartCoroutine(PostToOllama(npcId, prompt, onResponse, onError));
    }


    public void InjectContext(string npcId, string contextNote)
    {
        if (!conversationHistories.ContainsKey(npcId))
            conversationHistories[npcId] = new List<Message>();

        conversationHistories[npcId].Add(new Message("system", contextNote));
    }

    public void ClearHistory(string npcId)
    {
        if (conversationHistories.ContainsKey(npcId))
            conversationHistories[npcId].Clear();
    }

    private IEnumerator PostToOllama(
        string npcId,
        string systemPrompt,
        Action<string> onResponse,
        Action<string> onError)
    {
        var requestBody = new ChatRequest
        {
            model = modelName,
            stream = false,
            num_predict = maxTokens,
            messages = BuildMessageList(npcId, systemPrompt)
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using UnityWebRequest request = new UnityWebRequest(ollamaURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string err = $"Ollama request failed: {request.error}";
            Debug.LogError(err);
            onError?.Invoke(err);
            yield break;
        }

        string responseText = ParseResponse(request.downloadHandler.text);

        if (string.IsNullOrEmpty(responseText))
        {
            string err = "Ollama returned an empty response.";
            Debug.LogWarning(err);
            onError?.Invoke(err);
            yield break;
        }

        conversationHistories[npcId].Add(new Message("assistant", responseText));

        onResponse?.Invoke(responseText);
    }

    private List<Message> BuildMessageList(string npcId, string systemPrompt)
    {
        string fullprompt = $"{systemPrompt}\n\n{BaseRules}";
        var messages = new List<Message>
        {
            new Message("system", fullprompt)
        };
        messages.AddRange(conversationHistories[npcId]);
        return messages;
    }

    private string ParseResponse(string json)
    {
        try
        {
            ChatResponse response = JsonUtility.FromJson<ChatResponse>(json);
            return response?.message?.content ?? string.Empty;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse Ollama response: {e.Message}\nRaw: {json}");
            return string.Empty;
        }
    }

    [Serializable]
    private class ChatRequest
    {
        public string model;
        public bool stream;
        public int num_predict;
        public List<Message> messages;
    }

    [Serializable]
    private class ChatResponse
    {
        public Message message;
    }

    [Serializable]
    private class Message
    {
        public string role;
        public string content;
        public Message(string role, string content) { this.role = role; this.content = content; }
    }
}