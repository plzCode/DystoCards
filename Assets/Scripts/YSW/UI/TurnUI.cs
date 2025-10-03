using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnUI : MonoBehaviour
{
    [Header("UI 요소")]
    public CanvasGroup turnIcon_CanvasGroup;
    public Image turnImage;
    public TextMeshProUGUI turnText;
    public float fadeDuration = 0.5f;
    public float displayDuration = 1.5f;

    [System.Serializable]
    public class TurnPhaseSprite
    {
        public TurnPhase phase;
        public Sprite sprite;
    }

    public List<TurnPhaseSprite> phaseSprites;
    private Dictionary<TurnPhase, Sprite> spriteMap;

    private void Start()
    {
        spriteMap = new Dictionary<TurnPhase, Sprite>();
        foreach (var entry in phaseSprites)
        {
            if (!spriteMap.ContainsKey(entry.phase))
                spriteMap.Add(entry.phase, entry.sprite);
        }

        // 모든 Phase에 UI 표시 액션 등록
        foreach (TurnPhase phase in System.Enum.GetValues(typeof(TurnPhase)))
        {
            TurnManager.Instance.RegisterPhaseAction(phase, () => StartCoroutine(ShowPhaseUI(phase)));
        }
    }

    private IEnumerator ShowPhaseUI(TurnPhase phase)
    {
        string displayText = $"Turn {TurnManager.Instance.TurnCount}\n<size=120%>{phase}</size>";
        turnText.text = displayText;

        // 아이콘 변경
        if (spriteMap.TryGetValue(phase, out var sprite))
        {
            yield return StartCoroutine(SwitchTurnIcon(sprite));
        }

        // 텍스트 표시
        yield return StartCoroutine(ShowTurnText());

        // UI 완료 -> 다음 액션 실행
        TurnManager.Instance.MarkActionComplete();
    }

    private IEnumerator ShowTurnText()
    {
        turnText.gameObject.SetActive(true);
        CanvasGroup fadeCanvas = UIManager.Instance.fadeCanvas;

        yield return UIManager.Instance.FadeCanvas(fadeCanvas, 0f, 1f, fadeDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return UIManager.Instance.FadeCanvas(fadeCanvas, 1f, 0f, fadeDuration); // Fade Out

        turnText.gameObject.SetActive(false);
    }

    private IEnumerator SwitchTurnIcon(Sprite newSprite)
    {
        yield return UIManager.Instance.FadeCanvas(turnIcon_CanvasGroup, 1f, 0f, fadeDuration);
        turnImage.sprite = newSprite;
        yield return UIManager.Instance.FadeCanvas(turnIcon_CanvasGroup, 0f, 1f, fadeDuration);
    }
}
