using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
                instance = FindObjectOfType<T>();
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
}
