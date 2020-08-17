using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCauseGrappling : CharacterCanMoveBlock
{
    public HumanIsGrappled isGrappledScr;       //The script of the character that is grappled
    public Character getChar;
    public HumanCharacter getHuman;
    public Vector3 prevPos;
    private Vector3 grappleTargRelPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        getChar = GetComponent<Character>();
        if(getChar != null)
        {
            //getChar.AddMoveBlockObjs(this as Component);
            //Debug.Log(this as Component);
        }
        else
        {
            Destroy(this);
        }
        getHuman = GetComponent<HumanCharacter>();
        //variables for moving while grappled
        prevPos = gameObject.transform.position;
        getChar.Stumble(0.2f,0);
    }

    // Update is called once per frame
    void Update()
    {
        var humFallDown = GetComponent<HumanFallenDown>();
        var charIsGrappled = GetComponent<HumanIsGrappled>();
        if((isGrappledScr == null)
        ||(humFallDown != null)
        ||(charIsGrappled != null)
        ||(getHuman.GetIsHitStun())
        ||(getHuman.GetFalling())
        )
        {
            EndGrapple();
        }
        //Don't have to do that, just make the enemy move to the position using same logic as enemy logic
        /*
        if(prevPos != gameObject.transform.position
        && grappleTargRelPos == Vector3.zero)
        {
            grappleTargRelPos = gameObject.transform.position - prevPos;    //Gets relative position of grappled target
        }
        if(grappleTargRelPos != Vector3.zero
        && isGrappledScr.gameObject.GetComponent<Character>().FindWallOrChar(new Vector3(grappleTargRelPos.x, grappleTargRelPos.y, grappleTargRelPos.z)))
        {
            isGrappledScr.gameObject.GetComponent<Character>().ForceMoveXYZ(grappleTargRelPos.x, grappleTargRelPos.y, grappleTargRelPos.z); 
            grappleTargRelPos = Vector3.zero;
            prevPos = gameObject.transform.position;
        }
        */
    }

    public void SetIsGrappledScr(HumanIsGrappled newIsGrappled)
    {
        isGrappledScr = newIsGrappled;
    }

    public HumanIsGrappled GetIsGrappledScr()
    {
        return isGrappledScr;
    }

    public void EndGrapple()
    {
        Destroy(isGrappledScr);
        Destroy(this);
    }
    public GameObject GetGrappledHuman()
    {
        if(isGrappledScr != null)
        {
            return isGrappledScr.gameObject;
        }
        return null;
    }
}