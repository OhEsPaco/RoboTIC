using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private LevelManager manager;
    private GameObject player;
    private List<int> actions = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        manager = LevelManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = manager.MapRenderer.GetMainCharacter();
        }
    }

    private void ExecuteNextAction()
    {
        if (player != null)
        {

        }
        NotifyEndOfAction();
    }

    private void NotifyEndOfAction()
    {
        
    }


}
