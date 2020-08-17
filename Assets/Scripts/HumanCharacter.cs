using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
//Generic version of PlayerCharacter that can be applied to all humans. Split so that all humans (enemies and players) will update the same behaviours (e.g. grappling, dual wielding, etc.)
[RequireComponent(typeof(Character))]
public class HumanCharacter : MonoBehaviour
{
    Character thisChar;
    //Facing direction:
    public int faceDirection = 2;   //2 == down; 6 == right; 8 == up; 4 == left; (like numpad)
    //Slight delay before moving
    public float moveDelay = 0.15f;
    public float moveDelayWait = 0.0f;
    public bool isMoveDelay = false;    //Currently under move delay?
    public int defDirec = 2;            //Movement of defence, like faceDirection i.e. 2 == down; 6 == right; 8 == up; 4 == left; 

    public bool isMoving = false;
    public bool isMovingInput = false;  //Set to true when input for isMoving is received, false otherwise.
    //Variables for shoving
    public bool isShoving = false;
    public GameObject shoveHitBox;  //Variant of hitbox which is used to shove.
    public GameObject tripHitBox;  //Variant of hitbox which is used to trip.
    public int moveDirection = 2;
    //Is defending?
    public bool isDefending = false;
    public bool getBlockHit = false;

    public GameObject spriteGameObj;
    public Vector3 spriteGameObjOffset;  //Offset of sprite 
    public Vector3 spriteGameObjThisPrev;  //prev position of this object
    public float spriteMoveMag = 0.08f;
    //public float spriteMoveSpeed;     //Dummied out constant move speed - like in rpg maker games. 
    public Vector3 prevPos;
    public Vector3 oldPosValue;
    public bool movedBack;

    private HumanAnimate hSAnim;   //HumanSprite anim script

    //Hitstun variables
    public float preHealthPoints;
    public float hitstun = 0.5f;
    private float hitstunWait = 0.0f;
    private bool isHitStun;
    private bool preHitStun;

    //Hands
    public CharacterHoldable[] charHands = null;
    public int attackingHand = 0;
    //Is attacking?
    public bool[] isAttacking;
    public float[] attackWait;

    private bool checkCharHere = true;

    //Is fallen down?
    private bool fallenDown = false;
    private bool prevFallenDown = false;
    private bool kneeling = false;
    //private float kneelWaitTime = false;
    public bool isFalling = false;
    public bool hasFallingStomped = false;

    void OnDestroy()
    {
        hSAnim.Die();
    }
    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        spriteGameObjThisPrev = transform.position;
        oldPosValue = transform.position;
        thisChar = GetComponent<Character>();
        //Checks if HumanSprite has a HumanAnimate script
        hSAnim = spriteGameObj.GetComponent<HumanAnimate>();    //Should NOT be null!
        spriteGameObjOffset = hSAnim.transform.position - transform.position;
        //previous HP for hitstun
        preHealthPoints = thisChar.GetHealthPoints();
        isHitStun = false;
        preHitStun = false;

