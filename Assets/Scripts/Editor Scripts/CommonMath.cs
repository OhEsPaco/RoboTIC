using System;
using UnityEngine;

public class CommonMath
{
    public static float[] CalculateMeshLength(Vector3 localScale, Vector3 boundsSize)
    {
        Vector3 objectSize = Vector3.Scale(localScale, boundsSize);
        float[] size = new float[2];
        size[0] = objectSize.x;
        size[1] = objectSize.z;
        return size;
    }

    public static float CalculateScale(float blockSize, float numberOfBlocks, float actualLenght, float actualScale)
    {
        return (numberOfBlocks * blockSize * actualScale) / actualLenght;
    }

    public static Vector3 CalculateGridPoint(Vector3 position, Vector3 planeMinPos, float blockLength)
    {
        int xfactor = (int)((position.x - planeMinPos.x) / blockLength);
        int zfactor = (int)((position.z - planeMinPos.z) / blockLength);

        Vector3 newPos = new Vector3((planeMinPos.x + (blockLength / 2f) + xfactor * blockLength), position.y, (planeMinPos.z + (blockLength / 2f) + zfactor * blockLength));
        return newPos;
    }

    public static int GetYfactor(Vector3 position, Vector3 planeMinPos, float blockLength)
    {
        return (int)Math.Abs(((position.y - planeMinPos.y) / blockLength));
    }

    public static int GetXfactor(Vector3 position, Vector3 planeMinPos, float blockLength)
    {
        return (int)Math.Abs(((position.x - planeMinPos.x) / blockLength));
    }

    public static int GetZfactor(Vector3 position, Vector3 planeMinPos, float blockLength)
    {
        return (int)Math.Abs(((position.z - planeMinPos.z) / blockLength));
    }
}