using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanIsGrappled : CharacterCanMoveBlock
{
    public HumanCauseGrappling causeGrapplerScr;      //The Script from the character grappling the owner of this script
    public Character getChar;
    public HumanCharacter getHuman;
    public GameObject grapplerCauser;
    public int grapplerCauserDir;   //Direction from this character to the grappler character
    public float moveWait = 0.0f;
    public bool enableBreakGrapple = true;

    public Vector3 aiLocationEndGoal;
    public int aiSubStage = 0;
    private bool disabled = false;
    // Start is called before the first frame update
    void Start()
    {
        getChar = GetComponent<Character>();
        getHuman = GetComponent<HumanCharacter>();
        if(getChar != null)
        {
            getChar.AddMoveBlockObjs(this as Component);
            //Debug.Log(this as Component);
        }
        else
        {
            Destroy(this);
        }
        grapplerCauser = causeGrapplerScr.gameObject;
        aiLocationEndGoal = grapplerCauser.transform.position;
        grapplerCauserDir = FaceTarget();
    }
    public void SetMoveWait(float newMW)
    {
        moveWait =newMW;
    }
    public void DisableBreakGrapple()
    {
        enableBreakGrapple = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(getHuman.GetFalling())
        {
            Destroy(this);
        }
        if(moveWait > 0.0f)
        {
            moveWait-= Time.deltaTime;
            if(moveWait <= 0.0f
            && enableBreakGrapple)
            {
                moveWait = 0;
                Destroy(this);
                //Stumbles both grappler and target if broken this way.
                
                getChar.Stumble(0.25f,0);
                grapplerCauser.GetComponent<Character>().Stumble(0.25f,0);

            }
        }


        aiLocationEndGoal = grapplerCauser.transform.position;
        if(aiSubStage == 0)
        {
            
                /*
            if(transform.position.z < aiLocationEndGoal.z + 1.0f)
            {
                Vector3 getEndLoc = new Vector3(aiLocationEndGoal.x + 1.0f*((grapplerCauserDir == 4) ? 1 : 0)  +  -1.0f*((grapplerCauserDir == 6) ? 1 : 0),aiLocationEndGoal.y +0.0f,aiLocationEndGoal.z + 0.0f  + 1.0f*((grapplerCauserDir == 2) ? 1 : 0) +  -1.0f*((grapplerCauserDir == 8) ? 1 : 0));
                Vector3 getRelEndLoc = getEndLoc - transform.position;
                getChar.ForceMoveXYZ(-1.0f, 0.0f, 0.0f);
                //Debug.Break();
            }
            else 
                */
            //Debug.Log(aiLocationEndGoal);
            if(disabled)
            {
                if(!getChar.IsStumbling())
                {
                    disabled = false;
                }
            }
            if(!IsNextToTarget()
            && !disabled)
            {
                
                //Gets the correct forcemove 
                Vector3 getEndLoc = new Vector3(aiLocationEndGoal.x + 1.0f*((grapplerCauserDir == 4) ? 1 : 0)  +  -1.0f*((grapplerCauserDir == 6) ? 1 : 0),aiLocationEndGoal.y +0.0f,aiLocationEndGoal.z + 0.0f  + 1.0f*((grapplerCauserDir == 2) ? 1 : 0) +  -1.0f*((grapplerCauserDir == 8) ? 1 : 0));
                Vector3 getRelEndLoc = getEndLoc - transform.position;
                if(getChar.FindWallOrChar(getEndLoc)
                && getChar.FindCharAdj(getEndLoc) != grapplerCauser
                && getChar.FindCharAdj(getEndLoc) != this.gameObject)
                {
                    //Debug.Log(getChar.FindWallOrChar(getEndLoc));
                    Destroy(this);
                    Debug.Log("This Destroy is active!");
                    //Debug.Break();
                }
                else
                {
                    if(getChar.FindCharAdj(getEndLoc) == this.gameObject)
                    {
                        disabled = true;
                    }
                    //Goes up or down slopes
                    //Debug.Log(getChar.FindWallOrChar(getEndLoc));

                    //Code for walking up "slopes" or blocks that are less than 0.5f of the transform.position.y value above the character
                    GameObject getWall = getChar.GetWall(transform.position + new Vector3(getRelEndLoc.x, 0.0f, getRelEndLoc.z));
                    getRelEndLoc.y = 0.0f;
                    if(getWall != null)
                    {
                        float relativeWallSize = transform.position.y - getWall.transform.position.y;
                        //Debug.Log(relativeWallSize);
                        Debug.Log(relativeWallSize);
                        getRelEndLoc.y = relativeWallSize;
                    }
                    

                    getChar.ForceMoveXYZNoWaitNoCol(getRelEndLoc.x, getRelEndLoc.y, getRelEndLoc.z);
                    //Debug.Break();
                }
                //ignore if shoved
                //That is, if the 'straight ahead distance' is greater than 1.
                //Finds distance x and z from target
                //Vector3 getEndLoc = new Vector3(aiLocationEndGoal.x + 1.0f*((grapplerCauserDir == 4) ? 1 : 0)  +  -1.0f*((grapplerCauserDir == 6) ? 1 : 0),aiLocationEndGoal.y +0.0f,aiLocationEndGoal.z + 0.0f  + 1.0f*((grapplerCauserDir == 2) ? 1 : 0) +  -1.0f*((grapplerCauserDir == 8) ? 1 : 0));
                /* 
                if(
                    (xDistTar > -1 && grapplerCauserDir == 4)
                    ||(xDistTar < 1 && grapplerCauserDir == 6)
                    ||(zDistTar < 1 && grapplerCauserDir == 2)
                    ||(zDistTar < 1 && grapplerCauserDir == 8)
                )
                {
                    
                }
                */
                /*
                if()
                {

                }
                */
                //Ignores if grappler causer moves towards target
                
                    /*
                if(
                )
                {
                    
                }
                else
                {
                        */
                    /*
                    //Gets the correct forcemove 
                    Vector3 getEndLoc = new Vector3(aiLocationEndGoal.x + 1.0f*((grapplerCauserDir == 4) ? 1 : 0)  +  -1.0f*((grapplerCauserDir == 6) ? 1 : 0),aiLocationEndGoal.y +0.0f,aiLocationEndGoal.z + 0.0f  + 1.0f*((grapplerCauserDir == 2) ? 1 : 0) +  -1.0f*((grapplerCauserDir == 8) ? 1 : 0));
                    Vector3 getRelEndLoc = getEndLoc - transform.position;
                    if(getChar.FindWallOrChar(getEndLoc)
                    && getChar.FindCharAdj(getEndLoc) != grapplerCauser
                    )//&& getChar.FindCharAdj(getEndLoc) != this.gameObject)
                    {
                        Debug.Log(getChar.FindWallOrChar(getEndLoc));
                        Destroy(this);
                        Debug.Log("This Destroy is active!");
                        //Debug.Break();
                    }
                    else
                    {
                        Debug.Log(getChar.FindWallOrChar(getEndLoc));
                        getChar.ForceMoveXYZ(getRelEndLoc.x, getRelEndLoc.y, getRelEndLoc.z);
                        //Debug.Break();
                    }
                    //aiSubStage = 1;
                    */
                //}
                
            }
        }
        if(aiSubStage == 1)
        {
            if(IsNextToTarget())
            {
                aiSubStage = 0;
            }
            else
            {

            }
        }
    }

    public void SetCauseGrapplerScr(HumanCauseGrappling getCG)
    {
        causeGrapplerScr = getCG;
    }
    
    bool IsNextToTarget()
    {
        //Finds distance x and z from target
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
        //txt.text = xDistTar.ToString() + "\n"+ zDistTar.ToString();
        //Checks if next to target
        if(
            ( (xDistTar <= 0) && (zDistTar <= 1) )
        ||  ( (zDistTar <= 0) && (xDistTar <= 1) )
        )
        {
            return true;
        }
        return false;
    }
    //Simple pathfinding just moves the enemy in a straight line towards their goal
    //returns the int direction of the enemy's next movement (i.e. 2,4,6,8 like numpad)
    int CalculateSimplePath()
    {
        //Finds distance x and z from target
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
        if(
            ( (xDistTar <= 0) && (zDistTar <= 1) )
        ||  ( (zDistTar <= 0) && (xDistTar <= 1) )
        )
        {
            //Debug.Log("Dont Calculate Path!");
            //Don't calculate path
            return -1;
        }
        int returnFD = FaceTarget();//-1;
        if((xDistTar <= 1) && (zDistTar <= 1))
        {
            if(Random.Range(0,2) == 1)
            {
                returnFD = (transform.position.x > aiLocationEndGoal.x) ? returnFD = 4 : returnFD = 6;
            }
            
        }
        return returnFD;
    }
    //Returns int to face target using angle
    int FaceTarget()
    {
        Vector2 firstVector = new Vector2(aiLocationEndGoal.x - transform.position.x,aiLocationEndGoal.z - transform.position.z);
        Vector2 secondVector = new Vector2(0.0f,1.0f);
        float getAngle = Vector2.SignedAngle(firstVector, secondVector);
        //returns int to face
        if(((getAngle > 135) && (getAngle <= 180))
        || (getAngle <= -135)
        )
        {
            return 2;
        }
        if((getAngle > -135)
        && (getAngle <= -45)
        )
        {
            return 4;
        }
        if((getAngle > -45)
        && (getAngle <= 45)
        )
        {
            return 8;
        }
        if((getAngle > 45)
        && (getAngle <= 135)
        )
        {
            return 6;
        }
        return -1;
    }
    
    //Returns int to face target based on position. Too finicky to use, so it's dummied out. 
    int FaceTargetPosition()
    {
        //Finds distance x and z from target
        int xDistTar = (int) Mathf.Abs(Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f)));
        int zDistTar = (int) Mathf.Abs(Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f)));

        //Debug.Log(Vector2.SignedAngle(new Vector2(transform.position.x, transform.position.z), new Vector2(aiLocationEndGoal.x,aiLocationEndGoal.z)));
        Debug.Log(Vector2.SignedAngle(new Vector2(aiLocationEndGoal.x,aiLocationEndGoal.z),new Vector2(transform.position.x, transform.position.z) ));
        //returns int to face
        if(xDistTar > zDistTar)
        {
            if(transform.position.x > aiLocationEndGoal.x)
            {
                return 4;
            }
            else 
            {
                return 6;
            }
        }
        else
        {
            if(transform.position.z > aiLocationEndGoal.z)
            {
                return 2;
            }
            else 
            {
                return 8;
            }
        }
        //return -1;
    }
    public int GetGrapplerCauserDir()
    {
        return grapplerCauserDir;
    }
}