        //Sets up isAttacking and attackWait variables
        isAttacking = new bool[charHands.Length];
        attackWait = new float[charHands.Length];
        for(int i = 0; i < charHands.Length; i++)
        {
            isAttacking[i] = false;
            attackWait[i] = -1.0f;
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        //Forces move if there's nothing below the character.
        var getCharFloor = thisChar.FindCharAdj(transform.position + new Vector3(0.0f,-1.0f,0.0f));
        if((getCharFloor != null)
        && (!hasFallingStomped)
        )
        {
            Debug.Log("Charfound");
            /*
            var newHB = Instantiate(shoveHitBox, transform.position + new Vector3(0.0f,-1.0f,0.0f), transform.rotation);
            newHB.GetComponent<HitboxShove>().UpdateDamage(0);
            newHB.GetComponent<HitboxShove>().UpdateCharObj(null);//this.gameObject);
            newHB.GetComponent<HitboxShove>().UpdateMoveDirection(moveDirection);//causeGrap.GetIsGrappledScr().GetGrapplerCauserDir());//thisHuman.GetMoveDirection());
            newHB.GetComponent<HitboxShove>().UpdateMoveMagnitude(1);
            */
            
            var newHB = Instantiate(tripHitBox, transform.position + new Vector3(0.0f,-1.0f,0.0f), transform.rotation);
            newHB.GetComponent<HitboxTrip>().UpdateDamage(0);
            newHB.GetComponent<HitboxTrip>().UpdateCharObj(null);//this.gameObject);
            newHB.GetComponent<HitboxTrip>().UpdateTripMagnitude(1.0f);
            //
            hasFallingStomped = true;
        }
        GameObject getFloor = thisChar.GetWall(transform.position + new Vector3(0.0f,-1.0f,0.0f));
        if(getFloor == null)
        {
            if(!isFalling)
            {
                isFalling = true;
                //Resets movewait
                thisChar.ResetMoveWait();
                thisChar.Stumble(0.1f,1);
            }
            if(isFalling)
            {
                if(thisChar.MoveWaitIsZero())
                {
                    thisChar.ForceMoveXYZNoWaitNoCol(0.0f,-1.0f,0.0f);
                    thisChar.Stumble(0.1f,1);
                }
            }
        }
        else
        {
            float relativeFloor = getFloor.transform.position.y  - transform.position.y + 1.0f;
            if(isFalling)
            {
                //Debug.Log(relativeFloor);
                if(relativeFloor < 0.0f)
                {
                    if(thisChar.MoveWaitIsZero())
                    {
                        thisChar.ForceMoveXYZNoWaitNoCol(0.0f,relativeFloor,0.0f);
                        thisChar.StumbleAdditive(0.5f,1);
                        isFalling = false;
                    }
                }
                else
                {
                    isFalling = false;
                    thisChar.StumbleAdditive(0.5f,1);
                }
            }
            if(relativeFloor < 0.0f)
            {
                thisChar.ForceMoveXYZNoWaitNoCol(0.0f,relativeFloor,0.0f);
            }
            
        }
        if((!isFalling)
        && (hasFallingStomped)
        )
        {
            hasFallingStomped = false;
        }
        //Proned animations
        if(fallenDown)
        {
            prevFallenDown = true;
            if ((thisChar.GetMoveWait() <= 0.3f )
            && (thisChar.FindCharAdj(transform.position) == null)
            )
            {
                kneeling = true;
            }
            else
            {
                kneeling = false;
            }
        }
        else
        {
            kneeling = false;
        }
        if((prevFallenDown == true)
        && (fallenDown == false)
        )
        {
            hSAnim.Idle();
            prevFallenDown = false;
        }
        hSAnim.SetFallenDown(fallenDown);
        hSAnim.SetIsKneeling(kneeling);
        //Resets movedBack - the variable that is only true when the character has been moved back.
        if(movedBack
        && thisChar.CanMove())
        {
            movedBack = false;
        }
        //Hitstun
        if(preHealthPoints != thisChar.GetHealthPoints())
        {
            preHitStun = true;
            preHealthPoints = thisChar.GetHealthPoints();
        }
        if(preHitStun)
        {
            isHitStun = true;
            hSAnim.SetHurt(true);
            hSAnim.Attack(-1);
            preHitStun = false;
        }
        if(isHitStun)
        {
            //isAttacking = false;
            //Resets attack
            for(int i = 0; i < charHands.Length; i++)
            {
                charHands[i].CancelAttack();
            }
            hitstunWait += Time.deltaTime;
            if(hitstunWait > hitstun)
            {
                hitstunWait = 0.0f;
                isHitStun = false;
                hSAnim.SetHurt(false);
            }
        }
        //Finds an attack
        bool foundOneAttack = false;
        for(int i = 0; i < charHands.Length; i++)
        {
            /*
            if(isAttacking[i]
            && !AttackFound()
            && attackWait[i] < 0.0f)
            {
                charHands[i].Attack();
                charHands[i].AttackSetPressed(true);
            }
            */
            if(!AttackFound())
            {    
                if(isAttacking[i]
                && attackWait[i] <= 0.0f)
                {
                    charHands[i].Attack();
                    charHands[i].AttackSetPressed(true);
                }
                if(attackWait[i] > 0.0f)
                {
                    attackWait[i] -= Time.deltaTime;
                }
            
            }
            if(charHands[i].GetAttack())//isAttacking)
            {
                foundOneAttack = true;
                if(!isAttacking[i])
                {
                    charHands[i].AttackSetPressed(false);
                }
                if(attackWait[i] <= 0.0f)
                {
                    charHands[i].Attack();
                    int attackStageInt = charHands[i].GetAttackStage() - 1;
                    hSAnim.Attack(attackStageInt);
                    hSAnim.AttackType(charHands[i].GetAttackType());
                }
                else
                {
                    attackWait[i] -= Time.deltaTime;
                }
            }
            //Cancels attack if not attacking
            if(!foundOneAttack)
            {
                hSAnim.Attack(-1);
            }
        }
        //isDefending is more accurately named isDodging
        //as it only concerns itself with dodge defences now.
        if(isDefending)
        {
            if(moveDelayWait < moveDelay)
            {
                moveDelayWait+= Time.deltaTime;
                //sets defend direction
                if(moveDirection != -1)
                {
                    defDirec = moveDirection;
                }
                if(isMoving)
                {
                    thisChar.Defend(true, defDirec, 2); //thisChar.Defend(true && !isAttacking, defDirec, 2);  //Held defence
                }
                else
                {
                    thisChar.Defend(true, defDirec, 1); //thisChar.Defend(true && !isAttacking, defDirec, 1);  //TapDefence
                }
                hSAnim.Defend(defDirec);
            }
            if(moveDelayWait >= moveDelay
            || AttackFound()
            )
            {
                isMoveDelay = false;
                moveDelayWait = 0.0f;
                thisChar.Stumble(0.5f);
                isDefending = false;
                hSAnim.Defend(-1);
                thisChar.Defend(false,-1,-1);
            }
        }
        if(thisChar.CanMove())
        {
            if(isShoving)
            {
                //Creates box here
                Vector3 hitboxloc = new Vector3(0.0f,0.0f,0.0f);
                switch(moveDirection)
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
                var newHB = Instantiate(shoveHitBox, transform.position + hitboxloc, transform.rotation);
                newHB.GetComponent<HitboxShove>().UpdateDamage(0);
                newHB.GetComponent<HitboxShove>().UpdateCharObj(this.gameObject);
                newHB.GetComponent<HitboxShove>().UpdateMoveDirection(moveDirection);
                newHB.GetComponent<HitboxShove>().UpdateMoveMagnitude(1);

                hSAnim.Shove(moveDirection);   //Reuse move animation for shoving.
                
                thisChar.Stumble(1.5f);
                isShoving = false;
                
                //May need to be reused to disable animation everywhere.
                for(int i = 0; i < charHands.Length; i++)
                {
                    if(isAttacking[i])
                    {
                        isAttacking[i] = false;
                        hSAnim.Attack(-1);
                    }
                }

            }
            if(isMoving)
            {
                if(thisChar.FindWallOrChar(transform.position + new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0))))
                {
                    //Code for walking up "slopes" or blocks that are less than 0.5f of the transform.position.y value above the character
                    GameObject getWall = thisChar.GetWall(transform.position + new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0)));

