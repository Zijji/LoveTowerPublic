using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
[RequireComponent(typeof(Character))]
public class EnemyCharacter : MonoBehaviour
{
    Character thisChar;
    //Facing direction:
    public int faceDirection = 2;   //2 == down; 6 == right; 8 == up; 4 == left; (like numpad)
    public float moveMagnitude = 1.0f;
    //Attacks are handled here (except for drawing which is handled in PlayerAnimate
    //Time for attack frames
    public float attackWindUp1Time = 0.2f;
    public float attackWindUp2Time = 0.4f;
    public float attackHitTime = 0.8f;
    public float attackRecoverTime = 1.0f;
    //Is attacking?
    public bool isAttacking = false;
    public float attackWait = 0.0f;
    public enum AttackStage { noAttack, windup1, windup2, hit, recover };
    public AttackStage curAttackStage = AttackStage.noAttack;
    public GameObject attackHitBox;
    //Character stats
    public float damage = 22.0f;


    public GameObject PlayerSprite;

    private PlayerAnimate pSAnim;   //PlayerSprite anim script

    // Start is called before the first frame update
    void Start()
    {
        thisChar = GetComponent<Character>();
        //Checks if PlayerSprite has a PlayerAnimate script
        pSAnim = PlayerSprite.GetComponent<PlayerAnimate>();    //Should NOT be null!
    }

    // Update is called once per frame
    void Update()
    {

        //Gets the facing direction
        /*
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > moveMagnitude)
        {
            if(Input.GetAxis("Mouse X") > 0)
            {
                faceDirection = 6;
            }
            if(Input.GetAxis("Mouse X") < 0)
            {
                faceDirection = 4;
            }
        }
        if((Mathf.Abs(Input.GetAxis("Mouse Y")) > moveMagnitude))
        {
            if(Input.GetAxis("Mouse Y") > 0)
            {
                faceDirection = 8;
            }
            if(Input.GetAxis("Mouse Y") < 0)
            {
                faceDirection = 2;
            }
        }

        if(Input.GetButton("Fire1"))
        {
            isAttacking = true;
        }
        */
        isAttacking = true;
        if(isAttacking)
        {
            if(attackWait <= 0.0f)
            {
                switch(curAttackStage)
                {
                    case AttackStage.noAttack:
                        curAttackStage = AttackStage.windup1;
                        attackWait = attackWindUp1Time;
                        break;
                    case AttackStage.windup1:
                        curAttackStage = AttackStage.windup2;
                        attackWait = attackWindUp2Time;
                        break;
                    case AttackStage.windup2:
                        curAttackStage = AttackStage.hit;
                        attackWait = attackHitTime;
                        //Creates box here
                        Vector3 hitboxloc = new Vector3(0.0f,0.0f,0.0f);
                        switch(faceDirection)
                        {
                            case 2: //down
                                hitboxloc = new Vector3(0.0f,0.0f,-1.0f);
                                break;
                            case 4: //left
                                hitboxloc = new Vector3(-1.0f,0.0f,0.0f);
                                break;
                            case 6: //right
                                hitboxloc = new Vector3(1.0f,0.0f,0.0f);
                                break;
                            case 8: //up
                                hitboxloc = new Vector3(0.0f,0.0f,1.0f);
                                break;
                        }
                        var newHB = Instantiate(attackHitBox, transform.position + hitboxloc, transform.rotation);
                        newHB.GetComponent<Hitbox>().UpdateDamage(damage);
                        newHB.GetComponent<Hitbox>().UpdateCharObj(this.gameObject);
                        break;
                    case AttackStage.hit:
                        curAttackStage = AttackStage.recover;
                        attackWait = attackRecoverTime;
                        break;
                    case AttackStage.recover:
                        curAttackStage = AttackStage.noAttack;
                        attackWait = 0.0f;
                        isAttacking = false;
                        break;
                }
                int attackStageInt = (int) curAttackStage - 1;
                pSAnim.Attack(attackStageInt);//, faceDirection);
                
            }
            else
            {
                attackWait -= Time.deltaTime;
            }
        }
        //Checks input
        /*
        if(thisChar.CanMove())
        {
            if(Input.GetAxis("Vertical") < 0.0f)
            {
                thisChar.MoveXYZ(0.0f,0.0f,-1.0f);
                pSAnim.Walk(2);
            }
            else if(Input.GetAxis("Vertical") > 0.0f)
            {
                thisChar.MoveXYZ(0.0f,0.0f,1.0f);
                pSAnim.Walk(8);
            }
            else if(Input.GetAxis("Horizontal") < 0.0f)
            {
                thisChar.MoveXYZ(-1.0f,0.0f,0.0f);
                pSAnim.Walk(4);
            }
            else if(Input.GetAxis("Horizontal") > 0.0f)
            {
                thisChar.MoveXYZ(1.0f,0.0f,0.0f);
                pSAnim.Walk(6);
            }
        }
        */
        //PlayerSprite.transform.position = Vector3.MoveTowards(PlayerSprite.transform.position, this.transform.position, (float) Vector3.Distance(PlayerSprite.transform.position,this.transform.position)*0.08f);
        
        //pSAnim.SetFaceDirection(faceDirection);
        //float rotation = Input.GetAxis("Horizontal")
    }
    public int getFaceDirection()
    {
        return faceDirection;
    }

}
