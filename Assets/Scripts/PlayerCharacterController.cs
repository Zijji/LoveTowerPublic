using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanCharacter))]
public class PlayerCharacterController : MonoBehaviour
{
    public HumanCharacter thisHuman;    //Controlling the human externally in this script
    private Character thisCharacter;    //Getting some functionality from character.
    public float mouseMoveMagnitude = 1.0f;
    public TextMesh txt;
    public Camera thisCam;              //Camera of the ui looking at the player.

    [SerializeField]
    private bool attackHeld0 = false;    //Set to true if the attack button has been held.
    [SerializeField]
    private bool attackHeld1 = false;    //Set to true if the attack button has been held.

    private Vector3 prevMouseScreenXY;
    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        thisCharacter = GetComponent<Character>();
        prevMouseScreenXY = new Vector3(Input.mousePosition.x/thisCam.pixelWidth - 0.5f, Input.mousePosition.y/thisCam.pixelHeight - 0.5f, 0.0f);
    }


    int FaceMouse(Vector3 getMouseScreenXy)
    {
        //txt.text = mouseScreenXy.x.ToString("F4") + "\n" +  mouseScreenXy.y.ToString("F4");
        int newFaceDir = -1;
        if(Mathf.Abs(getMouseScreenXy.x) > Mathf.Abs(getMouseScreenXy.y))
        {
            if(getMouseScreenXy.x > 0)
            {
                newFaceDir = 6;
            }
            if(getMouseScreenXy.x < 0)
            {
            newFaceDir = 4;
            }
        }
        if(Mathf.Abs(getMouseScreenXy.x) < Mathf.Abs(getMouseScreenXy.y))
        {
            if(getMouseScreenXy.y > 0)
            {
            newFaceDir = 8;
            }
            if(getMouseScreenXy.y < 0)
            {
            newFaceDir = 2;
            }
        }
        return newFaceDir;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.mousePosition);
        //Debug.Log(thisCam.WorldToScreenPoint(transform.position));
        //updates text with HP
        if(txt != null)
        {
            txt.text = thisCharacter.GetHealthPoints().ToString("F0");
        }
        
        //Gets the facing direction
        //Keyboard facing
        if(Input.GetButton("LookDown"))
        {
            thisHuman.SetFaceDirection(2);
        }
        else if(Input.GetButton("LookUp"))
        {
            thisHuman.SetFaceDirection(8);
        }
        else if(Input.GetButton("LookLeft"))
        {
            thisHuman.SetFaceDirection(4);
        }
        else if(Input.GetButton("LookRight"))
        {
            thisHuman.SetFaceDirection(6);
        }
        //mouse facing - absolute 
        Vector3 mouseScreenXy = new Vector3(Input.mousePosition.x/thisCam.pixelWidth - 0.5f, Input.mousePosition.y/thisCam.pixelHeight - 0.5f, 0.0f);
        if(mouseScreenXy != prevMouseScreenXY)
        {
            //Debug.Log("MOUSE MOVED");
            prevMouseScreenXY = mouseScreenXy;

            int getMouseFace = FaceMouse(mouseScreenXy);
            if(thisHuman.GetFaceDirection() != getMouseFace)
            {
                //Debug.Log("UPDATE FACEDIR");
                thisHuman.SetFaceDirection(getMouseFace);
            }
        }
        
        //mouse facing - relative 
        //Commented out for now, may put back in later
        /*
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > mouseMoveMagnitude)
        {
            if(Input.GetAxis("Mouse X") > 0)
            {
                thisHuman.SetFaceDirection(6);
            }
            if(Input.GetAxis("Mouse X") < 0)
            {
               thisHuman.SetFaceDirection(4);
            }
        }
        if((Mathf.Abs(Input.GetAxis("Mouse Y")) > mouseMoveMagnitude))
        {
            if(Input.GetAxis("Mouse Y") > 0)
            {
               thisHuman.SetFaceDirection(8);
            }
            if(Input.GetAxis("Mouse Y") < 0)
            {
               thisHuman.SetFaceDirection(2);
            }
        }
        */
        /*
        if(Input.GetButton("Fire2"))
        {
            thisHuman.SetAttacking(true , 1);
        }
        else
        {
            thisHuman.SetAttacking(false , 1);
        }
        */
        /*
        if(thisHuman.GetAttacking())
        {
            if(Input.GetButtonDown("Fire1"))
            {
                //thisHuman.Feint();
            }
            if(Input.GetButton("Fire1"))
            {
                //thisHuman.SetAttackType(1);
                //txt.text = "Overhead\n"+txt.text;
            }
            else
            {
                //thisHuman.SetAttackType(0);
                //txt.text = "Slash\n"+txt.text;
            }
            
        }
        else
        {
            
        }
        */

        //Resets Attacking
        thisHuman.SetAttacking(false , 0);
        thisHuman.SetAttacking(false , 1);
        //Attacking
        if(!attackHeld0)
        {
            if(Input.GetButton("Fire1"))
            {
                //Faces attack.
                int getMouseFace = FaceMouse(mouseScreenXy);
                Debug.Log(getMouseFace);
                if(thisHuman.GetFaceDirection() != getMouseFace)
                {
                    //Debug.Log("UPDATE FACEDIR");
                    thisHuman.SetFaceDirection(getMouseFace);
                }
                //Gets mouse click
                thisHuman.SetAttacking(true , 0);
            }
            else
            {
                thisHuman.SetAttacking(false , 0);
            }
            if(thisHuman.GetAttackWait(0) > 0.0f)
            {
                attackHeld0 = true;
                thisHuman.SetAttacking(false , 0);
            }
        }
        if(!Input.GetButton("Fire1"))
        {
            attackHeld0 = false;
        }
        

        if(!attackHeld1)
        {
            if(Input.GetButton("Fire2"))
            {
                //Faces attack.
                int getMouseFace = FaceMouse(mouseScreenXy);
                Debug.Log(getMouseFace);
                if(thisHuman.GetFaceDirection() != getMouseFace)
                {
                    //Debug.Log("UPDATE FACEDIR");
                    thisHuman.SetFaceDirection(getMouseFace);
                }
                thisHuman.SetAttacking(true , 1);
            }
            else
            {
                thisHuman.SetAttacking(false , 1);
            }
            if(thisHuman.GetAttackWait(1) > 0.0f)
            {
                attackHeld1 = true;
                thisHuman.SetAttacking(false , 1);
            }
        }
        if(!Input.GetButton("Fire2"))
        {
            attackHeld1 = false;
        }


        //Defending
        if(Input.GetButton("Defend"))
        {
            if(Input.GetButton("MoveDown"))
            {
                thisHuman.SetDefending(true, 2);
            }
            else if(Input.GetButton("MoveUp"))
            {
                thisHuman.SetDefending(true, 8);
            }
            else if(Input.GetButton("MoveLeft"))
            {
                thisHuman.SetDefending(true, 4);
            }
            else if(Input.GetButton("MoveRight"))
            {
                thisHuman.SetDefending(true, 6);
            }

            
            //Defending with weapons
            if(Input.GetButton("Fire1"))
            {
                thisHuman.SetBlocking(true , 0);
            }
            if(Input.GetButton("Fire2"))
            {
                thisHuman.SetBlocking(true , 1);
            }
        }
        else
        {
            GameObject shoveChar;
            if(Input.GetButton("MoveDown"))
            {
                //Shoves if there's a character there.
                shoveChar = thisCharacter.FindCharAdj(transform.position + new Vector3(0.0f, 0.0f, -1.0f));
                if((shoveChar != null)
                && (shoveChar != this.transform.gameObject)
                )
                {
                   //Debug.Log Debug.Log("Shove Down");
                    thisHuman.SetShove(true,2);
                }
                else
                {
                    thisHuman.SetMoving(true, 2);
                }
                
            }
            else if(Input.GetButton("MoveUp"))
            {
                shoveChar = thisCharacter.FindCharAdj(transform.position + new Vector3(0.0f, 0.0f, 1.0f));
                if((shoveChar != null)
                && (shoveChar != this.transform.gameObject)
                )
                {
                    //Debug.Log("Shove Up");
                    thisHuman.SetShove(true, 8);
                }
                else
                {
                    thisHuman.SetMoving(true, 8);
                }
            }
            else if(Input.GetButton("MoveLeft"))
            {
                shoveChar = thisCharacter.FindCharAdj(transform.position + new Vector3(-1.0f, 0.0f, 0.0f));
                if((shoveChar != null)
                && (shoveChar != this.transform.gameObject)
                )
                {
                    //Debug.Log("Shove Left");
                    thisHuman.SetShove(true, 4);
                }
                else
                {
                    thisHuman.SetMoving(true, 4);
                }
            }
            else if(Input.GetButton("MoveRight"))
            {
                shoveChar = thisCharacter.FindCharAdj(transform.position + new Vector3(1.0f, 0.0f, 0.0f));
                if((shoveChar != null)
                && (shoveChar != this.transform.gameObject)
                )
                {
                    //Debug.Log("Shove Right");
                    thisHuman.SetShove(true, 6);
                }
                else
                {
                    thisHuman.SetMoving(true, 6);
                }
            }

        }
        //Moving

        
    }
}
