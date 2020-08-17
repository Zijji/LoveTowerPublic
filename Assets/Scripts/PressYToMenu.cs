using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressYToMenu : MonoBehaviour
{
    private bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            isActive = !isActive;
            //Assuming parent is the parent game object
            //Code snippet from https://stackoverflow.com/questions/22278821/how-to-deactivate-all-children-in-unity
            for(int i=0; i< transform.childCount; i++)
            {
                var getChild = transform.GetChild(i).gameObject;
                if(getChild != null)
                {
                    getChild.SetActive(isActive);
                }
                    
            }
        }
        
    }
}
