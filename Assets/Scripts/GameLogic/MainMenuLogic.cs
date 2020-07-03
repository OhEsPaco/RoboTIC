// MainMenuLogic.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using System;
using System.Collections;
using UnityEngine;
using static MessageScreenManager;

/// <summary>
/// La clase <see cref="MainMenuLogic" /> contiene la lógica del menú principal.
/// </summary>
public class MainMenuLogic : MonoBehaviour
{
    /// <summary>
    /// El objeto que contiene al escenario.
    /// </summary>
    [SerializeField] private GameObject placeableMap;

    /// <summary>
    /// Instancia de la clase a la que los demás objetos pueden acceder.
    /// </summary>
    private static MainMenuLogic mainMenuLogic;

    /// <summary>
    /// Retorna la instancia de la clase.
    /// </summary>
    public static MainMenuLogic Instance
    {
        get
        {
            if (!mainMenuLogic)
            {
                mainMenuLogic = FindObjectOfType(typeof(MainMenuLogic)) as MainMenuLogic;

                if (!mainMenuLogic)
                {
                    Debug.LogError("There needs to be one active MainMenuLogic script on a GameObject in your scene.");
                }
            }

            return mainMenuLogic;
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        placeableMap.GetComponent<MapController>().EnableMainMenuControls();
        EventAggregator.Instance.Publish<MsgFindingSpace>(new MsgFindingSpace(true));
        StartCoroutine(FindSpaceAndLaunchMenu());
    }

    /// <summary>
    /// Espera a que termine de encontrarse el espacio en el que se encuentra el jugador ¡
    /// y lanza el menú principal.
    /// </summary>
    /// <returns>The <see cref="IEnumerator"/>.</returns>
    private IEnumerator FindSpaceAndLaunchMenu()
    {
        bool success = false;
        do
        {
            yield return new WaitUntil(() => SpaceCollectionManager.Instance.IsReady());
            success = SpaceCollectionManager.Instance.PlaceItemInWorld(placeableMap);
        } while (!success);

        EventAggregator.Instance.Publish<MsgFindingSpace>(new MsgFindingSpace(false));

        ShowMainMenu();
    }

    /// <summary>
    /// Lanza el menú principal.
    /// </summary>
    public void ShowMainMenu()
    {
        placeableMap.GetComponent<MapController>().EnableMainMenuControls();
        EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("main", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Play", ShowMapMenu),
            Tuple.Create<string, OnMessageScreenButtonPressed>("Exit", ExitProgram),
            Tuple.Create<string, OnMessageScreenButtonPressed>("Editor", ShowEditor)}));
    }

    /// <summary>
    /// Lanza el menú de mapas.
    /// </summary>
    public void ShowMapMenu()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        MapMenuLogic.Instance.ShowMapMenu();
    }

    /// <summary>
    /// Lanza el editor.
    /// </summary>
    public void ShowEditor()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        EditorLogic.Instance.ShowEditor();
    }

    /// <summary>
    /// Sale del programa.
    /// </summary>
    public void ExitProgram()
    {
        Application.Quit();
    }
}