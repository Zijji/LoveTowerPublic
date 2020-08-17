using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHoldable : MonoBehaviour
{
    public int hand;    //Hand held by this item.
    public InvItem thisItem;    //Inventory item of this item for dropping/storing in inventory
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Holdables have an 'Attack' command. Not necessarily an attack - could just be used in some way.
    public virtual void Attack()
    {

    }
    public virtual void AttackSetPressed(bool newPressed)
    {
        
    }
    public virtual int GetAttackStage()
    {
        return 0;
    }
    public virtual bool GetAttack()
    {
        return false;
    }
    public virtual int GetAttackType()
    {
        return 0;
    }
    public virtual void CancelAttack()
    {

    }
    public virtual void Block()
    {
        
    }
    public virtual bool GetBlock()
    {
        return false;
    }
    public virtual bool HasBlocked()
    {
        return false;
    }
}
