using UnityEngine;
using TMPro;
using System.Collections;

public class ThinkingIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text dotsText;
    [SerializeField] private float dotInterval = 0.4f;

    private Coroutine animCoroutine;

    public void Show(MonoBehaviour host)
    {
        gameObject.SetActive(true);
        animCoroutine = host.StartCoroutine(AnimateDots());
    }

    public void Hide(MonoBehaviour host)
    {
        if (animCoroutine != null)
        {
            host.StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
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