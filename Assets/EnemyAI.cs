using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent nma;
    SightSensor sightSensor;
    HearingSensor hearingSensor;

    [Header("Player Reference")]
    public GameObject player;

    [Header("Suspicion Settings")]
    public float suspicionLevel = 0;
    public float suspicionThreshold = 35;
    public float suspicionDetectionSpeed = 20;
    public float suspicionLossSpeed = 10;

    public float suspectedLocationStoppingRadius = 2f;

    public float inspectionMovementSpeed = 1f;
    public float detectedMovementSpeed = 2f;


    [Header("Visible Debug Properties")]
    public Vector3 lastLocationOfInterest = Vector3.zero;
    public bool playerDetected = false;

    

    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
    }

    public void OnPlayerSeen()
    {
        playerDetected = true;
        suspicionLevel = 100;
        Debug.Log("Player was seen");
    }

    public void OnSoundHeard()
    {
        Debug.Log("A noise was heard");
    }

    public void InspectLocation()
    {

        Debug.Log("Detected something suspicious, inspecting...");

        nma.destination = lastLocationOfInterest;
        nma.speed = inspectionMovementSpeed;

        if (Vector3.Distance(transform.position, nma.destination) <= suspectedLocationStoppingRadius)
        {
            nma.isStopped = true;
            nma.ResetPath();
        }
    }

    public void UpdateLocationOfInterest(Vector3 position)
    {
        lastLocationOfInterest = position;
    }

    public void PlayerNotVisiblyDetected()
    {
        UpdateSuspicionLevel(-suspicionLossSpeed * Time.deltaTime);
        playerDetected = false;
    }

    public void PlayerDetectedAction()
    {

        Debug.Log("Following the player");

        nma.destination = player.transform.position;
        nma.speed = detectedMovementSpeed;

        if (Vector3.Distance(transform.position, nma.destination) <= suspectedLocationStoppingRadius)
        {
            nma.isStopped = true;
            nma.ResetPath();
        }
    }

    public void UpdateSuspicionLevel(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel + amount, 0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if(suspicionLevel == 100)
        {
            OnPlayerSeen();
        }

        if (playerDetected)
        {
            PlayerDetectedAction();
        }
        else if(suspicionLevel > suspicionThreshold)
        {
            InspectLocation();
        }
        
    }
}
