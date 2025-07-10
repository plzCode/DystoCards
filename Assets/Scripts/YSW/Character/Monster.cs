using UnityEngine;

public class Monster : Character
{
    public override void Die()
    {
        base.Die(); // Call base class Die method
        DropLoot(); // Call the DropLoot method when the monster dies
    }
    public void DropLoot()
    {

    }
}
