using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimateSubsprite : MonoBehaviour
{
    //head, torso, legs, leftarm and rightarm body sprites
    // x = 0, y = 0.025
    //size = 0.5
    public float pixelSize = 0.05f;
    public Vector3 baseXY;  //Base xy for all sprites except torso. Aligns all sprites to torso.
    public Vector2 adjustedXY;  //how much the baseXY needs to be adjusted for the base of all sprites.
    public GameObject head;
    public GameObject torso;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject legs;

    //XY for body parts.
    public Vector3 headXY;
    public Vector3 torsoXY;
    public Vector3 leftArmXY;
    public Vector3 rightArmXY;
    public Vector3 legsXY;

    // Start is called before the first frame update
    void Start()
    {
        torsoXY = new Vector3(adjustedXY.x, adjustedXY.y, 0.0f);
        //baseXY = new Vector3(adjustedXY.x, adjustedXY.y, 0.0f);
        //headXY = baseXY;
        //torsoXY = baseXY;
        //leftArmXY = baseXY;
        //rightArmXY = baseXY;
        //legsXY = baseXY;
    }

    // Update is called once per frame
    void Update()
    {
        //Sets sprite body locations.
        head.transform.localPosition = new Vector3(0.0f,12*pixelSize,0.0f);
        //torso.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
        leftArm.transform.localPosition = new Vector3(-6*pixelSize,0.0f,0.0f);
        rightArm.transform.localPosition = new Vector3(6*pixelSize,0.0f,0.0f);
        legs.transform.localPosition = new Vector3(0.0f,-11*pixelSize,0.0f);

        /*
        headXY = baseXY + new Vector3(0.0f,-12*pixelSize,0.0f);
        torsoXY = baseXY + new Vector3(0.0f,0*pixelSize,0.0f);
        leftArmXY = baseXY + new Vector3(0.0f,-12*pixelSize,0.0f);
        rightArmXY = baseXY + new Vector3(0.0f,-12*pixelSize,0.0f);
        legsXY = baseXY + new Vector3(0.0f,-12*pixelSize,0.0f);
        */
    }
}
