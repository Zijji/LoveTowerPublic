using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanCharacter))]
public class EnemyCharacterController : MonoBehaviour
{
    private HumanCharacter thisHuman;    //Controlling the human externally in this script
    private Character thisCharacter;    //Getting some functionality from character.
    public int moveDirection;         //MoveDirection; 2 == down; 6 == right; 8 == up; 4 == left; (like numpad) 

    //Variables for movement
    public int aiObjective = 0; //Current objective for Enemy. 0 == idle; 1 == move;
    public Vector3 aiLocationEndGoal;  //End goal for ai movement. That is, the end grid position the ai wants to head towards
    //public Vector3 aiLocationCurGoal;   //Current 'goal' for ai movement. i.e. the immediate next grid the ai wants to go to.
    
    //A* pathfinding stuff
    public List<Vector3> aiAsOpenList;      //Open list for A* pathfinding
    public List<Vector3> aiAsClosedList;      //Open list for A* pathfinding

    public int attackType = 0;       //Sets attack type: 0 == slash; 1 == overhead.

    

    public TextMesh txt;
    private string fighttxt;        //Text for whether the enemy is doing an overhead or a slash.

    //Ai track stages
    public int aiSubStage = 0;
    public float aiSubStageWait = 0;

    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        thisCharacter = GetComponent<Character>();
        moveDirection = -1 ;

        //CalculatePath();
    }

    // Update is called once per frame
    void Update()
    {
        if(thisHuman.GetMovedBack()
        && aiSubStage != 2)
        {
            aiSubStage = 2;
            aiSubStageWait = Random.Range(0.0f,0.4f);
        }
        //txt.text = fighttxt + "\n" + thisCharacter.GetHealthPoints().ToString("F0");
        txt.text = thisCharacter.GetHealthPoints().ToString("F0");

        aiLocationEndGoal = GameObject.Find("PlayerCharacter").transform.position;      //Replace this with better function that doesn't use GameObject.Find()
        //Moves until enemy is next to protag
        if(aiSubStage == 0)
        {
            if(!IsNextToTarget() && thisCharacter.CanMove() && !thisHuman.GetAttacking())
            {
                moveDirection = CalculateSimplePath();
                aiSubStage = 1;
            }
        }
        if(aiSubStage == 1)
        {
            if(moveDirection != -1)
            {
                if(thisCharacter.FindWall(transform.position + new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0))))
                {
                    thisHuman.SetMoving(true, moveDirection);
                    thisHuman.SetFaceDirection(moveDirection);
                }
            }
            if(!thisCharacter.CanMove()
            || thisHuman.GetIsHitStun())
            {
                aiSubStage = 0;
            }
        }
        if(aiSubStage == 2)
        {
            if(thisCharacter.CanMove())
            {
                aiSubStageWait -= Time.deltaTime;
                if(aiSubStageWait <= 0)
                {
                    aiSubStage = 0;
                }
            }
        }
        if(IsNextToTarget()  && thisCharacter.CanMove() && !thisHuman.GetAttacking())
        {
            //Attacks target
            thisHuman.SetFaceDirection(FaceTarget());//moveDirection);
            thisHuman.SetAttacking(true, 0);    //Attacks with left hand.
            //thisHuman.SetAttackType(attackType);
            fighttxt = attackType == 1 ? "Overhead\n" : "Slash\n";
            attackType = attackType == 1 ? 0 : 1;
        }

    }
    //Returns int to face target
    int FaceTarget()
    {
        //Finds distance x and z from target
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
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
    //Calculates A* path for pathfinding
    void CalculateAsPath()
    {
        /*

        //Add current position to closed list
        aiAsClosedList.Add(transform.position);
        //Adds adjacent positions to open list
        aiAsOpenList.Add(transform.position + new Vector3(0.0f + -1.0f*((checkPosition == 4) ? 1 : 0) +  1.0f*((checkPosition == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((checkPosition == 2) ? 1 : 0) +  1.0f*((checkPosition == 8) ? 1 : 0)));


        for(int checkPosition = 2; checkPosition > 8; checkPosition+=2)
        {
            if(
                (thisCharacter.FindWall(transform.position + new Vector3(0.0f + -1.0f*((checkPosition == 4) ? 1 : 0) +  1.0f*((checkPosition == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((checkPosition == 2) ? 1 : 0) +  1.0f*((checkPosition == 8) ? 1 : 0))))
            //&&  !(thisCharacter.FindWall(transform.position + new Vector3(0.0f + -1.0f*((checkPosition == 4) ? 1 : 0) +  1.0f*((checkPosition == 6) ? 1 : 0),0.0f,0.0f  + -1.0f*((checkPosition == 2) ? 1 : 0) +  1.0f*((checkPosition == 8) ? 1 : 0))))
            )
            {
            }
            aiAsOpenList.Add(transform.position + new Vector3(0.0f + -1.0f*((checkPosition == 4) ? 1 : 0) +  1.0f*((checkPosition == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((checkPosition == 2) ? 1 : 0) +  1.0f*((checkPosition == 8) ? 1 : 0)));

        }
            */
    }
    //Simple pathfinding just moves the enemy in a straight line towards their goal
    //returns the int direction of the enemy's next movement (i.e. 2,4,6,8 like numpad)
    int CalculateSimplePath()
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
            Debug.Log("Dont Calculate Path!");
            //Don't calculate path
            return -1;
        }
        int returnFD = FaceTarget();//-1;
        /*
        //returns int to move
        if(xDistTar > zDistTar)
        {
            if(transform.position.x > aiLocationEndGoal.x)
            {
                returnFD = 4;
            }
            else 
            {
                returnFD = 6;
            }
        }
        else
        {
            if(transform.position.z > aiLocationEndGoal.z)
            {
                returnFD = 2;
            }
            else 
            {
                returnFD = 8;
            }
        }
        */
        //Special case: if at a diagonal, randomly chooses which square to goto
        if((xDistTar <= 1) && (zDistTar <= 1))
        {
            if(Random.Range(0,2) == 1)
            {
                returnFD = (transform.position.x > aiLocationEndGoal.x) ? returnFD = 4 : returnFD = 6;
                /*
                    if(transform.position.x > aiLocationEndGoal.x)
                    {
                    }

                */
            }
            
        }
        //txt.text = xDistTar.ToString() + "\n"+ zDistTar.ToString();
        //txt.text = xDistTar.ToString("#.0000") + "\n"+ zDistTar.ToString("#.0000");
        return returnFD;
        //Vector2.SignedAngle(, );

        /*
        //1 find angle from goal to self
        Debug.Log(Vector2.SignedAngle(new Vector2(transform.position.x, transform.position.z), new Vector2(aiLocationEndGoal.x, aiLocationEndGoal.z)));
        
        txt.text = Vector2.SignedAngle(new Vector2(transform.position.x, transform.position.z), new Vector2(aiLocationEndGoal.x, aiLocationEndGoal.z)).ToString("#.0000");
        return 0;
        */
    }
    //returns true if next to target, false otherwise.
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
}
