using UnityEngine;

public class EditorMenuButton : MonoBehaviour
{
    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorMenu>(new MsgEditorMenu());
    }
}