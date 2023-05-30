using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightSensor : MonoBehaviour
{
    EnemyAI enemyAI;

    public GameObject observer;

    TriggerReport peripheryView;
    TriggerReport eyeView;
    
    [SerializeField]
    GameObject player;
    public Collider playerCollider;

    public LayerMask detectionMask;

    void Start()
    {
        //player = enemyAI.player;

        playerCollider = player.GetComponent<CharacterController>();
        enemyAI = GetComponent<EnemyAI>();

        peripheryView = observer.transform.GetChild(1).gameObject.GetComponent<TriggerReport>();
        eyeView = observer.transform.GetChild(0).gameObject.GetComponent<TriggerReport>();
    }

    void Update()
    {
        RaycastHit hitInfo;
        var result = Physics.Raycast(observer.transform.position, -(observer.transform.position - playerCollider.ClosestPoint(observer.transform.position)), out hitInfo, Mathf.Infinity, detectionMask);

        if (eyeView.colliders.Contains(playerCollider))
        {
            if (hitInfo.collider == playerCollider)
            {
                Debug.DrawRay(observer.transform.position, -(observer.transform.position - playerCollider.ClosestPoint(observer.transform.position)) * 10, Color.red);
                
                enemyAI.UpdateLocationOfInterest(player.transform.position);
                enemyAI.OnPlayerSeen();
            }
            else
            {
                enemyAI.PlayerNotVisiblyDetected();
            }
        }
        else if (peripheryView.colliders.Contains(playerCollider))
        {
            if (hitInfo.collider == playerCollider)
            {
                Debug.DrawRay(observer.transform.position, -(observer.transform.position - playerCollider.ClosestPoint(observer.transform.position)) * 10, Color.blue);

                enemyAI.UpdateLocationOfInterest(player.transform.position);
                enemyAI.UpdateSuspicionLevel(enemyAI.suspicionDetectionSpeed * Time.deltaTime);
            }
            else
            {
                enemyAI.PlayerNotVisiblyDetected();
            }
        }
        else
        {
            enemyAI.PlayerNotVisiblyDetected();
        }
    }
}
