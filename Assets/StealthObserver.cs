using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StealthObserver : MonoBehaviour
{
    public bool isDetected;

    public GameObject observerBox;

    public GameObject player;
    private Collider playerCollider;

    public LayerMask detectionMask;

    public float detectionSpeed = 1;
    public float detectionSpeedLossDivider = 2;

    public float suspicionLevel = 0;

    public float suspicionThreshold = 35;

    public float turnToLocationSpeed = 0.5f;


    public TriggerReport eyeView;
    public TriggerReport peripheryView;

    public Vector3 lastKnownLocation;

    private NavMeshAgent nma;

    public float positionSearchRadius = 1.5f;

    public float detectedMovementSpeed = 2.5f;
    public float defaultMovementSpeed = 1.25f;

    public float hearingDistance = 4f;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = player.GetComponent<CharacterController>();
        nma = GetComponent<NavMeshAgent>();
        EnemyDispatcher.instance.Register(gameObject);
    }

    //Debug.DrawRay(observerBox.transform.position, -(observerBox.transform.position - playerCollider.ClosestPoint(observerBox.transform.position)) * 10, Color.red);
    //Physics.Raycast(observerBox.transform.position, -(observerBox.transform.position - playerCollider.ClosestPoint(observerBox.transform.position)), out hitInfo, Mathf.Infinity, detectionMask);

    void Update()
    {
        RaycastHit hitInfo;
        var result = Physics.Raycast(observerBox.transform.position, -(observerBox.transform.position - playerCollider.ClosestPoint(observerBox.transform.position)), out hitInfo, Mathf.Infinity, detectionMask);

        if (result)
        {
            print(hitInfo.collider.gameObject);
        }

        if(Vector3.Distance(transform.position, lastKnownLocation) < positionSearchRadius)
        {
            nma.isStopped = true;
            nma.ResetPath();
        }

        if (suspicionLevel == 100)
        {
            isDetected = true;
            nma.speed = detectedMovementSpeed;
        }


        /*
        if (isDetected)
        {
            nma.speed = 2f;
        }
        else
        {
            nma.speed = 1f;
        }
        */


        Debug.DrawRay(lastKnownLocation, Vector3.up*2, new Color(255,110,0));

        if(suspicionLevel > suspicionThreshold && hitInfo.collider == playerCollider)
        {
            if(eyeView.colliders.Contains(playerCollider) || peripheryView.colliders.Contains(playerCollider))
            {
                lastKnownLocation = player.transform.position;
                
                if (lastKnownLocation != Vector3.zero) {
                    if (!(Vector3.Distance(transform.position, lastKnownLocation) < positionSearchRadius))
                    {
                        nma.destination = lastKnownLocation;
                    }
                }
            }
        }
        else if(suspicionLevel < suspicionThreshold)
        {
            nma.speed = defaultMovementSpeed;
        }

        if (lastKnownLocation != Vector3.zero)
        {
            //StartCoroutine(TurnToLocation(lastKnownLocation, turnToLocationSpeed));
            //nma.
        }

        if (eyeView.colliders.Contains(playerCollider) && hitInfo.collider == playerCollider)
        {
            Debug.DrawRay(observerBox.transform.position, -(observerBox.transform.position - playerCollider.ClosestPoint(observerBox.transform.position)) * 10, Color.red);
            
            isDetected = true;
            suspicionLevel = 100;
        }
        else if (peripheryView.colliders.Contains(playerCollider) && hitInfo.collider == playerCollider)
        {
            Debug.DrawRay(observerBox.transform.position, -(observerBox.transform.position - playerCollider.ClosestPoint(observerBox.transform.position)) * 10, Color.blue);

            updateDetection(detectionSpeed * Time.deltaTime);
        }
        else
        {
            isDetected = false;
            updateDetection((-detectionSpeed * Time.deltaTime) / detectionSpeedLossDivider);
        }


    }

    public void SoundEvent(GameObject soundSource)
    {
        if (Vector3.Distance(transform.position, soundSource.transform.position) < hearingDistance)
        {
            lastKnownLocation = soundSource.transform.position;
            suspicionLevel = suspicionThreshold + 1;
        }
    }

    public void updateDetection(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel + amount, 0, 100);
    }

    public IEnumerator TurnToLocation(Vector3 playerPosition, float turnSpeed)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-(transform.position - playerPosition)), turnSpeed * Time.deltaTime);
        yield return null;
    }

}
