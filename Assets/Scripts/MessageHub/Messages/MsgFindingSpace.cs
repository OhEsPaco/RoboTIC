// MsgFindingSpace.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

/// <summary>
/// Inicia o para el mensaje de "finding your space..."
/// </summary>
public class MsgFindingSpace
{
    /// <summary>
    /// True si lo inicia, false si lo para.
    /// </summary>
    public bool isFindingSpace;

    /// <summary>
    /// Crea una instancia del mensaje.
    /// </summary>
    /// <param name="isFindingSpace">¿Está buscando el espacio?</param>
    public MsgFindingSpace(bool isFindingSpace)
    {
        this.isFindingSpace = isFindingSpace;
    }
}