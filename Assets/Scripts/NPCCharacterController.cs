using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacterController : MonoBehaviour
{
    public int moveDirection;
    private HumanCharacter thisHuman;
    // Start is called before the first frame update
    void Start()
    {
        thisHuman = GetComponent<HumanCharacter>();
        
    }

    // Update is called once per frame
    void Update()
    {
        thisHuman.SetMoving(true, moveDirection);
        thisHuman.SetFaceDirection(moveDirection);
        
    }
}
