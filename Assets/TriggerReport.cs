using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerReport : MonoBehaviour
{
    public List<Collider> colliders;

    private void Start()
    {
        colliders = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!(colliders.Contains(other)))
        {
            colliders.Add(other);
        }
    }
}
