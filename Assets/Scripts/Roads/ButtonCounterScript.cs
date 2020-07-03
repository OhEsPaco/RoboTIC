using UnityEngine;

public class ButtonCounterScript : MonoBehaviour
{
    private GameObject counterObject;
    private Counter counterScript;

    // Start is called before the first frame update
    private void Awake()
    {
        counterObject = transform.Find("Counter").gameObject;
        counterScript = counterObject.GetComponent<Counter>();
    }

    public int SetNumber(int number)
    {
        return counterScript.SetNumber(number);
    }
}