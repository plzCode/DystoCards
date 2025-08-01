using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class TurnUI : MonoBehaviour
{
    [Header("UI ���")]
    public CanvasGroup turnIcon_CanvasGroup;
    public Image turnImage;
    public TextMeshProUGUI turnText;          // �� �ؽ�Ʈ
    public float fadeDuration = 0.5f;         // ���̵� �ӵ�
    public float displayDuration = 1.5f;      // ǥ�� �ð�

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
        TurnManager.Instance.OnTurnPhaseChanged += OnTurnPhaseChanged;
        
        spriteMap = new Dictionary<TurnPhase, Sprite>();
        foreach (var entry in phaseSprites)
        {
            if (!spriteMap.ContainsKey(entry.phase))
                spriteMap.Add(entry.phase, entry.sprite);
        }
        
        // ���� ���� �� ���� �� ǥ��
        OnTurnPhaseChanged(TurnManager.Instance.CurrentPhase);
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnPhaseChanged -= OnTurnPhaseChanged;
    }

    private void OnTurnPhaseChanged(TurnPhase newPhase)
    {
        
        string displayText = $"Turn {TurnManager.Instance.TurnCount}\n<size=120%>{newPhase}</size>";
        turnText.text = displayText;
        StartCoroutine(ShowTurnText());

        if(spriteMap.TryGetValue(newPhase, out var sprite))
        {
            StartCoroutine(SwitchTurnIcon(sprite));
        }

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
        // ���̵� �ƿ�
        yield return UIManager.Instance.FadeCanvas(turnIcon_CanvasGroup, 1f, 0f, fadeDuration);

        // �̹��� ��ü
        turnImage.sprite = newSprite;

        // ���̵� ��
        yield return UIManager.Instance.FadeCanvas(turnIcon_CanvasGroup, 0f, 1f, fadeDuration);
    }


}
