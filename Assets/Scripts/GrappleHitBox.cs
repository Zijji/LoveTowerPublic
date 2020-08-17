using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHitBox : MonoBehaviour
{
    public float timeToDes = 0.05f;
    public GameObject ownerCharacterObj = null;
    public int[] damageArea;        //Areas affected by the damage. these are all the num pad areas.
                                    //i.e. 5 == middle; 4 == left; 6 == right; 8 == up; 2 == down;
    public GameObject parryEffObj; //parry Effect created when the hitbox is parried.
    public GameObject grappleEffObj; //grapple Effect created when the hitbox hits.
    public CharacterHeldPunch ownerCharHold; //The owner's hand.

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToDes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        Character getChar = other.gameObject.GetComponent(typeof(Character)) as Character;
        if (getChar != null
        && !GameObject.ReferenceEquals(ownerCharacterObj, other.gameObject))
        {
            //Debug.Log("Found Character!");
            //Basic defence code - will be expanded later
            //Ignores all defences and just causes grappled condition
            HumanIsGrappled newIsGrappledScr = getChar.gameObject.AddComponent(typeof(HumanIsGrappled)) as HumanIsGrappled;
            //getChar.StumbleAdditive(0.5f + getChar.GetMoveWaitTime(), 1);
            //newIsGrappledScr.SetMoveWait(7.5f + getChar.GetMoveWaitTime());
            newIsGrappledScr.SetMoveWait(0.5f + getChar.GetMoveWaitTime());
            
            HumanCauseGrappling newCauseGrappScr = ownerCharacterObj.AddComponent(typeof(HumanCauseGrappling)) as HumanCauseGrappling;
            newIsGrappledScr.SetCauseGrapplerScr(newCauseGrappScr);
            newCauseGrappScr.SetIsGrappledScr(newIsGrappledScr);
            ownerCharHold.UpdateCauseGrap(newCauseGrappScr);
            //Grappled effect
            Instantiate(grappleEffObj, this.transform.position, Quaternion.identity);

            //Adds the isgrappling condition to the grappler

            //var newDamEff = Instantiate(grappleEffObj, this.transform.position, Quaternion.identity);
            //newDamEff.GetComponent<DamageNumber>().UpdateNumber(damage);
            /*
            if(getChar.isDefending == false)
            {
                getChar.healthPoints-= damage;
                var newDamEff = Instantiate(damageNumObj, this.transform.position, Quaternion.identity);
                newDamEff.GetComponent<DamageNumber>().UpdateNumber(damage);
            }
            else
            {
                if(((getChar.defenceType == 1)
                || (getChar.defenceType == 2))
                && (Array.Exists(damageArea, element => element == getChar.GetDefDir()) ))      ///REPLACE THIS WITH Array.IndexOf or something  better!!!
                {
                    getChar.healthPoints-= damage;
                    Debug.Log("Defence but damaged!");
                }
                
                else if(getChar.defenceType == 1)
                {
                    Debug.Log("Tap Def!");
                }
                else if(getChar.defenceType == 2)
                {
                    Debug.Log("Held Def!");
                }
                else if(getChar.defenceType == 3)
                {
                    Debug.Log("Parry Def!");
                    var newParryEff = Instantiate(parryEffObj, this.transform.position, Quaternion.identity);
                    Destroy(newParryEff,0.6f);
                }
                else
                {
                    Debug.Log("Huh? Defence?");
                }
                
            }
            */
            Destroy(gameObject);
        }
    }
    public void UpdateOwnerCharObj(GameObject thisObj)
    {
        ownerCharacterObj = thisObj;
    }
    public void UpdateOwnerHand(CharacterHeldPunch newCharHold)
    {
        ownerCharHold = newCharHold;
    }

    public void UpdateDamageArea(int[] newDamageArea)
    {
        //Damage areas: all areas that are covered by the attack.
        damageArea = newDamageArea;
    }
}
