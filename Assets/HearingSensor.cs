using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    public GameObject observer;

    public float hearingRange = 5f;
    public float volumeThreshold = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundEvent(SoundInfo soundInfo)
    {
        Vector3 position = soundInfo.position;
        float volume = soundInfo.volume;

        float percievedVolume = volume * Mathf.Pow( Mathf.Clamp01( (hearingRange - Vector3.Distance(observer.transform.position, position)) / hearingRange), 3 );

        Debug.Log("Sound heard at " + position + " with volume of " + volume + "and percieved volume of " + percievedVolume);
    }
}
