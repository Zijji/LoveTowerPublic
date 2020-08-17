using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float damage = 1.0f;
    public float timeToDes = 0.05f;
    public GameObject characterObj = null;
    public int[] damageArea;        //Areas affected by the damage. these are all the num pad areas.
                                    //i.e. 5 == middle; 4 == left; 6 == right; 8 == up; 2 == down;
    public GameObject parryEffObj; //parry Effect created when the hitbox is parried.
    public GameObject damageNumObj; //damage number Effect created when the hitbox hits.

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
        && !GameObject.ReferenceEquals( characterObj, other.gameObject))
        {
            //Debug.Log("Found Character!");
            //Basic defence code - will be expanded later
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
                    //getChar.SetHasDefended(true);
                    //Destroy(newParryEff,0.6f);
                }
                else if(getChar.defenceType == 4)
                {
                    if(Array.Exists(damageArea, element => element == getChar.GetDefDir()) )      ///REPLACE THIS WITH Array.IndexOf or something  better!!!
                    {
                        getChar.healthPoints-= damage;
                        Debug.Log("Block Defence but damaged!");
                    }
                    else
                    {
                        Debug.Log("Block Def!");
                        var newParryEff = Instantiate(parryEffObj, this.transform.position, Quaternion.identity);
                        getChar.SetHasDefended(true);
                        Destroy(newParryEff,0.6f);
                        
                    }
                }
                else
                {
                    Debug.Log("Huh? Defence?");
                }
                
            }
            Destroy(gameObject);
        }
    }
    public void UpdateDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void UpdateCharObj(GameObject thisObj)
    {
        characterObj = thisObj;
    }

    public void UpdateDamageArea(int[] newDamageArea)
    {
        //Damage areas: all areas that are covered by the attack.
        damageArea = newDamageArea;
    }
}
