using UnityEngine;
using TMPro;

public class GuardNPC : MonoBehaviour
{
    [Header("Guard Config")]
    [SerializeField] private RoomData roomData;
    [SerializeField] private Door door;

    private bool isConvinced = false;
    private int localStrikes = 0;
    private const int MaxStrikes = 3;

    public void StartDialogue()
    {
        if (isConvinced) return;
        DialogueUI.Instance.OpenDialogue(roomData.guardName, this);

        SendToGuard("*The player approaches*");
    }

    public void SendToGuard(string playerMessage)
    {
        // Build context from collected clues + strike count
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

        // Check for pass condition
        if (response.Contains("[PASS]"))
        {
            isConvinced = true;
            string cleanResponse = response.Replace("[PASS]", "").Trim();
            DialogueUI.Instance.ShowResponse(cleanResponse);
            DialogueUI.Instance.ShowPassEffect();
            door.Unlock();
            return;
        }

        // Check for fail/offend condition
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