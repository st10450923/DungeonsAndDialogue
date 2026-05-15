using UnityEngine;
using TMPro;

public class GuardNPC : MonoBehaviour, IInteractable
{
    [Header("Guard Config")]
    [SerializeField] private RoomData roomData;
    [SerializeField] private Door door;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private MaterialPropertyBlock mpb;


    private bool isConvinced = false;
    private int localStrikes = 0;
    private const int MaxStrikes = 3;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }
    private void SetOutline(bool enabled)
    {
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_OutlineEnabled", enabled ? 1f : 0f);
        renderer.SetPropertyBlock(mpb);
    }
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
    public void StartDialogue()
    {
        if (isConvinced) return;
        DialogueUI.Instance.OpenDialogue(roomData.guardName, this);
        SendToGuard("*The player approaches*");
    }
    public void Interact() => StartDialogue();
    public string GetPrompt() => $"Press E to speak with {roomData.guardName}";
    public void SendToGuard(string playerMessage)
    {
        string context = BuildContext();

        OllamaManager.Instance.InjectContext(roomData.guardId, context);

        DialogueUI.Instance.SetThinking(true);

        OllamaManager.Instance.SendMessage(
            npcId: roomData.guardId,
            userMessage: playerMessage,
            systemPrompt: roomData.guardSystemPrompt,
            onResponse: HandleResponse,
            onError: (err) => DialogueUI.Instance.ShowError(err)
        );
    }

    private void HandleResponse(string response)
    {
        DialogueUI.Instance.SetThinking(false);

        // Check for pass
        if (response.Contains("[PASS]"))
        {
            isConvinced = true;
            string cleanResponse = response.Replace("[PASS]", "").Trim();
            DialogueUI.Instance.ShowResponse(cleanResponse);
            RequestFarewell();
            return;
        }

        // Check for fail
        if (response.Contains("[STRIKE]"))
        {
            localStrikes++;
            string cleanResponse = response.Replace("[STRIKE]", "").Trim();
            DialogueUI.Instance.ShowResponse(cleanResponse);
            DialogueUI.Instance.ShowStrike(localStrikes, MaxStrikes);

            if (localStrikes >= MaxStrikes)
                HandleRoomFail();

            return;
        }

        DialogueUI.Instance.ShowResponse(response);
    }
    private void RequestFarewell()
    {
        DialogueUI.Instance.SetThinking(true);
        DialogueUI.Instance.SetInputActive(false);

        OllamaManager.Instance.SendMessage(
            npcId: roomData.guardId,
            userMessage: "*The guard steps aside*",
            systemPrompt: roomData.guardSystemPrompt +
                          " You have just been convinced. Say a brief heartfelt goodbye " +
                          "in 2 sentences that explains why you are letting them pass. " +
                          "Do not include any tokens like [PASS] or [STRIKE].",
            onResponse: (farewell) =>
            {
                DialogueUI.Instance.SetThinking(false);
                DialogueUI.Instance.ShowResponse(farewell);
                DialogueUI.Instance.ShowPassEffect(farewell, () =>
                {
                    door.Unlock();
                });
            },
            onError: (err) =>
            {
                DialogueUI.Instance.SetThinking(false);
                door.Unlock();
            }
        );
    }
    private string BuildContext()
    {
        var clues = GameManager.Instance.CollectedClues;
        if (clues.Count == 0) return "";

        return $"The player has read the following clues in this room: " +
               string.Join(", ", clues) + ". " +
               $"They have {localStrikes} strikes against them.";
    }

    private void HandleRoomFail()
    {
        DialogueUI.Instance.CloseDialogue();
        GameManager.Instance.ResetRoom();
    }
}