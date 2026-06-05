using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private HashSet<string> _collectedKeys = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKey(string keyID)
    {
        _collectedKeys.Add(keyID);
    }

    public bool HasKey(string keyID)
    {
        return _collectedKeys.Contains(keyID);
    }
}