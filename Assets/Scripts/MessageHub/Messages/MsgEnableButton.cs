// MsgEnableButton.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using static LevelButtons;

/// <summary>
/// Mensaje para activar un botón.
/// </summary>
public class MsgEnableButton
{
    /// <summary>
    /// El botón.
    /// </summary>
    public Buttons button;

    /// <summary>
    /// Inicializa la clase.
    /// </summary>
    /// <param name="button">El botón a activar.</param>
    public MsgEnableButton(Buttons button)
    {
        this.button = button;
    }
}