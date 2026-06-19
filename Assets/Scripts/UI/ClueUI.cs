using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClueUI : MonoBehaviour
{
    public static ClueUI Instance { get; private set; }

    [SerializeField] private GameObject cluePanel;
    [SerializeField] private TMP_Text clueTitleText;
    [SerializeField] private TMP_Text clueBodyText;
    //[SerializeField] private Image clueImage;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        cluePanel.SetActive(false);
        closeButton.onClick.AddListener(CloseClue);
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseClue();
        }
    }
    public void ShowClue(string title, string body, Sprite image = null)
    {
        Debug.Log($"Showing clue: {title}");
        clueTitleText.text = title;
        clueBodyText.text = body;
        //clueImage.gameObject.SetActive(image != null);
        //if (image != null) clueImage.sprite = image;

        cluePanel.SetActive(true);
        PlayerController.Instance.SetDialogueOpen(true); 
    }

    public void CloseClue()
    {
        cluePanel.SetActive(false);
        PlayerController.Instance.SetDialogueOpen(false);
    }
}