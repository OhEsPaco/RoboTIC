using UnityEngine;

public class LoopCounter : MonoBehaviour
{
    [SerializeField] private int maxNumber = 9;
    [SerializeField] private int defaultNumber = 0;
    private GameObject numbersParent;
    private GameObject[] numbers;
    private int actualNumber;
    private bool locked = false;
    public bool Locked { get { return locked; } }

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

    public int SetNumber(int number)
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

    public void OnSelect()
    {
        if (!locked)
        {
            SetNumber(actualNumber + 1);
        }
    }

    private void OnMouseDown()
    {
        if (!locked)
        {
            SetNumber(actualNumber + 1);
        }
    }

    public int ActualNumber()
    {
        return actualNumber;
    }

    public void Lock()
    {
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }
}