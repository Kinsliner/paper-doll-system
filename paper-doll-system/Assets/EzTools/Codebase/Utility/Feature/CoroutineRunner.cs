using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance { get { return instance; } }
    private static CoroutineRunner instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
}
