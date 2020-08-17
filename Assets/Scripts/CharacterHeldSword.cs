using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHeldSword : CharacterHoldable
{
    //Finds the human characters associated with this object.
    public Character thisChar;
    public HumanCharacter thisHuman;
    public int thisHumanHand;
    //Facing directions
    public int faceDirection = 0;
    public float damage = 1;
    //Power Attack Damage and Wait times
    public float powerAttackDamage = 10;            //Adds 10 damage. Additive for now, may be multiplicative later.
    public float powerAttackWaitTime = 0.3f;        //The added wait time for power attacks.
    //Time for attack frames
    public float attackWindUp1Time = 0.1f;
    public float attackWindUp2Time = 0.4f;
    public float attackHitTime = 0.4f;
    public float attackRecoverTime = 0.4f;
    //Attack Types
    public int attackType = 0;      //0 == slash, 1 == overhead
    //Number of times you can feint 
    public int feintCount = 0;
    public int feintMax = 1;
    //Parry times
    public float attackParryTime = 0.0f;
    public float attackParryWait = 0.0f;
    public bool isParrying = false;
    //For blocking with the sword
    public bool isBlocking = false;
    public bool isBlockingHit = false;
    public float blockTime = 0.4f;
    public float blockRecoilTime = 0.2f;    //Recoil from blocking a hit.
    public float blockWait = 0.0f;
    public float blockRecharge = 0.5f;     //block recharge for humancharacter waiting.
    public float blockHitRecharge = 0.3f;     //block recharge for if the block is hit .

    //Is attacking?
    public bool isAttacking = false;
    public bool isAttackingPressed = false;
    public float attackWait = 0.0f;
    public enum AttackStage { noAttack, windup1, windup2, hit, recover };
    public AttackStage curAttackStage = AttackStage.noAttack;
    public GameObject attackHitBox;

    public float attackRecharge = 0.5f;     //Attack recharge for humancharacter waiting.
    //Feint check
    public bool feintCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        thisChar = GetComponent<Character>();
        thisHuman = GetComponent<HumanCharacter>();
        //Finds which hand the sword is held in.
        thisHumanHand = thisHuman.FindWeaponInHand(this);
    }

    // Update is called once per frame
    void Update()
    {
        faceDirection = thisHuman.faceDirection;
        //cannot attack if grappling or if you are grappled
        //cannot attack if fallen down
        if((GetComponent<HumanFallenDown>() != null) || (GetComponent<HumanCauseGrappling>() != null) || (GetComponent<HumanIsGrappled>() != null))
        {
            isAttacking =  false;
            AttackSetPressed(false);
        }

        if(isBlocking)
        {
            if(blockWait > 0.0f)
            {
                blockWait -= Time.deltaTime;
                if(thisChar.GetHasDefended())
                {
                    isBlockingHit = true;
                    blockWait = blockRecoilTime;
                }
                if(isBlockingHit)
                {
                    thisChar.Defend(false, -1, -1);  //Parry Defence
                }
                else
                {
                    //Inverts faceDirection
                    int invertFDBlock = -1;
                    if(faceDirection == 2)
                    {
                        invertFDBlock = 8;
                    }
                    else if(faceDirection == 4)
                    {
                        invertFDBlock = 6;
                    }
                    else if(faceDirection == 6)
                    {
                        invertFDBlock = 4;
                    }
                    else if(faceDirection == 8)
                    {
                        invertFDBlock = 2;
                    }
                    thisChar.Defend(true, invertFDBlock, 4);  //Block Defence
                }

            }
            else
            {
                if(!isBlockingHit)
                {
                    thisHuman.SetAttackWaitAllMin(blockRecharge, thisHumanHand); 
                }
                else
                {
                    thisHuman.SetAttackWaitAllMin(blockHitRecharge, thisHumanHand); 
                }
                
                thisChar.Defend(false, -1, -1);  //Parry Defence
                isBlocking = false;
                isBlockingHit = false;
            }
        }

        if(isAttacking)
        {
            if(attackWait <= 0.0f)
            {
                switch(curAttackStage)
                {
                    case AttackStage.noAttack:
                        curAttackStage = AttackStage.windup1;
                        attackWait = attackWindUp1Time;
                        //feintCheck = false;
                        break;
                    case AttackStage.windup1:
                        curAttackStage = AttackStage.windup2;
                        attackWait = attackWindUp2Time;
                        isParrying = true;
                        if(isAttackingPressed)
                        {
                            attackType = 1;
                            
                            attackWait += powerAttackWaitTime;
                        }
                        else
                        {
                            attackType = 0;
                        }
                        break;
                    case AttackStage.windup2:
                        curAttackStage = AttackStage.hit;
                        attackWait = attackHitTime;
                        //Creates box here
                        Vector3 hitboxloc = new Vector3(0.0f,0.0f,0.0f);
                        switch(faceDirection)
                        {
                            case 2: //down
                                hitboxloc = new Vector3(0.0f,0.0f,-1.0f);
                                break;
                            case 4: //left
                                hitboxloc = new Vector3(-1.0f,0.0f,0.0f);
                                break;
                            case 6: //right
                                hitboxloc = new Vector3(1.0f,0.0f,0.0f);
                                break;
                            case 8: //up
                                hitboxloc = new Vector3(0.0f,0.0f,1.0f);
                                break;
                        }
                        var newHB = Instantiate(attackHitBox, transform.position + hitboxloc, transform.rotation);
                        float newHBDamage = damage;
                        if(attackType == 1)
                        {
                            newHBDamage += powerAttackDamage;
                        }
                        newHB.GetComponent<Hitbox>().UpdateDamage(newHBDamage);
                        newHB.GetComponent<Hitbox>().UpdateCharObj(this.gameObject);
                        int[] damageAreas = new int[] {5};
                        //Fixed overhead slash with slash
                        if(faceDirection == 4)
                        {
                            damageAreas = new int[] {8, 2, 5, 9, 6, 3};
                        }
                        else if(faceDirection == 8)
                        {
                            damageAreas = new int[] {1, 2, 5, 4, 6, 3};
                        }
                        else if(faceDirection == 2)
                        {
                            damageAreas = new int[] {7, 8, 9, 4, 5, 6};
                        }
                        else if(faceDirection == 6)
                        {
                            damageAreas = new int[] {7, 4, 1, 8, 5, 2};
                        }
                        //Slash or overhead attack
                        /*
                        if(attackType == 0)
                        {
                            //damageAreas = new int[] {5};
                            if((faceDirection == 4)
                            || (faceDirection == 6)
                            )
                            {
                                damageAreas = new int[] {8, 2, 5};
                            }
                            else if((faceDirection == 8)
                            || (faceDirection == 2)
                            )
                            {
                                damageAreas = new int[] {4, 5, 6};
                            }
                        }
                        else if(attackType == 1)
                        {
                            //damageAreas = new int[] {5};
                            if((faceDirection == 4)
                            || (faceDirection == 6)
                            )
                            {
                                damageAreas = new int[] {4, 5, 6};
                            }
                            else if((faceDirection == 8)
                            || (faceDirection == 2)
                            )
                            {
                                damageAreas = new int[] {8, 2, 5};
                            }
                            //damageAreas = new int[] {5};
                        }
                        */
                        newHB.GetComponent<Hitbox>().UpdateDamageArea(damageAreas);
                        break;
                    case AttackStage.hit:
                        curAttackStage = AttackStage.recover;
                        attackWait = attackRecoverTime;
                        break;
                    case AttackStage.recover:
                        curAttackStage = AttackStage.noAttack;
                        attackWait = 0.0f;
                        isAttacking = false;
                        feintCount = 0; //restores feint count.
                        thisHuman.SetAttackWaitAllMin(attackRecharge, thisHumanHand); 
                        break;
                }
            }
            else
            {
                attackWait -= Time.deltaTime;
            }
        }
        /*
        else
        {
            //CancelAttack();
            //curAttackStage = AttackStage.noAttack;
            //attackWait = 0.0f;
        }
        */
        if(isParrying)
        {
            thisChar.Defend(true, faceDirection, 3);  //Parry Defence
            attackParryWait +=  Time.deltaTime;
            if(attackParryWait > attackParryTime)
            {
                thisChar.Defend(false, -1, -1);  //Parry Defence
                isParrying = false;
                attackParryWait = 0.0f;
            }

        }
    }
    public override void Block()
    {
        if((!isAttacking)
        &&(!isBlocking)
        )
        {
            isBlocking = true;
            blockWait = blockTime;
            
        }
        
    }
    public override bool GetBlock()
    {
        return isBlocking;
    }
    
    public override void Attack()
    {
        //faceDirection = 0;
        if(!isBlocking)
        {
            isAttacking = true;
        }
    }
    public override void AttackSetPressed(bool newPressed)
    {
        isAttackingPressed = newPressed;
        //Debug.Log("Attack Pressed: "+isAttackingPressed);
        /*
        if(isAttackingPressed == false)
        {
            feintCheck = true;
        }
        */
        if(isAttackingPressed == true
        && feintCheck == false)
        {
            Feint();
        }
        feintCheck = isAttackingPressed;  //Sets to true when feint has been checked.
        //Feint();
    }
    public override bool GetAttack()
    {
        return isAttacking;
    }
    public override int GetAttackStage()
    {
        return (int) curAttackStage;
    }
    public override int GetAttackType()
    {
        return attackType;
    }

    //Does a feint, or a fake attack.
    public void Feint()
    {
        //Number of times you can feint
        if(feintCount >= feintMax)
        {
            //CancelAttack();
        }
        else
        {
            //Can only set the attack type during the no attack or windup phases
            if(//(curAttackStage == AttackStage.noAttack)
            //|| (curAttackStage == AttackStage.windup1)
            //(curAttackStage == AttackStage.windup1)
            (curAttackStage == AttackStage.windup2)
            )
            {
                attackWait = 0.0f;
                curAttackStage = AttackStage.noAttack;
                feintCount++;
            }
            
        }
        //else cancels the attack

        
    }
    public override void CancelAttack()
    {
        curAttackStage = AttackStage.noAttack;
        attackWait = 0.0f;
        isAttacking = false;
        feintCount = 0; //restores feint count. 
        //hum.Attack(-1);
    }
    public override bool HasBlocked()
    {
        return isBlockingHit;
    }
}
