using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public Card2D humanCard2D;
    public Human human;
    public LocationInfo location;

    public int durationDays;     // �� �ҿ� �ϼ�
    public int remainingDays;    // ���� �ϼ�

    public ExplorationData(Human human, LocationInfo location, Card2D humanCard2D)
    {
        this.human = human;
        this.location = location;
        this.durationDays = location.durationDays;
        this.remainingDays = location.durationDays;
        this.humanCard2D = humanCard2D;
    }
}
