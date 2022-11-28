using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Properties")]
    public float moveSpeed;
    public float jumpSpeed;

    [Header("Debug Values")]
    [SerializeField]
    Vector2 movementValue;

    float jumpVal = 0;

    /* [Misc] */
    public CharacterController cc;
    public PlayerInput pi;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        print(pi.currentActionMap.FindAction("Jump").ReadValue<float>());
        Move();
    }

    private void Move()
    {
        Vector3 move = transform.forward * movementValue.y + transform.right * movementValue.x;
        var hMove = moveSpeed * move * Time.deltaTime;

        if (!cc.isGrounded)
            hMove -= new Vector3(0, -Physics.gravity.y * Time.deltaTime, 0);
        else
            hMove.y = 0;

        if (cc.isGrounded && pi.currentActionMap.FindAction("Jump").triggered)
            hMove.y = Mathf.Sqrt(2 * jumpSpeed * Mathf.Abs(Physics2D.gravity.y));

        cc.Move(hMove);
    }

    void OnMove(InputValue value)
    {
        movementValue = value.Get<Vector2>();
    }

}
