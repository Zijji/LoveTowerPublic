using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterControllerAttacker : MonoBehaviour
{
    public HumanCharacter thisHuman;    //Controlling the human externally in this script
    private Character thisCharacter;    //Getting some functionality from character.

    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        thisCharacter = GetComponent<Character>();
    }


    // Update is called once per frame
    void Update()
    {
        
        thisHuman.SetAttacking(true , 0);
    }
}
