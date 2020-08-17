using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAutoAlign : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
