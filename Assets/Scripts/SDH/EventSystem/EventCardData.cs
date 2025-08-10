using UnityEngine;

[CreateAssetMenu(menuName = "Cards/08.EventCard")]
public class EventCardData : CardData
{
    public string functionKey; // 실행할 함수 키워드
    public string eventResult; // 이벤트 실행 후 결과 설명
}