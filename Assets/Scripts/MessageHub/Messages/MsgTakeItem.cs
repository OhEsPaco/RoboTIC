// MsgTakeItem.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

/// <summary>
/// Mensaje para recoger un item.
/// </summary>
public class MsgTakeItem
{
    /// <summary>
    /// El item.
    /// </summary>
    public Item item;

    /// <summary>
    /// Número de items en el inventario.
    /// </summary>
    public int numberOfItems;

    /// <summary>
    /// Constructor del mensaje.
    /// </summary>
    /// <param name="item">El item.</param>
    /// <param name="numberOfItems">Número de items en el inventario.</param>
    public MsgTakeItem(Item item, int numberOfItems)
    {
        this.item = item;
        this.numberOfItems = numberOfItems;
    }
}