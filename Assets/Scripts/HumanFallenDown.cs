using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanFallenDown : CharacterCanMoveBlock
{
    public Character getChar;
    public HumanCharacter getHuman;

    void Start()
    {
        //Note that being given the fallen down component doesn't actually stumble the character.
        getChar = GetComponent<Character>();
        getHuman = GetComponent<HumanCharacter>();
        if(getChar != null
        && getHuman != null)
        {
            getChar.AddMoveBlockObjs(this as Component);
            getChar.SetCollision(false);
            getHuman.SetFallenDown(true);
        }
        else
        {
            Destroy(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //getHuman.GetIsMovingInput() 
        if((getChar.FindCharAdj(transform.position))
        && (getChar.GetMoveWait() <= 0.3f)
        )
        {
            getChar.Stumble(0.3f, 1);
            getChar.SetMoveWaitEnabled(false);
        }
        else
        {
            if(!getChar.GetMoveWaitEnabled())
            {
                getChar.SetMoveWaitEnabled(true);
            }
        }
        if(getChar.MoveWaitIsZero()
        && (getChar.FindCharAdj(transform.position) == null)
        )
        {
            getChar.Stumble(1.0f);
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        getHuman.SetFallenDown(false);
        getChar.SetCollision(true);
    }
}