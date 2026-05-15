using System.Collections;
using TMPro;
using UnityEngine;
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

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }
    private void Start()
    {
        sendButton.onClick.AddListener(OnSend);
        closeButton.onClick.AddListener(CloseDialogue);
        dialoguePanel.SetActive(false);
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
        strikeText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        strikeText.color = Color.white;
    }
}