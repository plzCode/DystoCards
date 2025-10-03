using JetBrains.Annotations;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float AttackPower { get; set; }
    public float Health { get; set; }
    public float Defense { get; set; }
    public float Speed { get; set; }
    public float Stress { get; set; }
    public float Blood_resistance { get; set; }
    public float Fire_resistance { get; set; }
    public float Ice_resistance { get; set; }
    public int DiceMax { get; set; }
    public int DiceMin { get; set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(AttackPower);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
