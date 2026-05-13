using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private ThinkingIndicator thinkingIndicator;

    [Header("Text Elements")]
    [SerializeField] private TMP_Text guardNameText;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private TMP_Text strikeText;
    [SerializeField] private TMP_InputField playerInputField;

    [Header("Buttons")]
    [SerializeField] private Button sendButton;
    [SerializeField] private Button closeButton;

    private GuardNPC currentGuard;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        sendButton.onClick.AddListener(OnSend);
        closeButton.onClick.AddListener(CloseDialogue);
        dialoguePanel.SetActive(false);
    }

    public void OpenDialogue(string guardName, GuardNPC guard)
    {
        currentGuard = guard;
        guardNameText.text = guardName;
        responseText.text = "";
        playerInputField.text = "";
        strikeText.text = "";
        dialoguePanel.SetActive(true);
        PlayerController.Instance.SetDialogueOpen(true);
        playerInputField.ActivateInputField();
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        currentGuard = null;
        PlayerController.Instance.SetDialogueOpen(false);
    }

    public void ShowResponse(string text) => responseText.text = text;

    public void SetThinking(bool thinking)
    {
        if (thinking) thinkingIndicator.Show();
        else thinkingIndicator.Hide();

        sendButton.interactable = !thinking;
        playerInputField.interactable = !thinking;
    }

    public void ShowStrike(int current, int max)
    {
        strikeText.text = $"Strikes: {current}/{max}";
        StartCoroutine(FlashStrike());
    }

    public void ShowPassEffect()
    {
        CloseDialogue();
    }

    public void ShowError(string err) => responseText.text = $"<color=red>{err}</color>";

    private void OnSend()
    {
        string message = playerInputField.text.Trim();
        if (string.IsNullOrEmpty(message) || currentGuard == null) return;

        playerInputField.text = "";
        currentGuard.SendToGuard(message);
    }

    private IEnumerator FlashStrike()
    {
        strikeText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        strikeText.color = Color.white;
    }
}