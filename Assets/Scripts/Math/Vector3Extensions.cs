using UnityEngine;

public static class Vector3Extensions
{
    // Adds two vectors.
    public static Vector3 FastAdd(in this Vector3 a, in Vector3 b)
    {
        Vector3 result;
        result.x = a.x + b.x;
        result.y = a.y + b.y;
        result.z = a.z + b.z;
        return result;
    }

    // Subtracts one vector from another.
    public static Vector3 FastSub(in this Vector3 a, in Vector3 b)
    {
        Vector3 result;
        result.x = a.x - b.x;
        result.y = a.y - b.y;
        result.z = a.z - b.z;
        return result;
    }

    // Negates a vector.
    public static Vector3 FastNeg(in this Vector3 a)
    {
        Vector3 result;
        result.x = -a.x;
        result.y = -a.y;
        result.z = -a.z;
        return result;
    }

    // Multiplies a vector by a number.
    public static Vector3 FastMult(in this Vector3 a, in float d)
    {
        Vector3 result;
        result.x = a.x * d;
        result.y = a.y * d;
        result.z = a.z * d;
        return result;
    }

    // Divides a vector by a number.
    public static Vector3 FastDiv(in this Vector3 a, in float d)
    {
        Vector3 result;
        result.x = a.x / d;
        result.y = a.y / d;
        result.z = a.z / d;
        return result;
    }
}