using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEndText : MonoBehaviour
{
    public GameObject checkObject;
    private MeshRenderer thisRenderer;
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkObject == null)
        {
            thisRenderer.enabled = true;
            for(int i=0; i< transform.childCount; i++)
            {
                var getChild = transform.GetChild(i).gameObject;
                if(getChild != null)
                {
                    getChild.SetActive(true);
                }
                    
            }
        }
    }
}
