using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    public float verticalHeadBobAmplitude = 1f;
    public float horizontalHeadBobAmplitude = 1f;
    public float headBobSpeed = 1f;

    float headBobTime;

    Vector3 _cameraStartPos;
    StarterAssets.FirstPersonController fpc;

    // Start is called before the first frame update
    void Start()
    {
        _cameraStartPos = transform.localPosition;
        fpc = GetComponentInParent<StarterAssets.FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        headBobTime += Time.deltaTime * fpc.GetSpeed() / fpc.MoveSpeed;
        ApplyMovement(CalculateBob());
    }

    public Vector3 CalculateBob()
    {
        

        Vector3 head = new Vector3(0,0,0);
        if (fpc.Grounded)
        {
            head += Vector3.right * Mathf.Cos(headBobTime / headBobSpeed) * horizontalHeadBobAmplitude;
            head += Vector3.up * Mathf.Sin(2 * headBobTime / headBobSpeed) * verticalHeadBobAmplitude;
        }
        return head * fpc.GetSpeed() / fpc.MoveSpeed;
    }

    public void ApplyMovement(Vector3 movement)
    {
        transform.localPosition = _cameraStartPos + movement;
    }

}
