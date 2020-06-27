using static LevelButtons;

public class MsgEditorAvailableInstructionsChanged
{
    private Buttons button;
    private int numberOfInstrucions;

    public MsgEditorAvailableInstructionsChanged(Buttons button, int numberOfInstrucions)
    {
        this.button = button;
        this.numberOfInstrucions = numberOfInstrucions;
    }

    public Buttons Button { get => button; set => button = value; }
    public int NumberOfInstructions { get => numberOfInstrucions; set => numberOfInstrucions = value; }
}