using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimate : MonoBehaviour
{
    /*
    Attacks have the following frames:
        1/ Windup 1
        2/ windup 2
        3/ Hit
        4/ Recover
    */ 
    //Attack sprites assume there are 4 frames to the animation: 0 == windup1; 1 == windup2; 2 == attack; 3 == recover
    //Slash attack sprites
    public Sprite[] slashDown;
    public Sprite[] slashUp;
    public Sprite[] slashLeft;
    public Sprite[] slashRight;

    //Punch attack sprites
    public Sprite[] punchDown;
    public Sprite[] punchUp;
    public Sprite[] punchLeft;
    public Sprite[] punchRight;
    
    //Overhead attack sprites
    public Sprite[] overheadDown;
    public Sprite[] overheadUp;
    public Sprite[] overheadLeft;
    public Sprite[] overheadRight;
    
    private Sprite[] getAttackSprite;
    //Facing direction for animation
    public int faceDirection = 2;
    private int prevFaceDir;
    private bool strafe = false;
    private bool prevIsAttacking = false;
    //public bool movedWithoutLooking = false;  //For strafing before moving.
    
    public bool isAttacking = false;

    public Sprite[] walkDown;     //Assumes half the walking cycle is the left step, the other half the right step.
    public Sprite[] walkLeft;     
    public Sprite[] walkRight;     
    public Sprite[] walkUp;     

    //Hurt Sprites
    public Sprite hurtDown;     //Hurt sprites are only one frame.
    public Sprite hurtLeft;     
    public Sprite hurtRight;     
    public Sprite hurtUp;     

    //Shove Sprites
    public Sprite shoveDown;     //Hurt sprites are only one frame.
    public Sprite shoveLeft;     
    public Sprite shoveRight;     
    public Sprite shoveUp;  

    //Parry Sprites in each direction
    public bool isSwordBlock = false;
    public bool isSwordBlockHit = false;
    public Sprite swordBlockDown;
    public Sprite swordBlockLeft;     
    public Sprite swordBlockRight;     
    public Sprite swordBlockUp; 
    public Sprite swordBlockHitDown;
    public Sprite swordBlockHitLeft;     
    public Sprite swordBlockHitRight;     
    public Sprite swordBlockHitUp; 

    //Fallen over sprites
    public Sprite fallenOver;  
    public Sprite kneel;  
    public Sprite dead;  

    //Time in seconds each frame of walking animation is shown.  
    public float waitTime = 0.12f;
    public float curWait = 0.0f;
    public bool isWalking = false;
    
    private bool isHurt = false;
    private bool prevIsHurt = false;

    public enum WalkStep { left, right };
    private WalkStep curWalkStep = WalkStep.left;

    private Sprite[] animCycle; //Current Animation cycle
    public int curSpriteIndex;
    private SpriteRenderer getSR;
    private Vector3 spriteOffset = new Vector3(0,0,0); //offset for sprite.
    private Vector3 dodgeSpriteOffset = new Vector3(0,0,0); //offset for dodging.
    private Vector3 shoveSpriteOffset = new Vector3(0,0,0); //offset for shoving.
    private Vector3 bumpSpriteOffset = new Vector3(0,0,0); //offset for bumping into walls.

    public int attackType = 0;

    public bool isShoving = false;
    public float shoveWait = 0.0f;
    public float shoveTime = 0.2f;

    public bool isBumping = false;
    public float bumpWait = 0.0f;
    public float bumpTime = 0.2f;

    public bool isDead = false;

    
    //Checking if the character has fallen over
    public bool isFallenDown = false;
    public bool isKneeling = false;
    //public bool prevIsFallenDown = false;

    // Start is called before the first frame update
    void Start()
    {
        getSR = GetComponent<SpriteRenderer>();
        curWait = 0.0f;

        prevFaceDir = faceDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            getSR.sprite = dead;
        }
        //Prioritises fallen over sprite over ishurt 
        else if(isFallenDown)
        {
            if(isKneeling)
            {
                getSR.sprite = kneel;

            }
            else
            {
                getSR.sprite = fallenOver;
            }
        }
        //Hurt takes priority
        else if(isHurt)
        {
            prevIsHurt = true;
            //gets facedirection
            switch(faceDirection)
            {
                case 2:
                    getSR.sprite = hurtDown;
                    break;
                case 6:
                    getSR.sprite = hurtRight;
                    break;
                case 4:
                    getSR.sprite = hurtLeft;
                    break;
                case 8:
                    getSR.sprite = hurtUp;
                    break;
            }
        }
        //Defending
        else if(isSwordBlock)
        {
            //gets facedirection
            if(!isSwordBlockHit)
            {
                switch(faceDirection)
                {
                    case 2:
                        getSR.sprite = swordBlockDown;
                        break;
                    case 6:
                        getSR.sprite = swordBlockRight;
                        break;
                    case 4:
                        getSR.sprite = swordBlockLeft;
                        break;
                    case 8:
                        getSR.sprite = swordBlockUp;
                        break;
                }
            }
            else
            {
                switch(faceDirection)
                {
                    case 2:
                        getSR.sprite = swordBlockHitDown;
                        break;
                    case 6:
                        getSR.sprite = swordBlockHitRight;
                        break;
                    case 4:
                        getSR.sprite = swordBlockHitLeft;
                        break;
                    case 8:
                        getSR.sprite = swordBlockHitUp;
                        break;
                }
            }
        }
        //Attacks
        else if(isAttacking)
        {
            prevIsAttacking = true;
            getAttackSprite = null;
            switch (faceDirection)
            {
                case 2:
                    if(attackType == 0)
                    {
                        getAttackSprite = slashDown;
                    }
                    else if(attackType == 1)
                    {
                        getAttackSprite = overheadDown;
                    }
                    else if(attackType == 2)
                    {
                        getAttackSprite = punchDown;
                    }
                    break;
                case 4:
                    if(attackType == 0) 
                    {
                        getAttackSprite = slashLeft;
                    }
                    else if(attackType == 1)
                    {
                        getAttackSprite = overheadLeft;
                    }
                    else if(attackType == 2)
                    {
                        getAttackSprite = punchLeft;
                    }
                    break;
                case 6:
                    if(attackType == 0)
                    {
                        getAttackSprite = slashRight;
                    }
                    else if(attackType == 1)
                    {
                        getAttackSprite = overheadRight;
                    }
                    else if(attackType == 2)
                    {
                        getAttackSprite = punchRight;
                    }
                    break;
                case 8:
                    if(attackType == 0)
                    {
                        getAttackSprite = slashUp;
                    }
                    else if(attackType == 1)
                    {
                        getAttackSprite = overheadUp;
                    }
                    else if(attackType == 2)
                    {
                        getAttackSprite = punchUp;
                    }
                    break;
            }
            getSR.sprite = getAttackSprite[curSpriteIndex];
        }
        else if(isWalking)      //Prioritises attacking over walking
        {
            curWait -= Time.deltaTime;
            if(curWait < 0)
            {
                curSpriteIndex++;
                curWait = waitTime;
                if(curSpriteIndex < animCycle.Length)//-1)
                {
                    getSR.sprite = animCycle[curSpriteIndex];
                }
                else
                {
                    curSpriteIndex = 0;
                    getSR.sprite = animCycle[curSpriteIndex];
                    isWalking = false;
                }
            }
        }
        //Special isShove timer
        else if(isShoving)
        {
            //gets facedirection
            switch(faceDirection)
            {
                case 2:
                    getSR.sprite = shoveDown;
                    break;
                case 6:
                    getSR.sprite = shoveRight;
                    break;
                case 4:
                    getSR.sprite = shoveLeft;
                    break;
                case 8:
                    getSR.sprite = shoveUp;
                    break;
            }
            shoveWait += Time.deltaTime;
            if(shoveWait > shoveTime)
            {
                shoveWait = 0.0f;
                isShoving = false;
                shoveSpriteOffset = new Vector3(0.0f,0.0f,0.0f);
            }
        }
        else
        {
            //gets facedirection
            if(prevFaceDir != faceDirection)
            {
                prevFaceDir = faceDirection;
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
        if(!isAttacking
        && prevIsAttacking)
        {
            strafe = true;
            prevIsAttacking = false;
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
        if(!isHurt
        && prevIsHurt)
        {
            prevIsHurt = false;
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

        //Special isBump timer
        if(isBumping)
        {
            bumpWait += Time.deltaTime;
            if(bumpWait > bumpTime)
            {
                bumpWait = 0.0f;
                isBumping = false;
                bumpSpriteOffset = new Vector3(0.0f,0.0f,0.0f);
            }
        }
        spriteOffset = dodgeSpriteOffset + shoveSpriteOffset + bumpSpriteOffset;
    }
    public void Attack(int attackFrame)//, int getFaceDir)
    {
        if(attackFrame > -1)
        {
            isAttacking = true;
            curSpriteIndex = attackFrame;
        }
        else if((attackFrame <= -1) && (isAttacking))
        {
            isAttacking = false;
            curSpriteIndex = 0;
        }
    }
    public void Block(bool newSwordBlock)
    {
        isSwordBlock = newSwordBlock;
        if(isSwordBlock == true)
        {
            isSwordBlockHit = false;
        }
        
    }
    public bool GetBlock()
    {
        return isSwordBlock;
    }
    public void BlockHit(bool newSwordBlockHit)
    {
        isSwordBlockHit = newSwordBlockHit;
    }
    public void AttackType(int newAttackType)
    {
        //0 == Slash; 1 == Overhead; 2 == punch
        attackType = newAttackType;
    }
    public void Idle()
    {
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
    public void Walk(int walkInt)
    {
        if(!isAttacking)
        {
            //walkInt: integer for walking. 2 = down, 6 = right; 8 = up; 4 = left; Like numpad
            //Gets the step and stores the right frames in the anim cycle.
            Sprite[] getWalkSprite = null;
            if(strafe)
            {
                switch (faceDirection)
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
                strafe = false;
            }
            else
            {
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
    public void Defend(int walkInt)
    {
        switch (walkInt)
        {
            case 2:
                dodgeSpriteOffset = new Vector3(0.0f,0.0f,-0.6f);
                break;
            case 4:
                dodgeSpriteOffset = new Vector3(-0.6f,0.0f,0.0f);
                break;
            case 6:
                dodgeSpriteOffset = new Vector3(0.6f,0.0f,0.0f);
                break;
            case 8:
                dodgeSpriteOffset = new Vector3(0.0f,0.0f,0.6f);
                break;
            default:
                dodgeSpriteOffset = new Vector3(0.0f,0.0f,0.0f);
                break;
        }
    }
    //Shoving
    public void Shove(int walkInt)
    {
        isShoving = true;
        switch (walkInt)
        {
            case 2:
                shoveSpriteOffset = new Vector3(0.0f,0.0f,-0.6f);
                break;
            case 4:
                shoveSpriteOffset = new Vector3(-0.6f,0.0f,0.0f);
                break;
            case 6:
                shoveSpriteOffset = new Vector3(0.6f,0.0f,0.0f);
                break;
            case 8:
                shoveSpriteOffset = new Vector3(0.0f,0.0f,0.6f);
                break;
            default:
                shoveSpriteOffset = new Vector3(0.0f,0.0f,0.0f);
                break;
        }
    }
    //Bumping
    public void Bump(int walkInt)
    {
        isBumping = true;
        switch (walkInt)
        {
            case 2:
                bumpSpriteOffset = new Vector3(0.0f,0.0f,-0.25f);
                break;
            case 4:
                bumpSpriteOffset = new Vector3(-0.25f,0.0f,0.0f);
                break;
            case 6:
                bumpSpriteOffset = new Vector3(0.25f,0.0f,0.0f);
                break;
            case 8:
                bumpSpriteOffset = new Vector3(0.0f,0.0f,0.25f);
                break;
            default:
                bumpSpriteOffset = new Vector3(0.0f,0.0f,0.0f);
                break;
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
    public void SetHurt(bool newHurt)
    {
        isHurt = newHurt;
    }

    public Vector3 GetSpriteOffset()
    {
        return spriteOffset;
    }

    public void SetFallenDown(bool newFallenDown)
    {
        isFallenDown = newFallenDown;
    }

    public void SetIsKneeling(bool newIsKneeling)
    {
        isKneeling = newIsKneeling;
    }

    public void Die()
    {
        isDead = true;
    }
}
