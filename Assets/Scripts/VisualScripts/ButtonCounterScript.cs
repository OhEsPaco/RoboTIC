using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCounterScript : MonoBehaviour
{

    private GameObject counterObject;
    private Counter counterScript;
    // Start is called before the first frame update
    void Awake()
    {
        
        counterObject = transform.Find("Counter").gameObject;
        counterScript = counterObject.GetComponent<Counter>();
    }

    public int SetNumber(int number)
    {
        return counterScript.setNumber(number);
    }
    // Update is called once per frame
    void Update()
    {

    }


}