                    if(getWall != null)
                    {
                        float relativeWallSize = transform.position.y - getWall.transform.position.y;
                        //Debug.Log(relativeWallSize);
                        if(relativeWallSize > 0.4f)
                        {
                            //Debug.Log(getWall.transform.position);
                            //Debug.Log(transform.position);
                            thisChar.MoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),relativeWallSize,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0));
                            hSAnim.Walk(moveDirection);
                            //spriteGameObj.transform.position += -new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),relativeWallSize,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0));
                            
                        }
                        else
                        {
                            hSAnim.Bump(moveDirection); //Bumping into a wall
                            thisChar.Stumble(1.0f);
                        }
                    }
                }
                else
                {
                    //Code for walking down "slopes" or blocks that are less than 0.5f of the transform.position.y value below the character
                    GameObject getWall = thisChar.GetWall(transform.position + new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0)));

                    if(getWall != null)
                    {
                        //Debug.Log(getWall.transform.position);
                        //Debug.Log(transform.position);
                        float relativeWallSize = getWall.transform.position.y - transform.position.y + 1.0f;
                        //Debug.Log(relativeWallSize);
                        thisChar.MoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),relativeWallSize,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0));
                        hSAnim.Walk(moveDirection);
                    }
                    else
                    {
                        //hSAnim.Bump(moveDirection); //Bumping into a wall
                        //thisChar.Stumble(1.0f);
                        thisChar.MoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0));
                        hSAnim.Walk(moveDirection);
                    }
                    //spriteGameObj.transform.position += -new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0));
                }
            }
        
        }
        
        //Moves back if bumping into enemy. This only occurs when two characters move into the same place at the same time.
        if(oldPosValue != transform.position)
        {
            prevPos = oldPosValue;
            oldPosValue = transform.position;
        }
        //Code checks if there is already character at that position and moves back if there is
        if((checkCharHere)
        && (thisChar.GetCollision())
        )
        {
            GameObject[] getCharsHere = thisChar.CheckEmpty();
            if(getCharsHere != null)
            {
                transform.position = prevPos;
                getCharsHere = null;
                movedBack = true;
                //Debug.Log("charsHere");
                /*
                for(int i = 0; i < getCharsHere.Length; i++)
                {
                    Debug.Log(getCharsHere[i]);
                }
                */
                //Debug.Log(transform.position);
                //Debug.Break();
            }
        }
        //Moves the sprite object.
        /*
        if(spriteGameObjThisPrev != transform.position)
        {
            spriteGameObj.transform.position += spriteGameObjThisPrev - transform.position;
            spriteGameObjThisPrev = transform.position;
        }
        */
        spriteGameObj.transform.position = Vector3.MoveTowards(spriteGameObj.transform.position, this.transform.position + spriteGameObjOffset + hSAnim.GetSpriteOffset(), (float) Vector3.Distance(spriteGameObj.transform.position,this.transform.position + spriteGameObjOffset + hSAnim.GetSpriteOffset())*spriteMoveMag);
        //Dummied out constant movement - like in RPG Maker games: spriteGameObj.transform.position = Vector3.MoveTowards(spriteGameObj.transform.position, this.transform.position + spriteGameObjOffset + hSAnim.GetSpriteOffset(), spriteMoveSpeed);

        hSAnim.SetFaceDirection(faceDirection);
        isMoving = false;
        //moveDirection = -1;
        if(thisChar.CanMove())
        {
            isMovingInput = false;
        }

        //Animates blocks
        if(BlockFound())
        {
            hSAnim.Block(true); //Right now just assumes every block is a sword block.
            if(BlockHitFound())
            {
                getBlockHit = true;
            }
            hSAnim.BlockHit(getBlockHit);
        }
        else
        {
            if(hSAnim.GetBlock())
            {
                hSAnim.Block(false); //Right now just assumes every block is a sword block.
                getBlockHit = false;
                hSAnim.BlockHit(false);
                hSAnim.Idle();
            }
            
        }

    }
    //If input is currently being fed into isMoving
    public bool GetIsMovingInput()
    {
        return isMovingInput;
    }
    public void SetCheckCharHere(bool newCheckCharHere)
    {
        checkCharHere = newCheckCharHere;
        //Debug.Log(checkCharHere);
    }
    public int GetFaceDirection()
    {
        return faceDirection;
    }

    public void SetFaceDirection(int newFD)
    {
        //Only accepts valid numbers
        //2 == down; 6 == right; 8 == up; 4 == left; (like numpad)
        if((newFD == 2)
        || (newFD == 4)
        || (newFD == 6)
        || (newFD == 8)
        )
        {
            if((!GetAttacking())
            &&(!BlockFound())
            )
            {
                faceDirection = newFD;
            }
            
        }
        else
        {
            Debug.Log("Invalid faceDirection number detected!");
        }
    }

    public void SetAttacking(bool newAttacking, int attIndex)
    {
        if(!BlockFound())
        {
            isAttacking[attIndex] = newAttacking;
        }
        
        
    }

    public void SetBlocking(bool newBlocking, int attIndex)
    {
        if(!AttackFound()
        && (attackWait[attIndex] <= 0.0f)
        )
        {
            charHands[attIndex].Block();
        }
    }
    //Returns true if one of the hands is blocking, false otherwise.
    public bool BlockFound()
    {
        for(int i = 0; i < charHands.Length; i++)
        {
            if(charHands[i].GetBlock())
            {
                return true;
            }
        }
        return false;
    }
    //Returns true if one of the hands is blocking, false otherwise.
    public bool BlockHitFound()
    {
        for(int i = 0; i < charHands.Length; i++)
        {
            if(charHands[i].HasBlocked())
            {
                return true;
            }
        }
        return false;
    }
    /*
    //Put this here to enumerate all the places you need to cancel the attack.
    public void CancelAttack()
    {
        curAttackStage = AttackStage.noAttack;
        attackWait = 0.0f;
        isAttacking = false;
        feintCount = 0; //restores feint count.
        hSAnim.Attack(-1);
    }
    */
    //0 == slash, 1 == overhead
    /*
    public void SetAttackType(int newAttackType)
    {
        //Can only set the attack type during the no attack or windup phases
        if((curAttackStage == AttackStage.noAttack)
        || (curAttackStage == AttackStage.windup1)
        )
        {
            attackType = newAttackType;
            
            if(attackType == 0)
            {
                Debug.Log("Slash");
            }
            if(attackType == 1)
            {
                Debug.Log("Overhead");
            }
        }
        
    }
    */
    //Overloaded GetAttacking. If Any attack is found, returns true.
    public bool GetAttacking()
    {
        for(int i = 0; i < charHands.Length; i++)
        {
            if((isAttacking[i] == true)
            || (charHands[i].GetAttack())
            )
            {
                return true;
            }
        }
        return false;
    }

    public bool GetAttacking(int getIndex)
    {
        return isAttacking[getIndex];
    }

    public void SetMoving(bool newIsMoving, int newMoveDirection)
    {
        isMovingInput = true;
        //Sets the move direction before checking can move, meaning other movedirections are possible even if movement isn't.
        if((newMoveDirection == 2)
        || (newMoveDirection == 4)
        || (newMoveDirection == 6)
        || (newMoveDirection == 8)
        )
        {
            moveDirection = newMoveDirection;
        }
        if(thisChar.CanMove())
        {
            isMoving = newIsMoving;
            /*
            */
        }
    }
    public int GetMoveDirection()
    {
        return moveDirection;
    }

    public void SetShove(bool newIsShoving, int newShoveDirection)
    {
        if(thisChar.CanMove())
        {
            //Re-enable this code to re-enable shoving
            /*
            isShoving = newIsShoving;
            if((newShoveDirection == 2)
            || (newShoveDirection == 4)
            || (newShoveDirection == 6)
            || (newShoveDirection == 8)
            )
            {
                moveDirection = newShoveDirection;
            }
            */
        }

    }

    //If pushed from a shove
    public void SetShovePushed(int shoveDirection, int newFaceDirection)
    {
        if((newFaceDirection == 2)
        || (newFaceDirection == 4)
        || (newFaceDirection == 6)
        || (newFaceDirection == 8)
        )
        {
            faceDirection = newFaceDirection;
        }
        hSAnim.Bump(shoveDirection);
    }

    public void SetDefending(bool newIsDefending, int newDefDirection)
    {
        if(thisChar.CanMove()
        && !BlockFound())
        {
            isDefending = newIsDefending;
            if((newDefDirection == 2)
            || (newDefDirection == 4)
            || (newDefDirection == 6)
            || (newDefDirection == 8)
            )
            {
                moveDirection = newDefDirection;
            }
        }
        else
        {   //Cannot defend if you cannot move
            isDefending = false;
        }
        
    }

    public bool GetIsHitStun()
    {
        return isHitStun;
    }

    public void SetIsHitStun(bool newIsHitStun)
    {
        preHitStun = newIsHitStun;
    }

    public bool GetMovedBack()
    {
        return movedBack;
    }

    public bool AttackFound()
    {
        //Finds an attack
        //bool foundOneAttack = false;
        for(int i = 0; i < charHands.Length; i++)
        {
            if(charHands[i].GetAttack())//isAttacking)
            {
                return true;    //Returns true if at least one attack found.
            }
        }
        return false; //False otherwise
    }
    //returns index of weapon in character's hand, -1 otherwise.
    public int FindWeaponInHand(CharacterHoldable getScr)
    {
        for(int i = 0; i < charHands.Length; i++)
        {
            if(charHands[i] == getScr)
            {
                return i;
            }
        }

        return -1;       
    }
    //Sets attackWait[i]
    public void SetAttackWait(float newAttackWait, int attackWaitIndex)
    {
        attackWait[attackWaitIndex] = newAttackWait;     
    }
    //Sets attackWait[i] for all if attackWait isn't 0.0f but less than new attackwait
    public void SetAttackWaitAllMin(float newAttackWait, int attackWaitIndex)
    {
        attackWait[attackWaitIndex] = newAttackWait;    
        for(int i = 0; i < charHands.Length; i++)
        {
            if(attackWait[i] < newAttackWait
            && attackWait[i] > 0.0f)
            {
                attackWait[i] = newAttackWait;
            }
        } 
    }
    public float GetAttackWait(int attackWaitIndex)
    {
        return attackWait[attackWaitIndex];     
    }

    //Setters and getters for fallenDown
    public void SetFallenDown(bool newFallenDown)
    {
        fallenDown = newFallenDown;
    }
    public bool GetFallenDown()
    {
        return fallenDown;
    }
    public bool GetFalling()
    {
        return isFalling;
    }
}
