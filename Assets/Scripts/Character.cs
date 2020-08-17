using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Public script used for Characters e.g. Players and Enemies
    //Should only store what every character can do.
    public float moveWait = 0.0f;       //Current time
    public float moveWaitTime = 0.8f;   //Time to wait between movements
    public bool moveWaitEnabled = true;

    //private bool shouldMove = false;
    
    //Statistics are handled in the character scripts
    public float healthPoints = 1.0f;
    public float maxHealthPoints = 1.0f;

    private Transform thisTrans;

    //Tap defences
    public bool isDefending = false;
    public bool hasDefended = false;    //True when the character has defended. Used mostly for animation e.g. animating sword parries.
    public int defenceDirection= -1; //DefenceTypes: match with keyboard face directions (i.e. 2 == down, 6 == right, 8 == up, 4 == left)
    public int defenceType= -1; //DefenceTypes: Tap or Held defence. 1 == tap; 2 == held; 3 == parry; 4 == block

    public List<Component> moveBlockObjs;  //List of objects or components that block movement. If there are none, character can move.

    private Collider thisCollider;  //For enabling/disabling collider.
    

    // Start is called before the first frame update
    void Start()
    {
        moveBlockObjs = new List<Component>();
        thisTrans = GetComponent<Transform>();
        thisCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if((moveWait > 0)
        && (moveWaitEnabled)
        )
        {
            moveWait -= Time.deltaTime;
        }
        //'dies' 
        if(healthPoints <= 0.0f)
        {
            Destroy(gameObject);
        }
        //Checks movement blockers
        if(moveBlockObjs.Count > 0)
        {
            for(int i = 0; i < moveBlockObjs.Count; i++)
            {
                if(moveBlockObjs[i] == null)
                {
                    moveBlockObjs.RemoveAt(i);
                }
            }
        }
    }
    public void MoveXYZ(float newX, float newY, float newZ)
    {
        if(CanMove())
        {
            //Checks for collision with walls
            bool wallFound = false;
            bool charFound = false;
            GameObject storedWallObj = null;
            Collider[] allCols;
            if((allCols = Physics.OverlapSphere(thisTrans.position + new Vector3(newX, newY, newZ), 0.3f)).Length > 0)
            {
                foreach(var collider in allCols)
                {
                    var getWallObj = collider.gameObject;
                    if(getWallObj.CompareTag("Wall")
                    )
                    {
                        storedWallObj = getWallObj;
                        wallFound = true;
                    }
                    else if((getWallObj.GetComponent<Character>() != null))
                    {
                        charFound = true;   //Should be a shove attack, can't move through for now.
                    }
                }
            }
            if((!wallFound)
            &&(!charFound)
            )
            {
                thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
            }
            /*
            if((wallFound)
            && (!charFound)
            )
            {
                Debug.Log("Wall found but Character not found");
                if((thisTrans.position.y - storedWallObj.transform.position.y < 0.65f)
                && (FindWall(new Vector3(newX,newY + 0.5f,newZ)))
                )
                {
                    thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
                } 
                //if()CheckEmpty()
                
            }
            */
            moveWait = moveWaitTime;
        }
        /*
        GameObject[] getCharsHere = CheckEmpty();
        if(getCharsHere != null)
        {
            //Randomly chooses one to go back
            thisTrans.position = thisTrans.position + new Vector3(-newX,-newY,-newZ);
        }
        */
    }
    //Forcemove: used when a character is forced to move somewhere (e.g. from a shove.) Ignores movement
    public void ForceMoveXYZ(float newX, float newY, float newZ)
    {
        //Checks for collision with walls
        bool wallFound = false;
        Collider[] allCols;
        if((allCols = Physics.OverlapSphere(thisTrans.position + new Vector3(newX, newY, newZ), 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getWallObj = collider.gameObject;
                if(getWallObj.CompareTag("Wall")
                )
                {
                    wallFound = true;
                    //Debug.Log("Found Wall");
                }
                else if((getWallObj.GetComponent<Character>() != null))
                {
                    wallFound = true;
                    //Debug.Log("Found Character");
                    //Debug.Log("Character Found: "+getWallObj.name);
                }
            }
        }
        //Debug.Log(gameObject.name + " " + wallFound.ToString());
        
        if(!wallFound)
        {
            thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
        }
        
        //thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
        moveWait = moveWaitTime;
    }
    //Forcemovenowait: Forces a move with no wait time.
    public void ForceMoveXYZNoWait(float newX, float newY, float newZ)
    {
        //Checks for collision with walls
        bool wallFound = false;
        Collider[] allCols;
        if((allCols = Physics.OverlapSphere(thisTrans.position + new Vector3(newX, newY, newZ), 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getWallObj = collider.gameObject;
                if(getWallObj.CompareTag("Wall")
                )
                {
                    wallFound = true;
                }
                else if((getWallObj.GetComponent<Character>() != null))
                {
                    wallFound = true;
                }
            }
        }
        if(!wallFound)
        {
            thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
        }
    }
    //Forcemovenowaitnocol: Forces a move with no wait time and no collision.
    public void ForceMoveXYZNoWaitNoCol(float newX, float newY, float newZ)
    {
        //Checks for collision with walls
        thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
    }
    //ForcemoveNoChar: except it ignores characters.
    public void ForceMoveXYZIgnoreChar(float newX, float newY, float newZ)
    {
        //Checks for collision with walls
        bool wallFound = false;
        Collider[] allCols;
        if((allCols = Physics.OverlapSphere(thisTrans.position + new Vector3(newX, newY, newZ), 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getWallObj = collider.gameObject;
                if(getWallObj.CompareTag("Wall")
                )
                {
                    wallFound = true;
                    //Debug.Log("Found Wall");
                }
            }
        }
        if(!wallFound)
        {
            thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
        }
        //thisTrans.position = thisTrans.position + new Vector3(newX,newY,newZ);
        moveWait = moveWaitTime;
    }
    public GameObject[] CheckEmpty()
    {
        List<GameObject> charactersInPos = new List<GameObject>();
        Collider[] allCols;
        if((allCols = Physics.OverlapSphere(thisTrans.position, 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getObj = collider.gameObject;
                if((getObj.GetComponent<Character>() != null)
                && (getObj != this.gameObject)
                )
                {
                    charactersInPos.Add(getObj);   //Should be a shove attack, can't move through for now.
                }
            }
        }
        if(charactersInPos.Count > 0)
        {
            GameObject[] returnObjs = charactersInPos.ToArray();
            return returnObjs;
        }
        else
        {
            return null;
        }
    }
    //'Stumbling' is when a character has it's move wait set for whatever reason.
    //This stumble is for stubmles with an absolute value
    //Stumble types: 0 == relative, 1 == absolute
    public void Stumble(float newMWT, int stumbleType = 0)
    {
        //Only 'stumbles' if the movewait is less than 0. Stops the player from mashing dodge and moving to skip movements
        //However may result in other bugs elsewhere.
        if(!(moveWait > 0))
        {
            if(stumbleType == 0)
            {
                moveWait = newMWT*moveWaitTime;
            }
            else if(stumbleType == 1)
            {
                moveWait = newMWT;
            }
        }
    }
    //
    //Stumble additive adds on to the current move time.
    public void StumbleAdditive(float addMWT, int stumbleType = 0)
    {
        if(stumbleType == 0)
        {
            moveWait += addMWT*moveWaitTime;
        }
        else if(stumbleType == 1)
        {
            moveWait += addMWT;
        }
    }
    public void ResetMoveWait()
    {
        moveWait = 0.0f;
    }
    public float GetMoveWaitTime()
    {
        return moveWaitTime;
    }
    public bool IsStumbling()
    {
        if(moveWait > 0)
        {
            return true;
        }
        return false;
    }
    public bool CanMove()
    {
        if((moveWait <= 0)
        && (moveBlockObjs.Count <= 0)
        )
        {
            return true;
        }
        return false;
    }

    //returns if move wait is less than or equal to zero
    public bool MoveWaitIsZero()
    {
        if((moveWait <= 0))
        {
            return true;
        }
        return false;
    }

    //returns movewait
    public float GetMoveWait()
    {
        return moveWait;
    }

    public void Defend(bool newIsDef, int newDefDir, int newDefType)
    {
        //This character is defending from an attack.
        isDefending = newIsDef;
        defenceDirection = newDefDir;
        defenceType = newDefType;
    }
    public int GetDefDir()
    {
        return defenceDirection;
    }

    public bool FindWall(Vector3 wallLookVec)
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
                    return true;
                }
            }
        }
        return false;
    }
    //Finds a wall or character at that absolute position
    public bool FindWallOrChar(Vector3 wallLookVec)
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
                    return true;
                }
                else if((getWallObj.GetComponent<Character>() != null))
                {
                    return true;
                }
            }
        }
        return false;
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

    //Finds character adjacent to this one.
    //Returns the object that has the character.
    public GameObject FindCharAdj(Vector3 lookVec)
    {
        //Checks for collision with characters
        Collider[] allCols;
        //Returns true if there is a character found in the vector indicated by lookVec.
        if((allCols = Physics.OverlapSphere(lookVec, 0.3f)).Length > 0)
        {
            foreach(var collider in allCols)
            {
                var getObj = collider.gameObject;
                if(getObj.GetComponent<Character>() != null)
                {   
                    //Debug.Log("Found someone");
                    return getObj;
                }
            }
        }
        //Debug.Log("Found noone");
        return null;
    }

    public float GetHealthPoints()
    {
        return healthPoints;
    }

    public void AddMoveBlockObjs(Component getMB)
    {
        moveBlockObjs.Add(getMB);
        //Debug.Log("addlist");
    }

    public void SetCollision(bool getCol)
    {
        thisCollider.enabled  = getCol;
    }

    public bool GetCollision()
    {
        return thisCollider.enabled;
    }

    public void SetHasDefended(bool newHasDefended)
    {
        hasDefended = newHasDefended;
    }

    public bool GetHasDefended()
    {
        bool thisgetHD = hasDefended;
        hasDefended = false;
        return thisgetHD;
    }

    public bool GetMoveWaitEnabled()
    {
        return moveWaitEnabled;
    } 

    public void SetMoveWaitEnabled(bool isMoveWaitEnabled)
    {
        moveWaitEnabled = isMoveWaitEnabled;
    } 
}
