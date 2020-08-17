using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    List<Vector3> storedPathList = null;
    private HashSet<Vector3> reachablePositions;    //Stores all reachable positions.
    private HashSet<Vector3> outsidePositions;    //Stores all outside positions - that is all unreachable positions.
    // Start is called before the first frame update
    void Start()
    {
        reachablePositions = new HashSet<Vector3>();
        outsidePositions = new HashSet<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector3> GetStoredPath()
    {
        return storedPathList;
    }
    //Uses heuristics to find the adjacent square with the highest f value.
    public int AStarHeuristicNextStep(Vector3 endPosition = default(Vector3))
    {
        int returnInt = -1;
        float? storedF = null;
        for(int i = 2; i <= 8; i+=2)
        {
            Vector3 lookPos = transform.position + new Vector3(0.0f + -1.0f*((i == 4) ? 1 : 0) +  1.0f*((i == 6) ? 1 : 0), 0.0f ,0.0f  + -1.0f*((i == 2) ? 1 : 0) +  1.0f*((i == 8) ? 1 : 0));
            //Gets the wall if there is one.
            bool blocked = false;
            bool ground = false;
            GameObject getWallObj = GetWall(lookPos);
            if(getWallObj != null)
            {
                //Checks if there is a slope. If the slope is too sheer, it's a wall.
                float relativeWallSize = getWallObj.transform.position.y - transform.position.y + 1.0f;
                //Debug.Log(relativeWallSize);
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
                GameObject getGroundObj = GetWall(lookPos + new Vector3(0.0f, -1.0f, 0.0f));
                //Debug.Log("Looking for floor");
                if(getGroundObj != null)
                {
                    float relativeGroundSize = getGroundObj.transform.position.y - transform.position.y + 1.0f;
                    lookPos.y += relativeGroundSize;
                    ground = true;
                    //Debug.Log("Found floor");
                }
            }
            //Checks for character blocking
            if(!blocked)
            {
                if(CharAtPos(lookPos)
                && (endPosition != lookPos))
                {
                    blocked = true;
                }
            }
            if((!blocked)
            && (ground)
            )
            {
                //Calculates f value.
                float newHValue = Vector3.Distance(lookPos, endPosition);
                float newGValue = 1; //counts every movement as an additional 1.
                float newFValue = newGValue + newHValue;
                if((storedF == null)
                || (newFValue < storedF)
                )
                {
                    returnInt = i;
                    storedF = newFValue;
                }
            }
        }
        return returnInt;
    }
    //Uses A* pathfinding to find the next step, ignoring characters but not walls.
    /*
    public int AStarFindNextMoveNoChar(Vector3 startPosition = default(Vector3), Vector3 endPosition = default(Vector3), int cancelSearch = 250)
    {
        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();
        //Adds a new node to open list to start.
        AStarNode newStarNode = new AStarNode(startPosition, null, 0, 0, 0, 0);
        openList.Add(newStarNode);
        bool search = true;
        int searchi = 0;
        while(search)
        {
            //Retrieves node with lowest F - defaults to top of list.
            if(openList.Count <= 0)
            {
                //Has searched everything.
                return -1;
            }
            AStarNode getNode = openList[0];
            closedList.Add(getNode);
            openList.RemoveAt(0); 
            if(getNode.GetPosition() == endPosition)
            {
                search = false;
                //Calculate the path backwards using the parent variable of each AStarNode, then calculate the first move.
                List<Vector3> pathList = new List<Vector3>();
                AStarNode findPathNode = getNode;
                if(findPathNode.GetParent() != null)
                {
                    while(findPathNode.GetParent().GetParent() != null)
                    {
                        findPathNode = findPathNode.GetParent();
                        pathList.Insert(0,findPathNode.GetPosition());
                    }
                }
                storedPathList = pathList;
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
                    GameObject getWallObj = GetWall(lookPos);
                    if(getWallObj != null)
                    {
                        //Checks if there is a slope. If the slope is too sheer, it's a wall.
                        float relativeWallSize = getWallObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                        //Debug.Log(relativeWallSize);
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
                        GameObject getGroundObj = GetWall(lookPos + new Vector3(0.0f, -1.0f, 0.0f));
                        //Debug.Log("Looking for floor");
                        if(getGroundObj != null)
                        {
                            float relativeGroundSize = getGroundObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                            lookPos.y += relativeGroundSize;
                            ground = true;
                            //Debug.Log("Found floor");
                        }
                    }
                    if((!blocked)
                    && (ground)
                    && (!foundInClosedList)
                    )
                    {
                        //Calculates f value.
                        float newHValue = Vector3.Distance(lookPos, endPosition);
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
            if(searchi > cancelSearch)
            {
                search = false;
            }
        }
        return -1;
    }
    */

    //Returns true if the location is reachable, false if it is outside, and null if the location hasn't been calculated yet.
    public bool? LocationReachable(Vector3 getLocation)
    {
        if(reachablePositions.Contains(getLocation))
        {
            return true;
        }
        else if(outsidePositions.Contains(getLocation))
        {
            return false;
        }
        //returns null otherwise
        return null;
    }
    //Uses A* pathfinding to find the next step.
    public int AStarFindNextMove(Vector3 startPosition = default(Vector3), Vector3 endPosition = default(Vector3),bool lookChar = true, int cancelSearch = 250)
    {
        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();
        //Adds a new node to open list to start.
        AStarNode newStarNode = new AStarNode(startPosition, null, 0, 0, 0, 0);
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
                //Debug.Log("OpenList empty");
                //Has searched everything.
                return -1;
            }
            AStarNode getNode = openList[0];
            closedList.Add(getNode);
            openList.RemoveAt(0); 
            //Debug.Break();

            if(getNode.GetPosition() == endPosition)
            {
                search = false;
                //Calculate the path backwards using the parent variable of each AStarNode, then calculate the first move.
                List<Vector3> pathList = new List<Vector3>();
                AStarNode findPathNode = getNode;
                if(findPathNode.GetParent() != null)
                {
                    while(findPathNode.GetParent().GetParent() != null)
                    {
                        findPathNode = findPathNode.GetParent();
                        pathList.Insert(0,findPathNode.GetPosition());
                    }
                }
                storedPathList = pathList;
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
                    GameObject getWallObj = GetWall(lookPos);
                    if(getWallObj != null)
                    {
                        //Checks if there is a slope. If the slope is too sheer, it's a wall.
                        float relativeWallSize = getWallObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                        //Debug.Log(relativeWallSize);
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
                        GameObject getGroundObj = GetWall(lookPos + new Vector3(0.0f, -1.0f, 0.0f));
                        //Debug.Log("Looking for floor");
                        if(getGroundObj != null)
                        {
                            float relativeGroundSize = getGroundObj.transform.position.y - getNode.GetPosition().y + 1.0f;
                            lookPos.y += relativeGroundSize;
                            ground = true;
                            //Debug.Log("Found floor");
                        }
                    }
                    //Checks for character blocking
                    if(!blocked)
                    {
                        if(CharAtPos(lookPos)
                        && (lookChar)
                        && (endPosition != lookPos))
                        {
                            blocked = true;
                        }
                    }
                    if((!blocked)
                    && (ground)
                    && (!foundInClosedList)
                    )
                    {
                        //Calculates f value.
                        float newHValue = Vector3.Distance(lookPos, endPosition);
                        float newGValue = getNode.GetG() + 1; //counts every movement as an additional 1.
                        float newFValue = newGValue + newHValue;

                        //Creates neighbour
                        AStarNode thisNeighbour = new AStarNode(lookPos, getNode, newFValue, newGValue, newHValue , i);
                        //Adds the position to the reachablePosition
                        reachablePositions.Add(lookPos);

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
            if(searchi > cancelSearch)
            {
                /*
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
                */
                //Assumes it's not reachable
                if(!reachablePositions.Contains(endPosition))
                {
                    outsidePositions.Add(endPosition);
                }
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

    
    //Gets the wall at position in vector3
    public GameObject GetWall(Vector3 wallLookVec)
    {
        //Checks for collision with walls
        Collider[] allCols;
        //Returns true if there is a wall found in the vector indicated by wallLookVec.
        if((allCols = Physics.OverlapSphere(wallLookVec, 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getWallObj = collider.gameObject;
                if(getWallObj.CompareTag("Wall")
                )
                {
                    return getWallObj;
                }
            }
        }
        return null;
    }

    
    //Gets the wall at position in vector3
    public bool CharAtPos(Vector3 wallLookVec)
    {
        //Checks for collision with walls
        Collider[] allCols;
        //Returns true if there is a wall found in the vector indicated by wallLookVec.
        if((allCols = Physics.OverlapSphere(wallLookVec, 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getObj = collider.gameObject;
                if(getObj.GetComponent<Character>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
