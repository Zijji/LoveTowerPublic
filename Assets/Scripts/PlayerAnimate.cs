using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    /*
    Attacks have the following frames:
        1/ Windup 1
        2/ windup 2
        3/ Hit
        4/ Recover
    */
    public Sprite[] attackDown;     //Attacks downward
    public Sprite[] attackLeft;     
    public Sprite[] attackRight;     
    public Sprite[] attackUp;     
    
    private Sprite[] getAttackSprite;
    /*
    //Time for attack frames
    public float attackWindUp1Time = 0.2f;
    public float attackWindUp2Time = 0.4f;
    public float attackHitTime = 0.8f;
    public float attackRecoverTime = 1.0f;

    public enum AttackStage { noAttack, windup1, windup2, hit, recover };
    private AttackStage curAttackStage = AttackStage.noAttack;
    */
    //Facing direction for animation
    public int faceDirection = 2;

    
    public bool isAttacking = false;

    public Sprite[] walkDown;     //Assumes half the walking cycle is the left step, the other half the right step.
    public Sprite[] walkLeft;     
    public Sprite[] walkRight;     
    public Sprite[] walkUp;     

    //Time in seconds each frame of walking animation is shown.  
    public float waitTime = 0.2f;
    private float curWait = 0.0f;
    private bool isWalking = false;
    
    /*
    public enum WalkState { walkDown, walkUp, walkLeft, walkRight};
    private WalkState curWalkState = WalkState.walkDown;
    */
    public enum WalkStep { left, right };
    private WalkStep curWalkStep = WalkStep.left;

    private Sprite[] animCycle; //Current Animation cycle
    private int curSpriteIndex;
    private SpriteRenderer getSR;

    // Start is called before the first frame update
    void Start()
    {
        getSR = GetComponent<SpriteRenderer>();
        curWait = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Attacks
        if(isAttacking)
        {
            getAttackSprite = null;
            switch (faceDirection)
            {
                case 2:
                    getAttackSprite = attackDown;
                    break;
                case 4:
                    getAttackSprite = attackLeft;
                    break;
                case 6:
                    getAttackSprite = attackRight;
                    break;
                case 8:
                    getAttackSprite = attackUp;
                    break;
            }
            //Debug.Log(curSpriteIndex);
            getSR.sprite = getAttackSprite[curSpriteIndex];
            /*
            switch(curAttackStage)
            {
                case AttackStage.noAttack:
                    curAttackStage = AttackStage.windup1;
                    curWait = attackWindUp1Time;
                    curSpriteIndex = 0;
                    getSR.sprite = attackDown[curSpriteIndex];
                    break;
                case AttackStage.windup1:
                    curAttackStage = AttackStage.windup2;
                    curWait = attackWindUp2Time;
                    curSpriteIndex++;
                    getSR.sprite = attackDown[curSpriteIndex];
                    break;
                case AttackStage.windup2:
                    curAttackStage = AttackStage.hit;
                    curWait = attackHitTime;
                    curSpriteIndex++;
                    getSR.sprite = attackDown[curSpriteIndex];
                    break;
                case AttackStage.hit:
                    curAttackStage = AttackStage.recover;
                    curWait = attackRecoverTime;
                    curSpriteIndex++;
                    getSR.sprite = attackDown[curSpriteIndex];
                    break;
                case AttackStage.recover:
                    curAttackStage = AttackStage.noAttack;
                    curWait = 0.0f;
                    isAttacking = false;
                    break;
            }
            */
            //curWait -= Time.deltaTime;
            //if(curWait < 0)
            //{
            //    curSpriteIndex++;
            //    curWait = waitTime;
            //    if(curSpriteIndex > attackDown.Length-1)
            //    {
            //        curSpriteIndex = 0;
            //        isWalking = false;
            //    }
            //    getSR.sprite = attackDown[curSpriteIndex];
            //}
            
        }
        else if(isWalking)      //Prioritises attacking over walking
        {
            curWait -= Time.deltaTime;
            if(curWait < 0)
            {
                curSpriteIndex++;
                curWait = waitTime;
                if(curSpriteIndex > animCycle.Length-1)
                {
                    curSpriteIndex = 0;
                    isWalking = false;
                }
                getSR.sprite = animCycle[curSpriteIndex];
            }
        }
        else
        {
            
            //gets facedirection
            switch(faceDirection)
            {
                case 2:
                    getSR.sprite = walkDown[0];
                    break;
                case 6:
                    getSR.sprite = walkRight[0];
                    break;
                case 4:
                    getSR.sprite = walkLeft[0];
                    break;
                case 8:
                    getSR.sprite = walkUp[0];
                    break;

            }
        }
    }
    public void Attack(int attackFrame)//, int getFaceDir)
    {
        if(attackFrame > -1)
        {
            isAttacking = true;
            curSpriteIndex = attackFrame;
        }
        else if(attackFrame <= -1)
        {
            isAttacking = false;
            curSpriteIndex = 0;
        }
    }
    public void Walk(int walkInt)
    {
        if(!isAttacking)
        {
            //walkInt: integer for walking. 2 = down, 6 = right; 8 = up; 4 = left; Like numpad
            //Gets the step and stores the right frames in the anim cycle.
            Sprite[] getWalkSprite = null;
            switch (walkInt)
            {
                case 2:
                    getWalkSprite = walkDown;
                    break;
                case 4:
                    getWalkSprite = walkLeft;
                    break;
                case 6:
                    getWalkSprite = walkRight;
                    break;
                case 8:
                    getWalkSprite = walkUp;
                    break;
            }
            animCycle = new Sprite[getWalkSprite.Length/2];
            if( curWalkStep == WalkStep.left)
            {
                for(int i = 0; i < getWalkSprite.Length/2; i++)
                {
                    animCycle[i] = getWalkSprite[i];
                }
            }
            else if( curWalkStep == WalkStep.right)
            {
                for(int i = getWalkSprite.Length/2; i < getWalkSprite.Length; i++)
                {
                    animCycle[i - getWalkSprite.Length/2] = getWalkSprite[i];
                }
            }
            ChangeStep();
            //Starts the walk animation
            isWalking = true;
            curSpriteIndex = 0;
            getSR.sprite = animCycle[curSpriteIndex];
            curWait = waitTime;
        }
        
    }

    private void ChangeStep()
    {
        if( curWalkStep == WalkStep.left)
        {
            curWalkStep = WalkStep.right;
        }
        else if( curWalkStep == WalkStep.right)
        {
            curWalkStep = WalkStep.left;
        }
    }
    public void SetFaceDirection(int newFD)
    {
        faceDirection = newFD;
    }
}
