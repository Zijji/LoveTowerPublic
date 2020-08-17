using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanCharacter))]
public class TestCharacterController : MonoBehaviour
{
    public HumanCharacter thisHuman;    //Controlling the human externally in this script
    private Character thisCharacter;    //Getting some functionality from character.
    public int moveVar = 6;


    private Vector3 prevMouseScreenXY;
    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        thisCharacter = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        thisHuman.SetMoving(true, moveVar);
        if(thisCharacter.CanMove())
        {
            if(moveVar == 4)
            {
                moveVar = 6;
            }
            else
            {
                moveVar = 4;
            }
        }
        
    }
}
