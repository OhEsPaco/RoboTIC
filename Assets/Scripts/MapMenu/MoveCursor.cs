// MoveCursor.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using UnityEngine;

/// <summary>
/// Esta clase manda un mensaje para ejecutar el método OnPlace() de su padre cuando se hace tap en ella.
/// </summary>
public class MoveCursor : MonoBehaviour
{
    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        transform.parent.gameObject.SendMessage("OnPlace");
    }
}