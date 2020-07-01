using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVelocity : MonoBehaviour, IMoveVelocity
{
    private float moveSpeed;

    private Vector3 velocityVector;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetVelocity(Vector3 velocityVector, float speed = 15)
    {
        this.velocityVector = velocityVector;
        this.moveSpeed = speed;
    }

    private void FixedUpdate()
    {
        rb.velocity = velocityVector * moveSpeed;
    }
}
