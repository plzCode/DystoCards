using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public HumanCardData human;
    public LocationInfo location;

    public int durationDays;     // 총 소요 일수
    public int remainingDays;    // 남은 일수

    public ExplorationData(HumanCardData human, LocationInfo location)
    {
        this.human = human;
        this.location = location;
        this.durationDays = location.durationDays;
        this.remainingDays = location.durationDays;
    }
}
