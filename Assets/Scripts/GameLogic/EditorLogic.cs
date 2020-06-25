using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelObject;

public class EditorLogic : MonoBehaviour
{
    [SerializeField] private int mapSizeX = 8;
    [SerializeField] private int mapSizeY = 4;
    [SerializeField] private int mapSizeZ = 8;
    [SerializeField] private GameObject placeableMap;
    private EditorObject[] levelObjects;
    private SelectedTool selectedTool;
    private LevelData newLevel;
    private AvailableInstructions availableInstructions;
    private bool hasFlag = false;
    private bool hasPlayer = false;

    public enum EditorToolType
    {
        Item, Block, Player, Eraser
    }

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgEditorMapSize>(ServeMapSize);
        EventAggregator.Instance.Subscribe<MsgEditorSurfaceTapped>(EditorSurfaceTapped);
        EventAggregator.Instance.Subscribe<MsgEditorToolSelected>(ChangeSelectedTool);
    }

    private void ChangeSelectedTool(MsgEditorToolSelected msg)
    {
        if (selectedTool == null)
        {
            selectedTool = new SelectedTool(msg.ToolType, msg.ToolIdentifier);
        }
        else
        {
            selectedTool.ToolType = msg.ToolType;
            selectedTool.ToolIdentifier = msg.ToolIdentifier;
        }
    }

    private void Start()
    {
        ResetEditor();

        // newLevel = new LevelData();
        /*
             public string levelName;
    public List<int> levelSize;
    public List<int> playerPos;
    public int playerOrientation;
    public List<int> goal;
    public AvailableInstructions availableInstructions;
    public List<int> mapAndItems;

         */

        //newLevel.levelName = Guid.NewGuid().ToString();
        //newLevel.levelSize = new List<int>();
        //newLevel.levelSize.Add(mapSizeX);
        //newLevel.levelSize.Add(mapSizeY);
        //newLevel.levelSize.Add(mapSizeZ);
    }

    private void ResetEditor()
    {
        EventAggregator.Instance.Publish<MsgResetEditorSurface>(new MsgResetEditorSurface());
        selectedTool = null;
        hasFlag = false;
        hasPlayer = false;
        if (levelObjects != null)
        {
            StartCoroutine(DestroyOldObjectsInBackgroundCrt((EditorObject[])levelObjects.Clone()));
        }
        levelObjects = new EditorObject[mapSizeX * mapSizeY * mapSizeZ];
        availableInstructions = new AvailableInstructions();
    }

    private void ServeMapSize(MsgEditorMapSize msg)
    {
        List<int> mapSize = new List<int>();
        mapSize.Add(mapSizeX);
        mapSize.Add(mapSizeY);
        mapSize.Add(mapSizeZ);
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgEditorMapSize, List<int>>(msg, mapSize));
    }

    /*   public enum Blocks
    {
        NoBlock = 0,
        WaterBlock = 1,
        LavaBlock = 2,
        SolidBlock = 3,
        LiftBlock = 4,
        SpikesBlock = 5,
        IceBlock = 6
    };

    public enum Items
    {
        PlankItem = 25,
        FanItem = 26,
        FlagItem = 27,
        ActivatorItem = 28,
        BombItem = 29
    }*/

    private void EditorSurfaceTapped(MsgEditorSurfaceTapped msg)
    {
        if (selectedTool != null)
        {
            int x = msg.TappedPoint.SurfacePointPositionX;
            int z = msg.TappedPoint.SurfacePointPositionZ;

            switch (selectedTool.ToolType)
            {
                case EditorToolType.Item:
                    if (PlaceBlockOrItem(false, x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Block:
                    if (PlaceBlockOrItem(true, x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Player:
                    if (PlaceCharacter(selectedTool.ToolIdentifier, x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Eraser:
                    if (Eraser(x, z))
                    {
                        msg.TappedPoint.Down();
                    }
                    break;
            }
        }
    }

    private bool PlaceCharacter(int orientation, int x, int z, EditorSurfacePoint TappedPoint)
    {
        if (!hasPlayer)
        {
            int y = 0;
            bool insideMap = false;
            for (; y < mapSizeY; y++)
            {
                if (GetEditorObject(x, y, z) == null)
                {
                    insideMap = true;
                    break;
                }
            }

            if (insideMap)
            {
                bool validPos = false;
                EditorObject obj = GetEditorObject(x, y - 1, z);
                //Solo se puede poner un jugador encima de un bloque
                if (obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }

                if (validPos)
                {
                    GameObject spawned = MapRenderer.Instance.RenderMainCharacter();
                    EditorObject newEditorObject = new EditorObject(spawned, selectedTool.ToolType, selectedTool.ToolIdentifier);
                    SetEditorObject(x, y, z, newEditorObject);

                    spawned.transform.parent = TappedPoint.EditorSurface;
                    spawned.transform.rotation = TappedPoint.transform.rotation * spawned.transform.rotation;
                    spawned.transform.Rotate(new Vector3(0, 90f * selectedTool.ToolIdentifier, 0));
                    spawned.transform.position = new Vector3(TappedPoint.transform.position.x,
                        (TappedPoint.EditorSurface.position.y + TappedPoint.BlockLength * y) - TappedPoint.BlockLength / 2,
                        TappedPoint.transform.position.z);

                    hasPlayer = true;
                    return true;
                }
            }
        }
        return false;
    }

    private bool PlaceBlockOrItem(bool isBlock, int x, int z, EditorSurfacePoint TappedPoint)
    {
        int y = 0;
        bool insideMap = false;
        for (; y < mapSizeY; y++)
        {
            if (GetEditorObject(x, y, z) == null)
            {
                insideMap = true;
                break;
            }
        }

        if (insideMap)
        {
            bool validPos = false;
            EditorObject obj = GetEditorObject(x, y - 1, z);
            //No se puede poner un bloque encima de un jugador o un item
            if (isBlock)
            {
                if (obj == null || obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }
            }
            else
            {
                //Solo se puede poner items sobre bloques
                if (obj != null && obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }
            }

            if (validPos)
            {
                /*if(selectedTool.ToolType==EditorToolType.Player && hasPlayer)
                {
                    return false;
                }*/

                //Comprobamos que no se pueda poner mas de una bandera
                if (selectedTool.ToolType == EditorToolType.Item)
                {
                    if (selectedTool.ToolIdentifier == (int)Items.FlagItem)
                    {
                        return false;
                    }
                }
                GameObject spawned = MapRenderer.Instance.SpawnBlock(selectedTool.ToolIdentifier).gameObject;
                EditorObject newEditorObject = new EditorObject(spawned, selectedTool.ToolType, selectedTool.ToolIdentifier);
                SetEditorObject(x, y, z, newEditorObject);

                //Quaternion placeableMapRot = placeableMap.transform.rotation;
                //placeableMap.transform.rotation = new Quaternion();
                spawned.transform.parent = TappedPoint.EditorSurface;
                spawned.transform.rotation = TappedPoint.transform.rotation * spawned.transform.rotation;
                spawned.transform.position = new Vector3(TappedPoint.transform.position.x,
                    TappedPoint.EditorSurface.position.y + TappedPoint.BlockLength * y,
                    TappedPoint.transform.position.z);
                // placeableMap.transform.rotation = placeableMapRot;

                /*if (newEditorObject.ObjectType == EditorToolType.Player)
                {
                    hasPlayer = true;
                }*/
                if (newEditorObject.ObjectType == EditorToolType.Item)
                {
                    if (newEditorObject.ObjectIdentifier == (int)Items.FlagItem)
                    {
                        hasFlag = true;
                    }
                }
                return true;
            }
        }
        return false;
    }

    private bool Eraser(int x, int z)
    {
        int y = 0;

        for (; y < mapSizeY; y++)
        {
            if (GetEditorObject(x, y, z) == null)
            {
                break;
            }
        }

        y--;

        EditorObject objectToErase = GetEditorObject(x, y, z);
        if (objectToErase != null)
        {
            if (objectToErase.AssociatedGameobject != null)
            {
                Destroy(objectToErase.AssociatedGameobject);
            }
            if (objectToErase.ObjectType == EditorToolType.Player)
            {
                hasPlayer = false;
            }
            if (objectToErase.ObjectType == EditorToolType.Item)
            {
                if (objectToErase.ObjectIdentifier == (int)Items.FlagItem)
                {
                    hasFlag = false;
                }
            }
            SetEditorObject(x, y, z, null);
            return true;
        }
        return false;
    }

    private EditorObject GetEditorObject(in int x, in int y, in int z)
    {
        if (x < 0 || x >= mapSizeX) return null;
        if (y < 0 || y >= mapSizeY) return null;
        if (z < 0 || z >= mapSizeZ) return null;
        return levelObjects[x + z * mapSizeX + y * (mapSizeX * mapSizeZ)];
    }

    private bool SetEditorObject(in int x, in int y, in int z, in EditorObject editorObject)
    {
        if (x < 0 || x >= mapSizeX) return false;
        if (y < 0 || y >= mapSizeY) return false;
        if (z < 0 || z >= mapSizeZ) return false;
        levelObjects[x + z * mapSizeX + y * (mapSizeX * mapSizeZ)] = editorObject;
        return true;
    }

    private IEnumerator DestroyOldObjectsInBackgroundCrt(EditorObject[] editorObjects)
    {
        if (editorObjects != null)
        {
            for (int i = 0; i < editorObjects.Length; i++)
            {
                if (editorObjects[i] != null)
                {
                    if (editorObjects[i].AssociatedGameobject != null)
                    {
                        editorObjects[i].AssociatedGameobject.SetActive(false);
                    }
                }
            }

            yield return null;

            for (int i = 0; i < editorObjects.Length; i++)
            {
                if (editorObjects[i] != null)
                {
                    if (editorObjects[i].AssociatedGameobject != null)
                    {
                        Destroy(editorObjects[i].AssociatedGameobject);
                        yield return null;
                    }
                    editorObjects[i] = null;
                }
            }

            editorObjects = null;
        }
    }

    private class SelectedTool
    {
        private EditorToolType toolType;
        private int toolIdentifier;

        public EditorToolType ToolType { get => toolType; set => toolType = value; }
        public int ToolIdentifier { get => toolIdentifier; set => toolIdentifier = value; }

        public SelectedTool(EditorToolType toolType, int toolIdentifier)
        {
            this.toolType = toolType;
            this.toolIdentifier = toolIdentifier;
        }

        public SelectedTool()
        {
            this.toolType = EditorToolType.Eraser;
            this.toolIdentifier = -999;
        }
    }
}