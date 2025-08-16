using UnityEngine;

[CreateAssetMenu(menuName = "Cards/08.EventCard")]
public class EventCardData : CardData
{
    public string functionKey1; // 실행할 함수 키워드
    public string functionKey2; // 실행할 함수 키워드
    public string eventResult1; // 이벤트 실행 후 결과 설명
    public string eventResult2; // 이벤트 실행 후 결과 설명
    public float probability;
}