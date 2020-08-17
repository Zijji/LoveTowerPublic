using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanCharacter))]
[RequireComponent(typeof(AStarPathfinding))]
public class BasicEnemyController : MonoBehaviour
{
    private HumanCharacter thisHuman;    //Controlling the human externally in this script
    private Character thisCharacter;    //Getting some functionality from character.
    private AStarPathfinding thisAStarPathfinder;    //Getting some functionality from AStarPathfinder.

    public int moveDirection;         //MoveDirection; 2 == down; 6 == right; 8 == up; 4 == left; (like numpad) 

    //Variables for movement
    public int aiObjective = 0; //Current objective for Enemy. 0 == idle; 1 == move;
    public Vector3 aiLocationEndGoal;  //End goal for ai movement. That is, the end grid position the ai wants to head towards
    public Vector3 aiLocationEndGoalPrev;

    //Ai track stages
    public int aiSubStage = 0;
    public float aiSubStageWait = 0;
    public float reactionTime = 0.5f;
    //public List<Vector3> AStarPath = null;     //Last stored AStar Path
    //public bool onAstarPath = false;


    public TextMesh thisMesh;
    //Gets player character game object
    public GameObject getPlayer;

    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        thisCharacter = GetComponent<Character>();
        thisAStarPathfinder = GetComponent<AStarPathfinding>();
        moveDirection = -1 ;
        getPlayer = GameObject.Find("PlayerCharacter"); //Replace with more suitable function?
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

        if(getPlayer != null)
        {
            aiLocationEndGoal = getPlayer.transform.position;      //Replace this with better function that doesn't use GameObject.Find()
        }
        else
        {
            aiLocationEndGoal = transform.position;
            aiSubStage = 4;
        }

