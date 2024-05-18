using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTester : MonoBehaviour
{
    public ParticleSystem particle;
    public KeyCode playKey = KeyCode.Q;
    public KeyCode stopKey = KeyCode.W;
    public KeyCode pauseKey = KeyCode.E;
    public KeyCode clearKey = KeyCode.R;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(playKey))
        {
            Play();
        }
        if (Input.GetKeyDown(stopKey))
        {
            Stop();
        }
        if (Input.GetKeyDown(pauseKey))
        {
            Pause();
        }
        if (Input.GetKeyDown(clearKey))
        {
            Clear();
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        particle?.Play();
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        particle?.Stop();
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        particle?.Pause();
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        particle?.Clear();
    }
}
