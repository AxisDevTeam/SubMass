using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDispatcher : MonoBehaviour
{
    public static EnemyDispatcher instance;

    public List<GameObject> listeners = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Register(GameObject gameObject)
    {
        listeners.Add(gameObject);
    }
    
    public void Deregister(GameObject gameObject)
    {
        listeners.Remove(gameObject);
    }

    // Update is called once per frame
    public void PostSoundEvent(GameObject soundSource)
    {
        listeners.ForEach(listener => listener.SendMessage("SoundEvent", soundSource));
    }

}
