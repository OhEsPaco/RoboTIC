using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
     private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Play());
    }

    private void Play()
    {
        if (sound != null)
        {
            EventAggregator.instance.Publish<MsgPlaySfx>(new MsgPlaySfx(sound, 1.0f));
        }
    }
}