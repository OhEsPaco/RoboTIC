using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int maxNumber = 9;
    public int defaultNumber = 0;
    private GameObject numbersParent;
    private GameObject[] numbers;
    private int actualNumber;
    // Start is called before the first frame update
    void Awake()
    {
        numbersParent = transform.Find("Numbers").gameObject;
        numbers = new GameObject[maxNumber + 1];
        for(int i = 0; i <= maxNumber; i++)
        {
            numbers[i] = numbersParent.transform.Find("RepeatsX" + i).gameObject;
            numbers[i].SetActive(false);

        }
        actualNumber=setNumber(defaultNumber);
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
            numberAux = maxNumber;
        }

        numbers[numberAux].SetActive(true);
        actualNumber = numberAux;
        return numberAux;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
