using UnityEngine;

public class MoveCursor : MonoBehaviour
{
    public void OnSelect()
    {
        transform.parent.gameObject.SendMessage("OnPlace");
    }
}