using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public float damageNum = 0.0f;
    private float initScale;
    private float damAlpha;
    public Vector3 randomJump;
    
    private int stage;
    private float stageWait;
    // Start is called before the first frame update
    void Start()
    {
        stage = 0;
        stageWait = 0;
        UpdateNumber(damageNum);
        randomJump = transform.position + new Vector3(0.0f, Random.Range(0.4f, 0.6f),0.0f);//transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f),0.0f);
        //Destroy(gameObject, 0.8f);
        //assumes first child has same scale as all others.
        var getChild = transform.GetChild(0).gameObject;
        initScale = getChild.transform.localScale.x;
        damAlpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(stage == 0)
        {
            stageWait = 0.6f;
            stage = 1;
        }
        if(stage == 1)
        {
            stageWait -= Time.deltaTime;
            if(stageWait <= 0)
            {
                stage = 2;
                stageWait = 0;
            }
        }
        if(stage == 2)
        {
            damAlpha -= Time.deltaTime*2.25f;
            //stageWait-= Time.deltaTime;
            for(int i=0; i< transform.childCount; i++)
            {
                var getChild = transform.GetChild(i).gameObject;
                //getChild.transform.localScale -= new Vector3(Time.deltaTime*initScale, Time.deltaTime*initScale, Time.deltaTime*initScale);
                MeshRenderer getChildRend = getChild.GetComponent<MeshRenderer>();
                getChildRend.material.color = new Color(getChildRend.material.color.r, getChildRend.material.color.g, getChildRend.material.color.b, damAlpha);
            }
            if(damAlpha <= 0)
            {
                Destroy(gameObject);
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, randomJump, (float) Vector3.Distance(transform.position, randomJump)*0.18f); //new Vector3(0.0f, -0.01f, 0.0f);
    }
    
    //Updates the damage number and number shown on all children (assumed to be txt objects)
    public void UpdateNumber(float newDamageNum)
    {
        //Randomly 'jumps' to a position.
        transform.position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f),0.0f);//Random.Range(-0.2f, 0.2f));

        damageNum = newDamageNum;
        //Code snippet from https://stackoverflow.com/questions/22278821/how-to-deactivate-all-children-in-unity
        for(int i=0; i< transform.childCount; i++)
        {
            var getChild = transform.GetChild(i).gameObject;
            if(getChild != null)
            {
                getChild.GetComponent<TextMesh>().text = damageNum.ToString("F0");
            }
        }
    }
}
