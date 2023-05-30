using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baracuda.Monitoring;


public class PlayerSoundTester : MonitoredBehaviour
{
    [Monitor]
    public float playSoundVolume = 0;

    [Monitor]
    public float playSoundRadius = 0;

    public float scrollDelta;

    public LayerMask raycastLayerMask;

    public Material sphereMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            playSoundRadius = Mathf.Max(playSoundRadius + (scrollDelta * Input.mouseScrollDelta.y), 0);
        }
        else
        {
            playSoundVolume = Mathf.Max(playSoundVolume + (scrollDelta * Input.mouseScrollDelta.y), 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, raycastLayerMask);
            StartCoroutine(PlaySound(hit.point, playSoundVolume, playSoundRadius));
        }
    }

    private IEnumerator PlaySound(Vector3 pos, float volume, float radius)
    {
        //GameObject.FindGameObjectsWithTag

        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = radius * Vector3.one;
        obj.transform.position = pos;

        Color col = Color.red;
        col.a = 0.5f;

        obj.GetComponent<MeshRenderer>().material = sphereMaterial;
        obj.GetComponent<SphereCollider>().enabled = false;

        Destroy(obj,2);

        SoundInfo si = new SoundInfo(pos, radius, volume);
        SoundManager.Instance.BroadcastSound(si);

        yield return null;
    }
}
