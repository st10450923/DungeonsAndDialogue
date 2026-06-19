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
    //[SerializeField] private GameObject interactPrompt; 
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField]private MaterialPropertyBlock mpb;


    private bool hasBeenRead = false;

    //private void Start() => interactPrompt.SetActive(false);
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }
    public void Interact()
    {
        UI.ShowClue(clueName, clueText, clueSprite);

        if (!hasBeenRead)
        {
            hasBeenRead = true;
            GameManager.Instance.ClueCollected(clueText);

        }
    }
    private void SetOutline(bool enabled)
    {
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_OutlineEnabled", enabled ? 1f : 0f);
        renderer.SetPropertyBlock(mpb);
    }
    public string GetPrompt() => $"Press E to examine {clueName}";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SetOutline(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SetOutline(false);
    }
}