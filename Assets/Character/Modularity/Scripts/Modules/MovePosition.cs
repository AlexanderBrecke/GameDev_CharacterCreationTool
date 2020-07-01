using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePosition : MonoBehaviour, IMovePosition
{
    private Vector3 movePosition;
    private float radius = 0.2f;
    private float movementSpeed;
    private bool useMovePosition;

    public void SetMovePosition(Vector3 movePosition, float speed)
    {
        useMovePosition = true;
        this.movePosition = movePosition;
        this.movementSpeed = speed;
    }

    public void SetDestinationReachedRadius(float radius)
    {
        this.radius = radius;
    }

    public void StopUsingMovePosition()
    {
        useMovePosition = false;
    }

    public bool HasReachedDestination(float radius)
    {
        bool reached = false;
        if(Vector3.Distance(transform.position, movePosition) <= radius)
            reached = true;
        //print(reached + " position");
        return reached;
    }

    private void Update()
    {
        //Vector3 moveDirection = (movePosition - transform.position).normalized;
        //GetComponent<IMoveVelocity>().SetVelocity(moveDirection, movementSpeed);
        /*if(Vector3.Distance(transform.position, movePosition) > radius)
        {
            Vector3 moveDirection = (movePosition - transform.position).normalized;
            GetComponent<IMoveVelocity>().SetVelocity(moveDirection, movementSpeed);
        }
        else
            GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero, movementSpeed);*/
        if (!HasReachedDestination(radius) && useMovePosition)
        {
            Vector3 moveDirection = (movePosition - transform.position).normalized;
            GetComponent<IMoveVelocity>().SetVelocity(moveDirection, movementSpeed);
        }
        else
        {
            GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero, movementSpeed);
            //destinationReached = true;
        }
    }
}
