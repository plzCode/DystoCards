using UnityEngine;

public class PlayerStat : Player
{

    [SerializeField] private JobType job;
    [SerializeField] private PropertyType property;

    private void Start()
    {
        SetJob(job);
        SetProperty(property);
    }


    public void SetJob(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.Warrior: Warrior(); break;
            case JobType.Mage: Mage(); break;
            case JobType.Archer: Archer(); break;
            case JobType.Thief: Theif(); break;
            case JobType.Priest: Priest(); break;
        }
        Debug.Log($"{jobType} initialized with AttackPower: {AttackPower}");
    }

    public void SetProperty(PropertyType propertyType)
    {
        switch (propertyType)
        {
            case PropertyType.Fire: AttackPower += 5f; Fire_resistance += 5f; break;
            case PropertyType.Ice: Defense += 5f; Ice_resistance += 5f; break;
            case PropertyType.Blood: Health += 20f; Blood_resistance += 5f; break;
        }
        Debug.Log($"{propertyType} property applied.");
    }



    public void Warrior()
    {
        AttackPower = 20f;
        Defense = 15f;
        Speed = 5f;
        Stress = 10f;
        Blood_resistance = 5f;
        Fire_resistance = 3f;
        Ice_resistance = 2f;
        DiceMax = 3;
        DiceMin = 1;
    }
    public void Mage()
    {
        AttackPower = 25f;
        Defense = 5f;
        Speed = 7f;
        Stress = 15f;
        Blood_resistance = 2f;
        Fire_resistance = 5f;
        Ice_resistance = 3f;
        DiceMax = 5;
        DiceMin = 2;
    }
    public void Archer()
    {
        AttackPower = 15f;
        Defense = 10f;
        Speed = 10f;
        Stress = 5f;
        Blood_resistance = 3f;
        Fire_resistance = 2f;
        Ice_resistance = 5f;
        DiceMax = 6;
        DiceMin = 1;
    }
    public void Theif()
    {
        AttackPower = 18f;
        Defense = 8f;
        Speed = 12f;
        Stress = 8f;
        Blood_resistance = 4f;
        Fire_resistance = 4f;
        Ice_resistance = 4f;
        DiceMax = 8;
        DiceMin = 1;
    }
    public void Priest()
    {
        AttackPower = 10f;
        Defense = 12f;
        Speed = 6f;
        Stress = 12f;
        Blood_resistance = 5f;
        Fire_resistance = 5f;
        Ice_resistance = 3f;
        DiceMax = 3;
        DiceMin = 2;
    }


}

public enum JobType
{
    Warrior,
    Mage,
    Archer,
    Thief,
    Priest
}

public enum PropertyType
{
    Fire,
    Ice,
    Blood
}