        if(aiSubStage == 4)
        {
            thisHuman.SetAttacking(false , 0);

        }
        if(aiSubStage == 3)
        {
            thisHuman.SetAttacking(false , 0);
            if(!thisHuman.AttackFound())
            {
                aiSubStage = 0;
            }
        }
        if(aiSubStage == 0)
        {
            if(!IsNextToTarget())
            {
                if(thisCharacter.CanMove() && !thisHuman.GetAttacking())
                {
                    moveDirection = -1;
                    if(!getPlayer.GetComponent<HumanCharacter>().GetFalling()
                    && (aiLocationEndGoalPrev != aiLocationEndGoal))
                    {
                        if(thisAStarPathfinder.LocationReachable(aiLocationEndGoal) != false)
                        {
                            moveDirection = thisAStarPathfinder.AStarFindNextMove(transform.position, aiLocationEndGoal);//CalculateSimplePath();//
                            if(moveDirection == -1)
                            {
                                //Looks for route with no characters, and continues on that.
                                moveDirection = thisAStarPathfinder.AStarFindNextMove(transform.position, aiLocationEndGoal,false);
                            }
                        }
                        if(moveDirection == -1)
                        {
                            //If all else fails, just uses heuristic which in this case is just distance.
                            moveDirection = thisAStarPathfinder.AStarHeuristicNextStep(aiLocationEndGoal);
                        }
                        
                    }
                        /*
                    if(moveDirection == -1) //If can't find 
                    {
                        AStarPath = thisAStarPathfinder.GetStoredPath();
                        if(AStarPath.Count > 0)
                        {
                            //Follows the path sequence.
                            aiLocationEndGoal = AStarPath[0];
                            //Searches path sequence to see if it's "on path".
                            if(transform.position == aiLocationEndGoal)
                            {
                                //Removes node if on path.
                                AStarPath.RemoveAt(0);
                                if(AStarPath.Count > 0)
                                {
                                    aiLocationEndGoal = AStarPath[0];
                                }
                            }
                            moveDirection = CalculateSimplePathIgnoreClose();
                            aiSubStage = 1; 
                        }
                        else
                        {
                            //Put in "dumb pathfinding" here.
                            aiSubStage = 2;
                            aiSubStageWait = reactionTime;
                        }
                    }
                        */
                    //If it fails to find a route above, idles
                    if(moveDirection == -1)
                    {
                        aiLocationEndGoalPrev = aiLocationEndGoal;  //Stores the players location. If it changes, recalculates then.
                        aiSubStage = 2;
                        aiSubStageWait = reactionTime;
                    }
                    else
                    {
                        aiLocationEndGoalPrev = transform.position; //Has to be set for comparison.
                        aiSubStage = 1; 
                    }
                }
                
            }
            else if(IsNextToTarget())
            {
                if(!thisHuman.AttackFound()
                && thisCharacter.CanMove())
                {
                    thisHuman.SetFaceDirection(FaceTarget());//moveDirection);
                    thisHuman.SetAttacking(true, 0);    //Attacks with left hand.
                    aiSubStage = 3;
                }
            }
        }
        if(aiSubStage == 1)
        {
            if(moveDirection != -1)
            {
                //if(thisCharacter.FindWall(transform.position + new Vector3(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0) +  1.0f*((moveDirection == 6) ? 1 : 0),-1.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0) +  1.0f*((moveDirection == 8) ? 1 : 0))))
                //{
                    thisHuman.SetMoving(true, moveDirection);
                    thisHuman.SetFaceDirection(moveDirection);
                //}
            }
            if(!thisCharacter.CanMove()
            || thisHuman.GetIsHitStun()
            || moveDirection == -1)
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
        //Debug.Log("Aisubstage"+aiSubStage.ToString());
        /*
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
            if(!thisHuman.AttackFound())
            {
                thisHuman.SetFaceDirection(FaceTarget());//moveDirection);
                thisHuman.SetAttacking(true, 0);    //Attacks with left hand.
            }
            //thisHuman.SetAttackType(attackType);
            //attackType = attackType == 1 ? 0 : 1;
        }
        */
        
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
    //Simple pathfinding just moves the enemy in a straight line towards their goal
    //returns the int direction of the enemy's next movement (i.e. 2,4,6,8 like numpad)
    //Ignores too close to target.
    int CalculateSimplePathIgnoreClose()
    {
        int returnFD = FaceTarget();
        //Special case: if at a diagonal, randomly chooses which square to goto
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
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
        return returnFD;
    }
    //Simple pathfinding just moves the enemy in a straight line towards their goal
    //returns the int direction of the enemy's next movement (i.e. 2,4,6,8 like numpad)
    int CalculateSimplePath()
    {
        /*
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
        */
        if(IsNextToTarget())
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
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
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
        //if(transform.position.x - )
        /*
        //Finds distance x and z from target
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));
        thisMesh.text = xDistTar.ToString()+"\n"+zDistTar.ToString();
        //Debug.Log("xDist:"+xDistTar.ToString());
        //Debug.Log("zDist:"+zDistTar.ToString());
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
        */
        //Finds distance x and z from target
        int xDistTar = (int) Vector2.Distance(new Vector2(transform.position.x, 0.0f),new Vector2(aiLocationEndGoal.x, 0.0f));
        int yDistTar = (int) Vector2.Distance(new Vector2(transform.position.y, 0.0f),new Vector2(aiLocationEndGoal.y, 0.0f));
        int zDistTar = (int) Vector2.Distance(new Vector2(transform.position.z, 0.0f),new Vector2(aiLocationEndGoal.z, 0.0f));

        if(
            (
                ( (xDistTar <= 0) && (zDistTar <= 1) )
                ||  ( (zDistTar <= 0) && (xDistTar <= 1) )
            )
            &&(yDistTar <= 0.6f)
        )
        {
            return true;
        }
        return false;
    }

    //Uses A* pathfinding to find the next step for the enemy.
    int AStarFindNextMove()
    {
        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();
        //Adds a new node to open list to start.
        AStarNode newStarNode = new AStarNode(transform.position, null, 0, 0, 0, 0);
        openList.Add(newStarNode);
        //Debug.Log("OpenList Count:"+openList.Count.ToString());
        bool search = true;
        int searchi = 0;
        while(search)
        {
            //Debug.Log("Searchi:"+searchi.ToString());
            //Debug.Log("OpenList Count:"+openList.Count.ToString());
            //Retrieves node with lowest F - defaults to top of list.
            if(openList.Count <= 0)
            {
                Debug.Log("OpenList empty");
                //Has searched everything.
                return -1;
            }
            AStarNode getNode = openList[0];
            closedList.Add(getNode);
            openList.RemoveAt(0); 
            //Debug.Break();

            if(getNode.GetPosition() == aiLocationEndGoal)
            {
                search = false;
                //Calculate the path backwards using the parent variable of each AStarNode, then calculate the first move.
                List<AStarNode> pathList = new List<AStarNode>();
                AStarNode findPathNode = getNode;
                if(findPathNode.GetParent() != null)
                {
                    while(findPathNode.GetParent().GetParent() != null)
                    {
                        findPathNode = findPathNode.GetParent();
                    }
                }
                return findPathNode.GetMoveDir();
            }
            if(search)
            {
                //Calculates neighbour nodes.
                for(int i = 2; i <= 8; i+=2)
                {
                    //Traversable if there is nothing blocking movement there.
                    //ToDo: Check for slopes and drops here.
                    //Only counts walls - causes enemy to "bump" into characters.
                    //Vector3 testVectorULDR = new Vector3(0.0f + -1.0f*((i == 4) ? 1 : 0) +  1.0f*((i == 6) ? 1 : 0), 0.0f ,0.0f  + -1.0f*((i == 2) ? 1 : 0) +  1.0f*((i == 8) ? 1 : 0));
                    
                    Vector3 lookPos = getNode.GetPosition() + new Vector3(0.0f + -1.0f*((i == 4) ? 1 : 0) +  1.0f*((i == 6) ? 1 : 0), 0.0f ,0.0f  + -1.0f*((i == 2) ? 1 : 0) +  1.0f*((i == 8) ? 1 : 0));
                    
                    //Tries to find lookpos in closed.
                    bool foundInClosedList = false;
                    for(int icl = 0; icl < closedList.Count; icl++)
                    {
                        if(closedList[icl].GetPosition() == lookPos)
                        {
                            foundInClosedList = true;
                            icl = closedList.Count;
                        }
                    }
                    //Gets the wall if there is one.
                    bool blocked = false;
                    bool ground = false;
                    GameObject getWallObj = thisCharacter.GetWall(lookPos);
                    if(getWallObj != null)
                    {
                        //Checks if there is a slope. If the slope is too sheer, it's a wall.
                        float relativeWallSize = getWallObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                        Debug.Log(relativeWallSize);
                        if((relativeWallSize < -0.55f)
                        || (relativeWallSize > 0.55f)
                        )
                        {
                            blocked = true;
                        }
                        else
                        {
                            //Looks for ground to walk on
                            lookPos.y += relativeWallSize;
                            ground = true;
                        }
                    }
                    else
                    {
                        //Looks for floor
                        GameObject getGroundObj = thisCharacter.GetWall(lookPos + new Vector3(0.0f, -1.0f, 0.0f));
                        Debug.Log("Looking for floor");
                        if(getGroundObj != null)
                        {
                            float relativeGroundSize = getGroundObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                            lookPos.y += relativeGroundSize;
                            ground = true;
                            Debug.Log("Found floor");
                        }
                    }
                    /*

                            if(thisCharacter.GetWall(lookPos + new Vector3(0.0f, -1.0f, 0.0f))
                            || relativeWallSize > -0.5f)
                            {
                                ground = true;
                            }
                    */

                    if((!blocked)
                    && (ground)
                    && (!foundInClosedList)
                    )
                    {
                        //Calculates f value.
                        float newHValue = Vector3.Distance(lookPos, aiLocationEndGoal);
                        float newGValue = getNode.GetG() + 1; //counts every movement as an additional 1.
                        float newFValue = newGValue + newHValue;

                        //Creates neighbour
                        AStarNode thisNeighbour = new AStarNode(lookPos, getNode, newFValue, newGValue, newHValue , i);

                        //Tries to find lookpos in Open.
                        bool foundInOpenList = false;
                        int foundOpenListInd = -1;
                        for(int iop = 0; iop < openList.Count; iop++)
                        {
                            if(openList[iop].GetPosition() == lookPos)
                            {
                                foundInOpenList = true;
                                foundOpenListInd = iop;
                                iop = openList.Count;
                            }
                        }
                        
                        //If not found, inserts at the appropriate position
                        if(!foundInOpenList)
                        {
                            if(openList.Count > 0)
                            {
                                bool foundPos1 = false;
                                for(int ipos = 0; ipos < openList.Count; ipos++)
                                {
                                    if(openList[ipos].GetF() > thisNeighbour.GetF())
                                    {
                                        openList.Insert(ipos, thisNeighbour);
                                        ipos = openList.Count+1;
                                        foundPos1 = true;
                                    }
                                }
                                if(!foundPos1)
                                {
                                    openList.Add(thisNeighbour);
                                }
                            }
                            else
                            {
                                openList.Add(thisNeighbour);
                            }
                        }
                        //If found, checks if this is smaller. If smaller, removes and re-adds at the appropriate position.
                        if(foundInOpenList)
                        {
                            if(openList[foundOpenListInd].GetF() > thisNeighbour.GetF())
                            {
                                openList.RemoveAt(foundOpenListInd);
                                if(openList.Count > 0)
                                {
                                    bool foundPos2 = false;
                                    for(int ipos = 0; ipos < openList.Count; ipos++)
                                    {
                                        if(openList[ipos].GetF() > thisNeighbour.GetF())
                                        {
                                            openList.Insert(ipos, thisNeighbour);
                                            ipos = openList.Count+1;
                                            foundPos2 = true;
                                        }
                                    }
                                    if(!foundPos2)
                                    {
                                        openList.Add(thisNeighbour);
                                    }
                                }
                                else
                                {
                                    openList.Add(thisNeighbour);
                                }
                            }
                        }
                    }
                }
            }

            //Breaks out of loop if looped too many times.
            // Remove this later on? or make it bigger?
            searchi++;
            /*
            if(searchi == 1)
            {
                Debug.Log("Open List:");
                for(int opli = 0; opli < openList.Count; opli++)
                {
                    Debug.Log(openList[opli].GetPosition().ToString() + "; " + openList[opli].GetF().ToString());
                }
                Debug.Log("Closed List:");
                for(int opli = 0; opli < closedList.Count; opli++)
                {
                    Debug.Log(closedList[opli].GetPosition().ToString() + "; " + closedList[opli].GetF().ToString());
                }
            }
            */
            //Debug.Break();
            if(searchi > 100)
            {
                Debug.Log("Open List:");
                for(int opli = 0; opli < openList.Count; opli++)
                {
                    Debug.Log(openList[opli].GetPosition().ToString() + "; " + openList[opli].GetF().ToString());
                }
                Debug.Log("Closed List:");
                for(int opli = 0; opli < closedList.Count; opli++)
                {
                    Debug.Log(closedList[opli].GetPosition().ToString() + "; " + closedList[opli].GetF().ToString());
                }
                Debug.Log("Searchi:"+searchi.ToString());
                search = false;
            }
        }
        //return -1;  //If for some reason a path cannot be found.
        return -1;
    }

    private class AStarNode
    {
        Vector3 position;
        AStarNode parent;
        float f;
        float g;
        float h;
        int movedir;    //Move direction, either 2, 4, 6 or 8 for down, left, right or up respectively.
        //Setters and Getters
        public AStarNode(Vector3 newPosition, AStarNode newParent,float newf,float newg,float newh, int newMovedir)
        {
            position = newPosition;
            parent = newParent;
            f = newf;
            g = newg;
            h = newh;
            movedir = newMovedir;
        }

        public int GetMoveDir()
        {
            return movedir;
        }

        public Vector3 GetPosition()
        {
            return position;
        }
        public void SetPosition(Vector3 newPosition)
        {
            position  = newPosition;
        }

        public AStarNode GetParent()
        {
            return parent;
        }
        public void SetParent(AStarNode newParent)
        {
            parent  = newParent;
        }

        public float GetF()
        {
            return f;
        }
        public void SetF(float newf)
        {
            f  = newf;
        }

        public float GetG()
        {
            return g;
        }
        public void SetG(float newg)
        {
            g  = newg;
        }

        public float GetH()
        {
            return h;
        }
        public void SetH(float newh)
        {
            h  = newh;
        }
    }
}
