using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Special variant of the hitbox used to shove things
public class HitboxTrip : MonoBehaviour
{
    private float damage = 0.0f;
    public int moveDirection = -1;
    public float timeToDes = 0.034f;
    public GameObject characterObj = null;
    public Character characterObjChar;
    public GameObject damageEffObj; //damage effect created when the hitbox hits.
    public float tripMag = 0.0f;    //trip magnitude or how long the character is tripped.

    public GameObject otherObj = null;

    public int stage = 0;           //Stage of hitbox tripping
                                    //0 == needs to find character; 1 == found character, waiting for push 

    // Start is called before the first frame update
    void Start()
    {      
        if(characterObj != null)
        {
            characterObjChar = characterObj.GetComponent<Character>();
        }
        //Debug.Log(tripMag);
        //Debug.Log(characterObjChar == null);
        Destroy(gameObject, timeToDes);
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("ShoveStage: "+stage.ToString());
    }
    void OnTriggerEnter(Collider other)
    {
        if(stage == 0)
        {
            Character getChar = other.gameObject.GetComponent(typeof(Character)) as Character;
            HumanCharacter getHumChar = other.gameObject.GetComponent(typeof(HumanCharacter)) as HumanCharacter;
            if (getChar != null
            && !GameObject.ReferenceEquals(characterObj, other.gameObject))
            {
                //damages the character no matter if they're defending or not
                getChar.healthPoints-= damage;
                if(damage > 0.0f)   //Only shows 'damage' if the character has been damaged.
                {
                    var newDamEff = Instantiate(damageEffObj, this.transform.position, Quaternion.identity);
                    newDamEff.GetComponent<DamageNumber>().UpdateNumber(damage);
                }
                HumanFallenDown newFallenDownScr = getChar.gameObject.AddComponent(typeof(HumanFallenDown)) as HumanFallenDown;
                getChar.StumbleAdditive(tripMag);
                getHumChar.SetIsHitStun(true);
                stage = 1;
            }
        }
        
    }
    void OnDestroy()
    {

    }
    public void UpdateDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void UpdateCharObj(GameObject thisObj)
    {
        characterObj = thisObj;
    }
    public void UpdateTripMagnitude(float newTripMag)
    {
        tripMag = newTripMag;   //Does not need to check here as code assumes values != 2,4,6,8 are invalid.
    }
    public void UpdateOtherObj(GameObject newOtherObj)
    {
        otherObj = newOtherObj;
    }
}
