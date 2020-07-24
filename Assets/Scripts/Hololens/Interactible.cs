// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

[System.Serializable]
public class InteractibleParameters
{
    public bool Scrollable = true;
    public bool Placeable = true;
}

/// <summary>
/// Esta clase marca un objeto de tal forma que se pueda interactuar con el mismo
/// cuando el usuario lo mira.
/// </summary>
public class Interactible : MonoBehaviour
{
    public InteractibleParameters InteractibleParameters;

    [Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Add a BoxCollider if the interactible does not contain one.
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        EnableAudioHapticFeedback();
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (TargetFeedbackSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = TargetFeedbackSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    private void LateUpdate()
    {
        Debug.ClearDeveloperConsole();
    }
}