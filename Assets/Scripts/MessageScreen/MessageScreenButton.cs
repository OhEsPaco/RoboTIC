// MessageScreenButton.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using UnityEngine;
using static MessageScreenManager;

/// <summary>
/// Clase para los botones de <see cref="MessageScreen".
/// </summary>
[RequireComponent(typeof(Collider))]
public class MessageScreenButton : MonoBehaviour
{
    /// <summary>
    /// Delegado para informar cuando el usuario hace click.
    /// </summary>
    private OnMessageScreenButtonPressed informOnPressed;

    /// <summary>
    /// Tipo del botón.
    /// </summary>
    [SerializeField] private string buttonType;

    /// <summary>
    /// Para suscribirse al delegado.
    /// </summary>
    public OnMessageScreenButtonPressed InformOnPressed { get => informOnPressed; set => informOnPressed += value; }

    /// <summary>
    /// Retorna o cambia el tipo de botón.
    /// </summary>
    public string ButtonType { get => buttonType; set => buttonType = value; }

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        informOnPressed?.Invoke();
    }

    /// <summary>
    /// Pone el delegado a null.
    /// </summary>
    public void ResetDelegates()
    {
        informOnPressed = null;
    }
}