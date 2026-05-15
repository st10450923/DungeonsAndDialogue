using UnityEngine;
using TMPro;

public class Clue : MonoBehaviour, IInteractable
{
    [Header("Clue Content")]
    [SerializeField] private string clueName;
    [TextArea(2, 5)]
    [SerializeField] private string clueText;
    [SerializeField] private Sprite clueSprite; 

 [Header("Visuals")]
    [SerializeField] private ClueUI UI;
    [SerializeField] private GameObject interactPrompt; 
    [SerializeField] private SpriteRenderer glowEffect;

    private bool hasBeenRead = false;

    //private void Start() => interactPrompt.SetActive(false);

    public void Interact()
    {
        UI.ShowClue(clueName, clueText, clueSprite);

        if (!hasBeenRead)
        {
            hasBeenRead = true;
            // Notify GameManager so it gets injected into the guard's context
            /*GameManager.Instance.ClueCollected(clueText);
            if (glowEffect != null)
   glowEffect.color = new Color(1f, 1f, 1f, 0.3f);            */
        }
    }

    public string GetPrompt() => $"Press E to examine {clueName}";
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            interactPrompt.SetActive(false);
    }*/
}