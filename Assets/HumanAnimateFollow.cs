using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimateFollow : MonoBehaviour
{
    //Assumes this object is following the parent.
    GameObject humanCharacter;
    Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        humanCharacter = transform.parent.gameObject;
        prevPos = humanCharacter.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(prevPos != humanCharacter.transform.position)
        {
            transform.position += prevPos - humanCharacter.transform.position;
            prevPos = humanCharacter.transform.position;
        } 
    }
}
