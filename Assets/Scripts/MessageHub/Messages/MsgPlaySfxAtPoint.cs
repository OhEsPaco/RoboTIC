using UnityEngine;

public class MsgPlaySfxAtPoint
{
    public AudioClip clip;
    public float volume;
    public Vector3 point;

    public MsgPlaySfxAtPoint(AudioClip clip, float volume, Vector3 point)
    {
        this.clip = clip;
        this.volume = volume;
        this.point = point;
    }
}