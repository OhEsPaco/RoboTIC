using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadButton : MonoBehaviour
{
    public int buttonIndex = 0;
    private LevelManager manager;

    // Start is called before the first frame update
    void Awake()
    {
        manager = LevelManager.instance;
    }
    void OnMouseDown()
    {
        manager.Logic.ButtonInput(buttonIndex);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
