using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public LayerMask observerLayerMask;
    public static SoundManager Instance { get; private set; }
    
    public List<HearingSensor> sensors = new List<HearingSensor>();
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BroadcastSound(SoundInfo soundInfo)
    {
        //sensors.ForEach(sensor => sensor.BroadcastMessage("SoundEvent", soundInfo));

        Collider[] colliders = Physics.OverlapSphere(soundInfo.position, soundInfo.radius, observerLayerMask);
        foreach (var collider in colliders)
        {
            collider.transform.parent.BroadcastMessage("SoundEvent", soundInfo);
        }
    }
}

public class SoundInfo
{
    public Vector3 position;
    public float radius;
    public float volume;

    public SoundInfo(Vector3 position, float radius, float volume)
    {
        this.position = position;
        this.radius = radius;
        this.volume = volume;
    }
}
