using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public Human human;
    public LocationInfo location;

    public int durationDays;     // �� �ҿ� �ϼ�
    public int remainingDays;    // ���� �ϼ�

    public ExplorationData(Human human, LocationInfo location)
    {
        this.human = human;
        this.location = location;
        this.durationDays = location.durationDays;
        this.remainingDays = location.durationDays;
    }
}
