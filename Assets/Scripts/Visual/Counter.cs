using UnityEngine;

public class Counter : MonoBehaviour
{
    public int maxNumber = 9;
    public int defaultNumber = 0;
    private GameObject numbersParent;
    private GameObject[] numbers;
    private int actualNumber;

    public int ActualNumber { get => actualNumber; }

    // Start is called before the first frame update
    private void Awake()
    {
        numbersParent = transform.Find("Numbers").gameObject;
        numbers = new GameObject[maxNumber + 1];
        for (int i = 0; i <= maxNumber; i++)
        {
            numbers[i] = numbersParent.transform.Find("RepeatsX" + i).gameObject;
            numbers[i].SetActive(false);
        }
        actualNumber = SetNumber(defaultNumber);
    }

    public int SetNumber(in int number)
    {
        numbers[actualNumber].SetActive(false);
        int numberAux = number;
        if (number < 0)
        {
            numberAux = maxNumber;
        }

        if (number > maxNumber)
        {
            numberAux = 0;
        }

        numbers[numberAux].SetActive(true);
        actualNumber = numberAux;
        return numberAux;
    }
}