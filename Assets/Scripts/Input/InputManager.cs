using UnityEngine;

public class InputManager
{
    public bool IsPressed(string buttonName)
    {
        if (Input.GetAxis(buttonName) == 1)
        {
            return true;
        }
        return false;
    }
}