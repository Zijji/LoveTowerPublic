using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCharHealth : MonoBehaviour
{
    public Character getChar;
    public TextMesh getText;
    // Start is called before the first frame update
    void Start()
    {
        getText = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        getText.text = getChar.GetHealthPoints().ToString();
    }
}
