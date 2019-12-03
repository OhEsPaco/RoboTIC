using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxNumber = 9;
    public int defaultNumber = 0;
    private GameObject numbersParent;
    private GameObject[] numbers;
    private int actualNumber;
    private bool touchLocked;

    public bool TouchLocked { get => touchLocked; set => touchLocked = value; }

    // Start is called before the first frame update
    void Awake()
    {
        TouchLocked = false;
        numbersParent = transform.Find("Numbers").gameObject;
        numbers = new GameObject[maxNumber + 1];
        for (int i = 0; i <= maxNumber; i++)
        {
            numbers[i] = numbersParent.transform.Find("RepeatsX" + i).gameObject;
            numbers[i].SetActive(false);

        }
        actualNumber = setNumber(defaultNumber);
    }

    public int setNumber(int number)
    {

        numbers[actualNumber].SetActive(false);
        int numberAux = number;
        if (number < 0)
        {
            numberAux = 0;
        }

        if (number > maxNumber)
        {
            numberAux = 0;
        }

        numbers[numberAux].SetActive(true);
        actualNumber = numberAux;
        return numberAux;
    }

    void OnMouseDown()
    {

        // this object was clicked - do something
        if (!TouchLocked)
        {
            actualNumber = setNumber(actualNumber + 1);
        }
    }
    
    public int ActualNumber()
    {
        return actualNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
