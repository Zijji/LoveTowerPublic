using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextGameobjectAngle : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public TextMesh thisTextMesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 firstVector = new Vector2(object2.transform.position.x - object1.transform.position.x,object2.transform.position.z - object1.transform.position.z);
        Vector2 secondVector = new Vector2(0.0f,1.0f);
        thisTextMesh.text = Vector2.SignedAngle(firstVector, secondVector).ToString();
        //thisTextMesh.text = Vector2.Angle(new Vector2(object2.transform.position.x,object2.transform.position.z),new Vector2(object1.transform.position.x, object1.transform.position.z)).ToString();
    }
}
