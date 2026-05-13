using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Door : MonoBehaviour
{
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer doorSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite lockedSprite;

    [SerializeField] private float unlockDelay = 1.5f;
    private bool isUnlocked = false;

    public void Unlock()
    {
        if (isUnlocked) return;
        StartCoroutine(UnlockSequence());
    }
    private IEnumerator UnlockSequence()
    {
        yield return new WaitForSeconds(unlockDelay);

        isUnlocked = true;

        doorSprite.sprite = openSprite;

        doorCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUnlocked && other.CompareTag("Player"))
            GameManager.Instance.OnRoomCleared();
    }
    public void ResetDoor()
    {
        isUnlocked = false;
        doorCollider.enabled = true;
        doorSprite.sprite = lockedSprite;
    }
}