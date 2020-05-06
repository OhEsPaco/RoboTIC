using UnityEngine;

public class MsgPlaySfx
{
    public AudioClip clip;
    public float volume;

    public MsgPlaySfx(AudioClip clip, float volume)
    {
        this.clip = clip;
        this.volume = volume;
    }
}