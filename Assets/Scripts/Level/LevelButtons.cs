using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtons : MonoBehaviour
{
    public GameObject Action;
    public GameObject Condition;
    public GameObject Jump;
    public GameObject Loop;
    public GameObject Move;
    public GameObject TurnLeft;
    public GameObject TurnRight;

    public GameObject Play;
    public GameObject Restart;

    public int setNumber(int button, int number)
    {
        switch (button)
        {
            case ButtonConstants.Action:
                return Action.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;

            case ButtonConstants.Condition:
                return Condition.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            case ButtonConstants.Jump:
                return Jump.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            case ButtonConstants.Loop:
                return Loop.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            case ButtonConstants.Move:
                return Move.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            case ButtonConstants.TurnLeft:
                return TurnLeft.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            case ButtonConstants.TurnRight:
                return TurnRight.GetComponent<ButtonCounterScript>().SetNumber(number);
                break;
            default:
                return 0;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
