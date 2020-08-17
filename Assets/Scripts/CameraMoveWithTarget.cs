using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveWithTarget : MonoBehaviour
{
    public GameObject Target;
    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = Target.transform.position;
        //Puts object in the centre of screen.
    }

    // Update is called once per frame
    void Update()
    {
        if(prevPos != Target.transform.position)
        {
            transform.position += (Target.transform.position - prevPos);
        }
        prevPos = Target.transform.position;
    }
}
