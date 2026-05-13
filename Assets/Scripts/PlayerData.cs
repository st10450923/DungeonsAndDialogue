using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public int StrikeCount { get; private set; } = 0;
    public const int MaxStrikes = 3;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddStrike()
    {
        StrikeCount++;
        return StrikeCount >= MaxStrikes; 
    }

    public void ResetStrikes() => StrikeCount = 0;
}