using UnityEngine;

public class EditorSaveButton : MonoBehaviour
{
    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorSaveMap>(new MsgEditorSaveMap());
    }
}