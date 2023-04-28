using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Stealth State Asset", menuName = "Stealth/Stealth State")]
public class StealthState : ScriptableObject
{
    public GlobalAlertState alertState;

    public float detectionLevel = 0;


    public void addDetection(float amount)
    {
        detectionLevel = Mathf.Clamp(detectionLevel + amount, 0, 100);
    }

    public void loseDetection(float amount)
    {
        detectionLevel = Mathf.Clamp(detectionLevel + amount, 0, 100);
    }

}

public enum GlobalAlertState{

    undetected,
    suspicious,
    alerted,
    active
}


