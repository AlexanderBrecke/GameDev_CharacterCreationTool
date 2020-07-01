using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovePosition
{
    void SetMovePosition(Vector3 movePosition, float speed);
    bool HasReachedDestination(float radius);
    void StopUsingMovePosition();
    void SetDestinationReachedRadius(float radius);

}
