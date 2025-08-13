using TMPro;
using UnityEngine;
using System.Collections;

public class UITextFade : MonoBehaviour
{
    TMP_Text tmpText;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void ChangeTextWithFade(string newText, float fadeTime = 0.5f)
    {
        StartCoroutine(FadeChangeRoutine(newText, fadeTime));
    }

    IEnumerator FadeChangeRoutine(string newText, float fadeTime)
    {
        Color c = tmpText.color;

        // 페이드 아웃
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            tmpText.color = c;
            yield return null;
        }

        // 텍스트 변경 후 페이드 인
        tmpText.text = newText;

        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            tmpText.color = c;
            yield return null;
        }

        c.a = 1f;
        tmpText.color = c;
    }
}
