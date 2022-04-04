using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TriggerEntity : MonoBehaviour
{
    public bool once = false;

    public delegate bool TriggerCallback(Collider other);
    private List<TriggerCallback> holders = new List<TriggerCallback>();

    public void RegisterCallback(TriggerCallback c)
    {
        holders.Add(c);
    }

    public void OnTriggerEnter(Collider other)
    {
        bool test = false;
        foreach (TriggerCallback c in holders)
            if (c != null && c.Invoke(other))
                test = true;

        if (once && test)
        {
            Destroy(gameObject);
        }
    }
}
