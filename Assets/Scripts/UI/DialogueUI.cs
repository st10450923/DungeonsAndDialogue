using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private bool isThinking;

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
    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseDialogue();

        if (Keyboard.current.enterKey.wasPressedThisFrame&& !isThinking)
            OnSend();
    }
    public void OpenDialogue(string guardName, GuardNPC guard)
    {
        dialoguePanel.SetActive(true);
        currentGuard = guard;
        guardNameText.text = guardName;
        responseText.text = "";
        playerInputField.text = "";
        strikeText.text = "";
        PlayerController.Instance.SetDialogueOpen(true);
        playerInputField.ActivateInputField();
    }
    public void SetInputActive(bool active)
    {
        sendButton.interactable = active;
        playerInputField.interactable = active;
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
        isThinking = thinking;
        if (thinking) thinkingIndicator.Show(this);
        else thinkingIndicator.Hide(this);

        sendButton.interactable = !thinking;
        playerInputField.interactable = !thinking;
    }

    public void ShowStrike(int current, int max)
    {
        strikeText.text = $"Strikes: {current}/{max}";
        StartCoroutine(FlashStrike());
    }
    private IEnumerator PassSequence(string farewell, System.Action onComplete)
    {
        responseText.text = farewell;
        yield return new WaitForSeconds(3f);
        CloseDialogue();
        onComplete?.Invoke();
    }
    public void ShowPassEffect(string farewell, System.Action onComplete)
    {
        StartCoroutine(PassSequence(farewell, onComplete));
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
        for (int i = 0; i < 2; i++)
        {
            strikeText.color = Color.red;
            yield return new WaitForSeconds(0.4f);
            strikeText.color = Color.white;
        }
    }
}