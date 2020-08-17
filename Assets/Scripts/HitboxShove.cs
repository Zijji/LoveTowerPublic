using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Special variant of the hitbox used to shove things
public class HitboxShove : MonoBehaviour
{
    private float damage = 0.0f;
    public int moveDirection = -1;
    public float timeToDes = 0.034f;
    public GameObject characterObj = null;
    public Character characterObjChar;
    public GameObject damageEffObj; //damage effect created when the hitbox hits.
    public float moveMag = 0.0f;    //move magnitude or how far is the character moved.
    float thisMoveMag = 0.0f;

    public GameObject otherObj = null;

    public int stage = 0;           //Stage of hitbox shoving
                                    //0 == needs to find character; 1 == found character, waiting for push 

    // Start is called before the first frame update
    void Start()
    {      
        if(characterObj != null)
        {
            characterObjChar = characterObj.GetComponent<Character>();
        }
        
        //Debug.Log(characterObjChar == null);
        Destroy(gameObject, timeToDes);
    }

    // Update is called once per frame
    void Update()
    {
        if(stage == 1)
        {
            //if(characterObjChar.FindWall(characterObj.transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag) )  == false)
            if(characterObj != null)
            {
                if(characterObjChar.FindWallOrChar(characterObj.transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag) )  == false)
                {
                    //Debug.Log("move shove");
                    characterObjChar.ForceMoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag);
                    Destroy(gameObject);
                    stage = 2;
                }
            }
            
            /*
            //if(characterObjChar.FindWall(characterObj.transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag) )  == false)
            if(characterObj != null)
            {
                float relativeWallSize = 0.0f;
                Vector3 shovePosition = characterObj.transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag);
                bool shoveBlocked = false;
                
                Debug.Log("Char found");
                if(characterObjChar.FindWallOrChar(shovePosition)  == true)
                {
                    Debug.Log("Found wallorchar");
                    shoveBlocked = true;    //Set to true - assumes it's a character in the way.
                    GameObject getWallObj = characterObjChar.GetWall(shovePosition);
                    if((getWallObj != null)
                    && (characterObjChar.FindCharAdj(shovePosition) == null)
                    )
                    {
                        Debug.Log("Calculate relative size");
                        //measures slope size.
                        relativeWallSize = getWallObj.transform.position.y - transform.position.y + 1.0f;
                        if((relativeWallSize < -0.55f)
                        || (relativeWallSize > 0.55f)
                        )
                        {
                            shoveBlocked = true;
                        }
                        Debug.Log(relativeWallSize);
                    }
                }
                if(!shoveBlocked)
                {
                    //Debug.Log("move shove");
                    characterObjChar.ForceMoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,relativeWallSize,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag);
                    Destroy(gameObject);
                    stage = 2;

                }
            }
            */
            
        }
        //Debug.Log("ShoveStage: "+stage.ToString());
    }
    void OnTriggerEnter(Collider other)
    {
        if(stage == 0)
        {
            //Debug.Log("Collided!");
            Character getChar = other.gameObject.GetComponent(typeof(Character)) as Character;
            HumanCharacter getHumChar = other.gameObject.GetComponent(typeof(HumanCharacter)) as HumanCharacter;
            //Debug.Log(getChar == null);     //True if getchar is null???
            //Debug.Log(other.gameObject.name);     //True if getchar is null???
            if (getChar != null
            && !GameObject.ReferenceEquals(characterObj, other.gameObject))
            {
                //shoves the character no matter if they're defending or not
                getChar.healthPoints-= damage;
                if(damage > 0.0f)   //Only shows 'damage' if the character has been damaged.
                {
                    var newDamEff = Instantiate(damageEffObj, this.transform.position, Quaternion.identity);
                    newDamEff.GetComponent<DamageNumber>().UpdateNumber(damage);
                }
                bool moveMagFound = false;
                float relativeWallSize = 0.0f;
                if(moveMag > 0.0f)
                {   
                    while(!moveMagFound)
                    {
                        thisMoveMag++;
                        if(!(thisMoveMag < moveMag))
                        {
                            moveMagFound = true;
                        }
                        if(getChar.FindWall(transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag) )  == true)
                        {
                            GameObject getWallObj = getChar.GetWall(transform.position + new Vector3 (0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag) );
                            relativeWallSize = getWallObj.transform.position.y - transform.position.y + 1.0f;
                            if((relativeWallSize < -0.55f)
                            || (relativeWallSize > 0.55f)
                            )
                            {
                                thisMoveMag--;
                                moveMagFound = true;
                            }
                        }
                    }
                }
                getChar.ForceMoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,relativeWallSize,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag);
                getChar.Stumble(0.5f);
                getHumChar.SetShovePushed(moveDirection, -1);
                getHumChar.SetIsHitStun(true);
                                /*
                thisMoveMag--;
                if(thisMoveMag > 0.0f)
                {
                    //Moves the original object
                    characterObj.GetComponent<Character>().ForceMoveXYZ(0.0f + -1.0f*((moveDirection == 4) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 6) ? 1 : 0)*thisMoveMag,0.0f,0.0f  + -1.0f*((moveDirection == 2) ? 1 : 0)*thisMoveMag +  1.0f*((moveDirection == 8) ? 1 : 0)*thisMoveMag);

                }
                */
                //Destroy(gameObject);
                //Debug.Log("Trigger Shove");
                stage = 1;
            }
        }
        
    }
    void OnDestroy()
    {
        
        /*
        for(int i = 0; i < damageArea.Length; i++)
        {
            Debug.Log(damageArea[i]);
        }
        */
    }
    public void UpdateDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void UpdateCharObj(GameObject thisObj)
    {
        characterObj = thisObj;
    }
    public void UpdateMoveDirection(int newMoveDirection)
    {
        moveDirection = newMoveDirection;   //Does not need to check here as code assumes values != 2,4,6,8 are invalid.
    }
    public void UpdateMoveMagnitude(float newMoveMag)
    {
        moveMag = newMoveMag;   //Does not need to check here as code assumes values != 2,4,6,8 are invalid.
    }
    public void UpdateOtherObj(GameObject newOtherObj)
    {
        otherObj = newOtherObj;
    }
}
