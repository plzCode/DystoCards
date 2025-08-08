using System.Collections;
using UnityEngine;


public class CharacterCardBinder : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;

    void Start() => StartCoroutine(DelayBind());

    IEnumerator DelayBind()
    {
        yield return null;   // ① 스폰 완료
        // yield return null; // RuntimeData 로 판별하고 싶을 땐 주석 해제
        BindLoop();
    }

    void BindLoop()
    {
        int cnt = 0;

        foreach (var card in FindObjectsOfType<Card2D>())
        {
            // CardData 로 캐릭터 여부 판정
            if (card.cardData is not CharacterCardData) continue;

            if (!card.TryGetComponent(out CharacterCard2D _))
                card.gameObject.AddComponent<CharacterCard2D>();

            if (!card.TryGetComponent(out FacilityParentWatcher _))
                card.gameObject.AddComponent<FacilityParentWatcher>();

            cnt++;
        }

        Debug.Log($"[Binder] Bind 완료, 캐릭터 수 = {cnt}");
    }
}