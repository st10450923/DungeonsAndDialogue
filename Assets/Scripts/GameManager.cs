using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private RoomData[] rooms;
    [SerializeField] private Scene[]Levels;
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
    public void StartGame()
    {
        CurrentRoomIndex = 0;
        CollectedClues.Clear();
        SceneManager.LoadScene("Room1");
    }
    public void QuitGame()
    {
               Application.Quit();
    }
    public void OnRoomCleared()
    {
        CollectedClues.Clear();
        CurrentRoomIndex++;
        if (CurrentRoomIndex >= rooms.Length)
            SceneManager.LoadScene("WinScreen"); 
        else
            SceneManager.LoadScene("Room" + (1+CurrentRoomIndex)); 
    }
    public void ResetRoom()
    {
        CollectedClues.Clear();
        OllamaManager.Instance.ClearHistory(rooms[CurrentRoomIndex].guardId);
        PlayerData.Instance.ResetStrikes();

        var door = FindFirstObjectByType<Door>();
        if (door != null) door.ResetDoor();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}