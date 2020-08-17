using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHeldPunch : CharacterHoldable
{
    //Finds the human characters associated with this object.
    public Character thisChar;
    public HumanCharacter thisHuman;
    public int thisHumanHand;
    //Facing directions
    public int faceDirection = 0;
    public float damage = 1;
    //Time for attack frames
    public float attackWindUp1Time = 0.4f;
    public float attackWindUp2Time = 0.4f;
    public float attackHitTime = 0.4f;
    public float attackRecoverTime = 0.4f;
    //Attack Types
    public int attackType = 0;      //0 == slash, 1 == overhead
    //Number of times you can feint 
    public int feintCount = 0;
    public int feintMax = 1;
    //Parry times
    public float attackParryTime = 0.1f;
    public float attackParryWait = 0.0f;
    public bool isParrying = false;
    //Is attacking?
    public bool isAttacking = false;
    public bool isAttackingPressed = false;
    public float attackWait = 0.0f;
    public enum AttackStage { noAttack, windup1, windup2, hit, recover };
    public AttackStage curAttackStage = AttackStage.noAttack;
    public enum GrappleStage { noAttack, windup1, hit};
    public GrappleStage curGrappleStage = GrappleStage.noAttack;
    public float grappleWindUp1Time = 0.25f;
    public float grappleHitTime = 0.3f;
    public float grappleRecoverTime = 0.5f;
    private int grapFaceDir = 0;
    [SerializeField]
    private bool isSwappingLoc = false;
    public GameObject attackHitBox;
    public GameObject grappleHitBox;
    public GameObject shoveHitBox;  //Variant of hitbox which is used to shove.
    public GameObject tripHitBox;  //Variant of hitbox which is used to shove.
    public int grappleReleaseType = 0;  //0 == throw, 1 == trip
    [SerializeField]
    private HumanCauseGrappling causeGrap = null;

    public float attackRecharge = 0.2f;     //Attack recharge for humancharacter waiting.
    public float grappleRecharge = 2.0f;     //Attack recharge for after grapples.
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

    //Sum of attack times.
    private float SumAttackTimes()
    {
        return attackWindUp1Time + attackWindUp2Time + attackHitTime + attackRecoverTime;
    }
    void LateUpdate()
    {
        if(isSwappingLoc == true)
        {
            Vector3 storedVec = causeGrap.GetGrappledHuman().transform.position;
            Vector3 storedVec2 = transform.position;
            causeGrap.GetGrappledHuman().transform.position = Vector3.zero;
            transform.position = Vector3.zero + new Vector3(1f,1f,1f);
            causeGrap.GetGrappledHuman().transform.position = storedVec2;
            transform.position = storedVec;
            isSwappingLoc = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //cannot attack if fallen down
        if((GetComponent<HumanFallenDown>() != null))
        {
            isAttacking =  false;
            AttackSetPressed(false);
        }
        faceDirection = thisHuman.faceDirection;
        //Cause Grap up here for special grappling attacks like throws
        if(causeGrap != null)
        {
            if(curAttackStage != AttackStage.noAttack)
            {
                curAttackStage = AttackStage.noAttack;
                attackWait = 0.0f;
                isAttacking = false;
            }
            if(isAttacking)
            {
                if(attackWait <= 0.0f)
                {
                    //public enum GrappleStage { noAttack, windup1, windup2, hit, recover };
                    causeGrap.GetIsGrappledScr().DisableBreakGrapple();
                    switch(curGrappleStage)
                    {
                        case GrappleStage.noAttack:
                            grapFaceDir = thisHuman.GetFaceDirection();
                            attackWait = grappleWindUp1Time;
                            curGrappleStage = GrappleStage.windup1;
                            break;
                        case GrappleStage.windup1:
                            grappleReleaseType = 0;
                            if(isAttackingPressed)
                            {
                                grappleReleaseType = 1;
                            }
                            curGrappleStage = GrappleStage.hit;
                            attackWait = grappleHitTime;
                            if(grapFaceDir == causeGrap.GetIsGrappledScr().GetGrapplerCauserDir())
                            {
                                thisHuman.SetCheckCharHere(false);
                                causeGrap.GetGrappledHuman().GetComponent<HumanCharacter>().SetCheckCharHere(false);
                                //Vector3 storedVec = causeGrap.GetGrappledHuman().transform.position;
                                //Vector3 storedVec2 = transform.position;
                                //causeGrap.GetGrappledHuman().transform.position = Vector3.zero;
                                //transform.position = new Vector3(1.0f,0.0f,0.0f);
                                //causeGrap.GetGrappledHuman().transform.position = storedVec2;
                                //transform.position = storedVec;
                                isSwappingLoc = true;
                            }
                            break;
                        case GrappleStage.hit:
                            thisHuman.SetCheckCharHere(true);
                            causeGrap.GetGrappledHuman().GetComponent<HumanCharacter>().SetCheckCharHere(true);
                            /*  Dummied out grapple cancel code. Too finnicky!
                            int targetMoveDir = causeGrap.GetGrappledHuman().GetComponent<HumanCharacter>().GetMoveDirection();
                            if( causeGrap.GetGrappledHuman().GetComponent<HumanCharacter>().GetIsMovingInput())
                            {
                                if(!(
                                    (targetMoveDir == 2 && grapFaceDir == 8)
                                    ||(targetMoveDir == 4 && grapFaceDir == 6)
                                    ||(targetMoveDir == 8 && grapFaceDir == 2)
                                    ||(targetMoveDir == 6 && grapFaceDir == 4)
                                ))
                                {
                                    //Throws if attack is not held, trips otherwise.
                                    if(grappleReleaseType == 1)
                                    {
                                        var newHB = Instantiate(tripHitBox, causeGrap.GetGrappledHuman().transform.position, transform.rotation);
                                        newHB.GetComponent<HitboxTrip>().UpdateDamage(0);
                                        newHB.GetComponent<HitboxTrip>().UpdateCharObj(null);//this.gameObject);
                                        newHB.GetComponent<HitboxTrip>().UpdateTripMagnitude(1.0f);
                                    }
                                    else
                                    {
                                        var newHB = Instantiate(shoveHitBox, causeGrap.GetGrappledHuman().transform.position, transform.rotation);
                                        newHB.GetComponent<HitboxShove>().UpdateDamage(0);
                                        newHB.GetComponent<HitboxShove>().UpdateCharObj(null);//this.gameObject);
                                        newHB.GetComponent<HitboxShove>().UpdateMoveDirection(grapFaceDir);//causeGrap.GetIsGrappledScr().GetGrapplerCauserDir());//thisHuman.GetMoveDirection());
                                        newHB.GetComponent<HitboxShove>().UpdateMoveMagnitude(1);
                                    }
                                }
                            }
                            */
                            //Throws if attack is not held, trips otherwise.
                            if(grappleReleaseType == 1)
                            {
                                var newHB = Instantiate(tripHitBox, causeGrap.GetGrappledHuman().transform.position, transform.rotation);
                                newHB.GetComponent<HitboxTrip>().UpdateDamage(0);
                                newHB.GetComponent<HitboxTrip>().UpdateCharObj(null);//this.gameObject);
                                newHB.GetComponent<HitboxTrip>().UpdateTripMagnitude(1.0f);
                            }
                            else
                            {
                                var newHB = Instantiate(shoveHitBox, causeGrap.GetGrappledHuman().transform.position, transform.rotation);
                                newHB.GetComponent<HitboxShove>().UpdateDamage(0);
                                newHB.GetComponent<HitboxShove>().UpdateCharObj(null);//this.gameObject);
                                newHB.GetComponent<HitboxShove>().UpdateMoveDirection(grapFaceDir);//causeGrap.GetIsGrappledScr().GetGrapplerCauserDir());//thisHuman.GetMoveDirection());
                                newHB.GetComponent<HitboxShove>().UpdateMoveMagnitude(1);
                            }
                            isAttacking = false;
                            thisHuman.SetAttackWaitAllMin(grappleRecharge, thisHumanHand); 
                            isAttackingPressed = false;
                            attackWait = grappleRecoverTime;
                            causeGrap.EndGrapple();
                            causeGrap = null;
                            curGrappleStage = GrappleStage.noAttack;
                            break;
                    }
                    
                    /*
                    var newHB = Instantiate(shoveHitBox, causeGrap.GetGrappledHuman().transform.position, transform.rotation);
                    newHB.GetComponent<HitboxShove>().UpdateDamage(0);
                    newHB.GetComponent<HitboxShove>().UpdateCharObj(null);//this.gameObject);
                    newHB.GetComponent<HitboxShove>().UpdateMoveDirection(thisHuman.GetFaceDirection());//causeGrap.GetIsGrappledScr().GetGrapplerCauserDir());//thisHuman.GetMoveDirection());
                    newHB.GetComponent<HitboxShove>().UpdateMoveMagnitude(1);
                    if(thisHuman.GetFaceDirection() == causeGrap.GetIsGrappledScr().GetGrapplerCauserDir() )
                    {
                        Vector3 storedVec = causeGrap.GetGrappledHuman().transform.position;
                        causeGrap.GetGrappledHuman().transform.position = transform.position;
                        transform.position = storedVec;
                    }
                    
                    isAttacking = false;
                    thisHuman.SetAttackWaitAllMin(attackRecharge, thisHumanHand); 
                    isAttackingPressed = false;
                    attackWait = grappleRecoverTime;
                    causeGrap.EndGrapple();
                    causeGrap = null;
                    */
                }
                else
                {
                    attackWait -= Time.deltaTime;
                }
            
            }
            
        }
        else
        {
            
            //cannot attack if grappling or if you are grappled
            if((GetComponent<HumanCauseGrappling>() != null) || (GetComponent<HumanIsGrappled>() != null))
            {
                isAttacking =  false;
            }
            if(isAttacking)
            {
                if(attackWait <= 0.0f)
                {
                    switch(curAttackStage)
                    {
                        case AttackStage.noAttack:
                            if(!thisChar.CanMove())
                            {
                                isAttacking = false;
                            }
                            else
                            {
                                thisChar.Stumble(SumAttackTimes(), 1);
                                curAttackStage = AttackStage.windup1;
                                attackWait = attackWindUp1Time;
                                //feintCheck = false;
                            }
                            
                            break;
                        case AttackStage.windup1:
                            curAttackStage = AttackStage.windup2;
                            attackWait = attackWindUp2Time;
                            isParrying = true;
                            if(isAttackingPressed)
                            {
                                attackType = 1;
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
                            //grappling
                            if(attackType == 1)
                            {
                                GrappleHitBox newGrabBox = Instantiate(grappleHitBox, transform.position + hitboxloc, transform.rotation).GetComponent<GrappleHitBox>();
                                newGrabBox.UpdateOwnerCharObj(this.gameObject);
                                newGrabBox.UpdateOwnerHand(this);   
                                //thisHuman.SetAttackWaitAllMin(attackRecharge, thisHumanHand); 
                            }
                            else if(attackType == 0)
                            {
                                var newHB = Instantiate(attackHitBox, transform.position + hitboxloc, transform.rotation);
                                newHB.GetComponent<Hitbox>().UpdateDamage(damage);
                                newHB.GetComponent<Hitbox>().UpdateCharObj(this.gameObject);
                                int[] damageAreas = new int[] {5};
                            }
                            
                            /*  Punching code. Re-enable when you want this to punch instead of grapple
                            var newHB = Instantiate(attackHitBox, transform.position + hitboxloc, transform.rotation);
                            newHB.GetComponent<Hitbox>().UpdateDamage(damage);
                            newHB.GetComponent<Hitbox>().UpdateCharObj(this.gameObject);
                            int[] damageAreas = new int[] {5};
                            //Slash or overhead attack
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
                            newHB.GetComponent<Hitbox>().UpdateDamageArea(damageAreas);
                            */
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
            else
            {
                //CancelAttack();
                curAttackStage = AttackStage.noAttack;
                attackWait = 0.0f;
            }
            
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
    }
    public override void Attack()
    {
        faceDirection = 0;
        isAttacking = true;
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
        //Returns 2 since the attack is a punch.
        return 2;
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

    public void UpdateCauseGrap(HumanCauseGrappling newCauseGrap)
    {
        causeGrap = newCauseGrap;
        thisHuman.SetAttackWaitAllMin(attackRecharge, thisHumanHand); 
    }
}
