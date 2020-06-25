using UnityEngine;
using static EditorLogic;

[RequireComponent(typeof(Collider))]
public class EditorTool : MonoBehaviour
{
    [SerializeField] private EditorToolType toolType;
    [SerializeField] private int toolIdentifier;

    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorToolSelected>(new MsgEditorToolSelected(toolType, toolIdentifier));
    }
}