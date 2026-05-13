using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClueUI : MonoBehaviour
{
    public static ClueUI Instance { get; private set; }

    [SerializeField] private GameObject cluePanel;
    [SerializeField] private TMP_Text clueTitleText;
    [SerializeField] private TMP_Text clueBodyText;
    [SerializeField] private Image clueImage;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        cluePanel.SetActive(false);
        closeButton.onClick.AddListener(CloseClue);
    }

    public void ShowClue(string title, string body, Sprite image = null)
    {
        clueTitleText.text = title;
        clueBodyText.text = body;
        clueImage.gameObject.SetActive(image != null);
        if (image != null) clueImage.sprite = image;

        cluePanel.SetActive(true);
        PlayerController.Instance.SetDialogueOpen(true); // freeze movement
    }

    public void CloseClue()
    {
        cluePanel.SetActive(false);
        PlayerController.Instance.SetDialogueOpen(false);
    }
}