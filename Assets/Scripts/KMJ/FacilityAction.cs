using UnityEngine;

public struct FacilityAction
{
    public string id;            // 고유 ID (예: "farm_plant")
    public string label;         // 버튼 텍스트
    public int cost;             // 필요 스태미너
    public ResourceType reward;  // 어떤 자원
    public int amount;           // 보상량
}
