using System;
using System.Collections;
using UnityEngine;
using static MessageScreenManager;

public class MainMenuLogic : MonoBehaviour
{
    private static MainMenuLogic mainMenuLogic;
    [SerializeField] private GameObject placeableMap;

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

    private void Start()
    {
        placeableMap.GetComponent<MapController>().EnableMainMenuControls();
        EventAggregator.Instance.Publish<MsgFindingSpace>(new MsgFindingSpace(true));
        StartCoroutine(FindSpaceAndLaunchMenu());
        //SpaceCollectionManager.Instance.PlaceItemInWorld(placeableMap);

        //yield return new WaitUntil(() => SpaceCollectionManager.Instance.IsReady());
        //SpaceCollectionManager.Instance.PlaceItemInWorld(placeableMap);
    }

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

    public void ShowMainMenu()
    {
        placeableMap.GetComponent<MapController>().EnableMainMenuControls();
        EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("main", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Play", ShowMapMenu),
            Tuple.Create<string, OnMessageScreenButtonPressed>("Exit", ShowEditor),
            Tuple.Create<string, OnMessageScreenButtonPressed>("Editor", ExitProgram)}));
    }

    public void ShowMapMenu()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        MapMenuLogic.Instance.ShowMapMenu();
    }

    public void ShowEditor()
    {
    }

    public void ExitProgram()
    {
        Application.Quit();
    }
}