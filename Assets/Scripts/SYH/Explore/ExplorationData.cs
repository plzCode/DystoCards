using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public Card2D humanCard2D;
    public Human human;
    public LocationInfo location;

    public int durationDays;     // 총 소요 일수
    public int remainingDays;    // 남은 일수

    public ExplorationData(Human human, LocationInfo location, Card2D humanCard2D)
    {
        this.human = human;
        this.location = location;
        this.durationDays = location.durationDays;
        this.remainingDays = location.durationDays;
        this.humanCard2D = humanCard2D;
    }
}
