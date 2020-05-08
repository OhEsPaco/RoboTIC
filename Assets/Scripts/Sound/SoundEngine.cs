using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEngine : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private EventAggregator eventAggregator;

    // Start is called before the first frame update
    private void Awake()
    {
        if (eventAggregator != null)
        {
            audioSource = GetComponent<AudioSource>();
            eventAggregator.Subscribe<MsgPlaySfx>(PlaySfx);
            eventAggregator.Subscribe<MsgPlaySfxAtPoint>(PlaySfxAtPoint);
        }
        else
        {
            Debug.LogError("EventAggregator must be present to play sounds");
        }
    }

    private void PlaySfx(MsgPlaySfx msg)
    {
        audioSource.PlayOneShot(msg.clip, msg.volume);
    }

    private void PlaySfxAtPoint(MsgPlaySfxAtPoint msg)
    {
        AudioSource.PlayClipAtPoint(msg.clip, msg.point, msg.volume);
    }

    private void PauseClip(AudioClip clip)
    {
    }

    private void StopClip(AudioClip clip)
    {
    }
}