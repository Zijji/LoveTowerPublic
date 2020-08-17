using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimateSpriteBodypart : MonoBehaviour
{
    public Sprite[] walkDown; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //walks in the direction indicated by walkDirection, showing the frame indicated by walkFrame
    public void Walk(int walkDirection, int walkframe)
    {
        switch(walkDirection)
        {
            case 2:
                
                break;
            case 4:
                break;
            case 6:
                break;
            case 8:
                break;
            default:
                break;
        }
    }
}
