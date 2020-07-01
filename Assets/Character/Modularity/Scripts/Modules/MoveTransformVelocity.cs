using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransformVelocity : MonoBehaviour, IMoveVelocity
{
    private float moveSpeed = 15;

    private Vector3 velocityVector;

    

    public void SetVelocity(Vector3 velocityVector, float speed)
    {
        this.velocityVector = velocityVector;
        this.moveSpeed = speed;
    }

    private void Update()
    {
        transform.position += velocityVector * moveSpeed * Time.deltaTime;
    }
}
