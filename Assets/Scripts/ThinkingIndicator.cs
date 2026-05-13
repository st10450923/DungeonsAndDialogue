using UnityEngine;
using TMPro;
using System.Collections;

public class ThinkingIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text dotsText;
    [SerializeField] private float dotInterval = 0.4f;

    private Coroutine animCoroutine;

    public void Show()
    {
        gameObject.SetActive(true);
        animCoroutine = StartCoroutine(AnimateDots());
    }

    public void Hide()
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateDots()
    {
        string[] frames = { ".", "..", "..." };
        int i = 0;
        while (true)
        {
            dotsText.text = frames[i % frames.Length];
            i++;
            yield return new WaitForSeconds(dotInterval);
        }
    }
}