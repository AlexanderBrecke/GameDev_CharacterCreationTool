using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox;
using ToolBox.Misc;

public class State_Patrol : State
{
    public enum PatrolType { Circular, ForthAndBack, Roam}
    PatrolType patrolType;

    Transform roamTransform;
    float roamRadius;

    List<Vector3> patrolPoints;

    Vector3 currentPatrolPoint;
    Vector3 lastPatrolPoint;
    Vector3 nextPatrolPoint;
    bool patrolling = false;

    

    public State_Patrol(Character character, List<Vector3> patrolPoints, Transform roamTransform, float roamRadius, PatrolType patrolType) : base(character)
    {
        this.patrolType = patrolType;
        this.patrolPoints = patrolPoints;
        this.roamTransform = roamTransform;
        this.roamRadius = roamRadius;
        if (lastPatrolPoint == null)
            lastPatrolPoint = this.patrolPoints[0];

    }

    public override IEnumerator OnStateEnter()
    {
        Debug.Log(character.Name + " Entered Patrol state");
        character.anim.SetBool("IsWalking", true);
        patrolling = true;
        if (currentPatrolPoint == null)
            currentPatrolPoint = patrolPoints[0];

        Vector3 nextPointToGoTo = Vector3.zero;

        while (true)
        {
            if (patrolType != PatrolType.Roam)
            {
                foreach (Vector3 position in patrolPoints)
                {
                    if (Vector3.Distance(character.transform.position, position) <= character.radiusToReachTarget)
                        currentPatrolPoint = position;
                }
                if (Vector3.Distance(character.transform.position, currentPatrolPoint) > character.radiusToReachTarget)
                {
                    lastPatrolPoint = currentPatrolPoint;
                }

                //Find the next patrol point
                if (nextPatrolPoint == null || Vector3.Distance(character.transform.position, nextPatrolPoint) <= character.radiusToReachTarget)
                {
                    character.anim.SetBool("IsWalking", false);
                    FindNextPatrolPoint();
                    character.positionOrDirection = nextPatrolPoint;

                    //Make the patroller look at the next patrol point before doing anything else
                    while (true)
                    {
                        if (MiscFunctions.IsLookingAtPosition(character.transform, nextPatrolPoint, 3.5f))
                        {
                            character.anim.SetBool("IsWalking", true);
                            break;
                        }
                        yield return null;
                    }
                    // ---

                    //Wait a little bit
                    yield return new WaitForSeconds(0.2f);
                    // ---
                }

                if (character.positionOrDirection != nextPatrolPoint)
                {
                    while (true)
                    {
                        if (MiscFunctions.IsLookingAtPosition(character.transform, nextPatrolPoint, 3.5f))
                            break;

                        yield return null;
                    }
                    character.positionOrDirection = nextPatrolPoint;
                    yield return new WaitForSeconds(0.2f);
                }

                if(!MiscFunctions.HasReachedDestination(character.transform.position, nextPatrolPoint, character.radiusToReachTarget))
                {
                    character.Move(character.baseSpeed, 0.2f);
                }

            } else
            {
                
                if(nextPointToGoTo == Vector3.zero || MiscFunctions.HasReachedDestination(character.transform.position ,nextPointToGoTo, character.radiusToReachTarget))
                {
                    character.anim.SetBool("IsWalking", false);
                    nextPointToGoTo = FindNextPatrolPoint();
                    character.positionOrDirection = nextPointToGoTo;
                }

                if (!MiscFunctions.HasReachedDestination(character.transform.position, nextPointToGoTo, character.radiusToReachTarget) && MiscFunctions.IsLookingAtPosition(character.transform, nextPointToGoTo, 3.5f))
                {
                    character.anim.SetBool("IsWalking", true);
                    character.Move(character.baseSpeed, 0.2F);
                }
            }



            yield return null;
            if(!patrolling)
                break;


        }

        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        patrolling = false;
        Debug.Log(character.Name + " Exited Patrol state");
        character.anim.SetBool("IsWalking", false);
        return base.OnStateExit();
    }


    Vector3 FindNextPatrolPoint()
    {

        switch (patrolType)
        {
            case PatrolType.Circular:
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    if (currentPatrolPoint == patrolPoints[i])
                    {
                        if (currentPatrolPoint != patrolPoints[patrolPoints.Count - 1] && patrolPoints[i + 1] != lastPatrolPoint || currentPatrolPoint == patrolPoints[0])
                        {
                            nextPatrolPoint = patrolPoints[i + 1];
                        }
                        else
                        {
                            nextPatrolPoint = patrolPoints[0];
                        }
                    }
                }
                break;
            case PatrolType.ForthAndBack:
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    if (currentPatrolPoint == patrolPoints[i])
                    {
                        if (currentPatrolPoint != patrolPoints[patrolPoints.Count - 1] && patrolPoints[i + 1] != lastPatrolPoint || currentPatrolPoint == patrolPoints[0])
                        {
                            nextPatrolPoint = patrolPoints[i + 1];
                        }
                        else
                        {
                            nextPatrolPoint = patrolPoints[i - 1];
                        }

                    }
                }
                break;
            case PatrolType.Roam:
                //Random patrol seems to be pretty unstable and should not be used as it stands. ---------------------------
                //int rand = Random.Range(0, patrolPoints.Count);
                Vector3 rand = roamTransform.position + Random.insideUnitSphere * roamRadius;
                rand.y = 0;
                nextPatrolPoint = rand;

                /*if (currentPatrolPoint.transform == patrolPoints[rand])
                    rand = Random.Range(0, patrolPoints.Count);
                //Debug.Log(rand);
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    if (currentPatrolPoint.transform != patrolPoints[rand])
                        nextPatrolPoint = patrolPoints[rand];
                }*/
                //----------------------------------------------------------------------
                break;
            default:
                break;
        }
        return nextPatrolPoint;
    }

}
