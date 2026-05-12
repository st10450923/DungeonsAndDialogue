using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class OllamaTest : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;   // Player types here
    [SerializeField] private TMP_Text responseText;       // Response displays here
    [SerializeField] private Button sendButton;           // Click to send

    private void Start()
    {
        sendButton.onClick.AddListener(OnSend);
        responseText.text = "Waiting for response...";
    }


    private void OnSend()
    {
        string userMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;

        responseText.text = "Thinking...";
        sendButton.interactable = false;

        OllamaManager.Instance.SendMessage(
            npcId: "test_npc",
            userMessage: userMessage,
            systemPrompt: "You are a grumpy dungeon guard. Keep responses to 2 sentences.",
            onResponse: (reply) =>
            {
                responseText.text = reply;
                inputField.text = "";
                sendButton.interactable = true;
            },
            onError: (err) =>
            {
                responseText.text = $"<color=red>Error: {err}</color>";
                sendButton.interactable = true;
            }
        );
    }
}