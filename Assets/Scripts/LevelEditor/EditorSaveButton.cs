// EditorSaveButton.cs 
// Francisco Manuel García Sánchez - Belmonte
// 2020

using UnityEngine;

/// <summary>
/// Manda un mensaje <see cref="MsgEditorSaveMap"/> cuando el editor hace tap sobre una instancia de esta clase.
/// </summary>
public class EditorSaveButton : MonoBehaviour
{
    /// <summary>
    /// Manda un mensaje <see cref="MsgEditorSaveMap"/> cuando el editor hace tap sobre una instancia de esta clase.
    /// </summary>
    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorSaveMap>(new MsgEditorSaveMap());
    }
}
