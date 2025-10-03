using UnityEngine;

[System.Serializable]
public struct CardReward
{
    public bool useId;         // true면 id, false면 name
    public string idOrName;    // "002" 또는 "Wood"
    public int count;          // 보상량
}

[System.Serializable]
public struct FacilityAction
{
    public string id;          // 예: "forest_chop"
    public string label;       // 예: "벌목"
    public int cost;           // 스태미너 소모(휴식은 음수)

    // ★ 보상은 '카드 스폰'으로만 관리
    public CardReward[] rewards;   // 0개(휴식)~N개(청소 등 복수 보상)
}
