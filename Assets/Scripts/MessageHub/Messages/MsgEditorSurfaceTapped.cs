public class MsgEditorSurfaceTapped
{
    private EditorSurfacePoint tappedPoint;

    public MsgEditorSurfaceTapped(EditorSurfacePoint tappedPoint)
    {
        this.tappedPoint = tappedPoint;
    }

    public EditorSurfacePoint TappedPoint { get => tappedPoint; }
}