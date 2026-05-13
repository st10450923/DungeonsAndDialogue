using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private RoomData[] rooms;
    public int CurrentRoomIndex { get; private set; } = 0;
    public List<string> CollectedClues { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClueCollected(string clueText)
    {
        CollectedClues.Add(clueText);
        OllamaManager.Instance.InjectContext(
            rooms[CurrentRoomIndex].guardId,
            $"The player just read: '{clueText}'"
        );
    }

    public void OnRoomCleared()
    {
        CurrentRoomIndex++;
        if (CurrentRoomIndex >= rooms.Length)
        {
            //Win screen trigger

        }
        else
            SceneManager.LoadScene("DungeonRoom");
    }
    public void ResetRoom()
    {
        CollectedClues.Clear();
        OllamaManager.Instance.ClearHistory(rooms[CurrentRoomIndex].guardId);
        PlayerData.Instance.ResetStrikes();

        // Find and reset the door
        FindFirstObjectByType<Door>()?.ResetDoor();

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}