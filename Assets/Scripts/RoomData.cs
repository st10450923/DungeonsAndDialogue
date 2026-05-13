using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Dungeon/Room Data")]
public class RoomData : ScriptableObject
{
    public string guardId;
    public string guardName;
    [TextArea(3, 6)] public string guardSystemPrompt;
    [TextArea(2, 4)] public string roomDescriptionPrompt;
    public string[] clueTexts;
}