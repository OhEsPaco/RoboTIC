using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRenderer : MonoBehaviour
{
    public float actionSpeed = 1f;
    public float rotationSpeedMultiplier = 100f;
    private LevelManager manager;

    private void Awake()
    {
        manager = LevelManager.instance;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DoJump(GameObject player, List<int> currentBlock, List<int> intendedBlock, int playerOrientation)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);

        StartCoroutine(JumpCoroutine(player, target, actionSpeed, 1f, 1f));
    }

    private IEnumerator JumpCoroutine(GameObject player, Vector3 target, float speed, float arcHeight, float angle)
    {
        for (float currentTime = 0; currentTime <= 1; currentTime += speed * Time.deltaTime)
        {
            Vector3 newPoint = Vector3.Lerp(player.transform.position, target, currentTime);

            player.transform.position = new Vector3(newPoint.x, (-angle * arcHeight * currentTime * currentTime + angle * arcHeight * currentTime) + Mathf.Lerp(player.transform.position.y, target.y, currentTime), newPoint.z);

            yield return null;
        }

        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();
    }

    public void DoTurnRight(GameObject player)
    {
        StartCoroutine(TurnCoroutine(rotationSpeedMultiplier * actionSpeed, player, 90f, false));
    }

    public void DoTurnLeft(GameObject player)
    {
        StartCoroutine(TurnCoroutine(rotationSpeedMultiplier * actionSpeed, player, 90f, true));
    }

    private IEnumerator TurnCoroutine(float speed, GameObject player, float degrees, bool left)
    {
        int sign = left ? -1 : 1;
        float currentDegrees = 0f;
        for (; currentDegrees < degrees; currentDegrees += speed * Time.deltaTime)
        {
            float step = sign * (speed * Time.deltaTime);
            player.transform.Rotate(0, step, 0);
            yield return null;
        }

        //error correction
        player.transform.Rotate(0, sign * (degrees - currentDegrees), 0);
        NotifyEndOfAction();
    }

    public void DoMove(GameObject player, List<int> currentBlock, List<int> intendedBlock)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);

        StartCoroutine(MoveCoroutine(actionSpeed, player, target, manager.MapRenderer.blockLength));
    }

    private void NotifyEndOfAction()
    {
        manager.Logic.NotifyEndOfAction();
    }

    private IEnumerator MoveCoroutine(float speed, GameObject player, Vector3 target, float distance)
    {
        for (float currentDistance = 0f; currentDistance < distance; currentDistance += speed * Time.deltaTime)
        {
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            yield return null;
        }
        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();
    }
}