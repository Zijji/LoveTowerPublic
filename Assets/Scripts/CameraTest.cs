using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public float moveMagnitude = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Gets the facing direction
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > moveMagnitude)
        {
            if(Input.GetAxis("Mouse X") > 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 270);
            }
            if(Input.GetAxis("Mouse X") < 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 90);
            }
        }
        if((Mathf.Abs(Input.GetAxis("Mouse Y")) > moveMagnitude))
        {
            if(Input.GetAxis("Mouse Y") > 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 0);
            }
            if(Input.GetAxis("Mouse Y") < 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 180);
            }
        }
        //Debug.Log(Input.GetAxis("Mouse X"));
        /*
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > moveMagnitude)
        {
            if(Input.GetAxis("Mouse X") > 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 270);
            }
            if(Input.GetAxis("Mouse X") < 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 90);
            }
        }
        if((Mathf.Abs(Input.GetAxis("Mouse Y")) > moveMagnitude))
        {
            if(Input.GetAxis("Mouse Y") > 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 0);
            }
            if(Input.GetAxis("Mouse Y") < 0)
            {
                transform.eulerAngles = new Vector3(90, 0, 180);
            }
        }

        */
        
    }
}
