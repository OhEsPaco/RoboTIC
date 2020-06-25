using static EditorLogic;

public class MsgEditorToolSelected
{
    private EditorToolType toolType;
    private int toolIdentifier;

    public EditorToolType ToolType { get => toolType; set => toolType = value; }
    public int ToolIdentifier { get => toolIdentifier; set => toolIdentifier = value; }

    public MsgEditorToolSelected(EditorToolType toolType, int toolIdentifier)
    {
        this.toolType = toolType;
        this.toolIdentifier = toolIdentifier;
    }

    public MsgEditorToolSelected()
    {
        this.toolType = EditorToolType.Eraser;
        this.toolIdentifier = -999;
    }
}