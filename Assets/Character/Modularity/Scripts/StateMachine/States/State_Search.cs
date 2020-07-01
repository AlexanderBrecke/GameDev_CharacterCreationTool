using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox.Misc;

public class State_Search : State
{
    NPC_CharacterController controller;
    bool searching = false;
    bool startTimer = false;
    
    float searchTime;
    float searchRadius;
    Vector3 lastKnownPosition;

    Vector3 lookingDirection = Vector3.zero;
    Vector3 left = Vector3.zero;
    Vector3 right = Vector3.zero;

    public State_Search(Character character, NPC_CharacterController controller, float searchTime, float searchRadius) : base(character)
    {
        this.controller = controller;
        this.searchTime = searchTime;
        this.searchRadius = searchRadius;
    }

    public override IEnumerator OnStateEnter()
    {
        lookingDirection = Vector3.zero;
        left = Vector3.zero;
        right = Vector3.zero;
        searching = true;
        controller.hasSearched = false;
        startTimer = false;

        

        int search = 0;

        bool goNext = false;
        bool lookedLeft = false;
        bool lookedRight = false;

        Vector3 nextSearchPoint = Vector3.zero;

        lastKnownPosition = controller.lastKnownPosition;

        
        Debug.Log(character.Name + " Entered Search state");
        character.anim.SetBool("IsWalking", true);

        float timer = searchTime;

        while (true)
        {


            //Go to the last known location of the target
            character.positionOrDirection = lastKnownPosition;
            if (!startTimer && MiscFunctions.IsLookingAtPosition(character.transform, lastKnownPosition, 3.5f))
                character.Move(character.baseSpeed, 0.2f);

            if (MiscFunctions.HasReachedDestination(character.transform.position, lastKnownPosition, 0.2f))
            {
                character.anim.SetBool("IsWalking", false);
                startTimer = true;
            }

            if (startTimer)
            {
                timer -= Time.fixedDeltaTime;


                // --- COULD THIS BE DONE WITHIN A FOR LOOP? If so, you could loop this as many times as you need and don't need to hardcode it ---
                if(search == 0)
                {

                    if(lookingDirection == Vector3.zero)
                        SetLookingDirections();
                    if (!lookedLeft && MiscFunctions.IsLookingAtPosition(character.transform, left, 3.5f))
                    {
                        yield return new WaitForSeconds(1f);
                        lookedLeft = true;
                    }
                    if (lookedLeft && !lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, right, 3.5f))
                    {
                        yield return new WaitForSeconds(1f);
                        lookedRight = true;
                    }
                    if (lookedLeft && lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, lookingDirection, 3.5f))
                    {
                        yield return new WaitForSeconds(1);
                        search = 1;
                        ResetLookingDirections();
                        lookedLeft = false;
                        lookedRight = false;
                        goNext = true;
                        //Debug.Log("Foo");
                    }
                }
                else if(search == 1)
                {
                    //Debug.Log("Something");
                    nextSearchPoint = setNextSearchPoint(nextSearchPoint);
                    character.positionOrDirection = nextSearchPoint;

                    if (goNext && MiscFunctions.IsLookingAtPosition(character.transform, nextSearchPoint, 3.5f))
                    {
                        character.anim.SetBool("IsWalking", true);
                        character.Move(character.baseSpeed, 0.2f);
                    }
                    if(MiscFunctions.HasReachedDestination(character.transform.position, nextSearchPoint, 0.2f))
                    {
                        character.anim.SetBool("IsWalking", false);
                        if(lookingDirection == Vector3.zero)
                            SetLookingDirections();
                        goNext = false;

                        if (!lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, right, 3.5f))
                        {
                            yield return new WaitForSeconds(1f);
                            lookedRight = true;
                        }
                        if (!lookedLeft && lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, left, 3.5f))
                        {
                            yield return new WaitForSeconds(1f);
                            lookedLeft = true;
                        }
                        if (lookedLeft && lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, lookingDirection, 3.5f))
                        {
                            yield return new WaitForSeconds(1);
                            search = 2;
                            ResetLookingDirections();
                            lookedLeft = false;
                            lookedRight = false;
                            nextSearchPoint = Vector3.zero;
                            goNext = true;
                        }
                    }
                }
                else if(search == 2)
                {
                    nextSearchPoint = setNextSearchPoint(nextSearchPoint);
                    character.positionOrDirection = nextSearchPoint;

                    if (goNext && MiscFunctions.IsLookingAtPosition(character.transform, nextSearchPoint, 3.5f))
                    {
                        character.anim.SetBool("IsWalking", true);
                        character.Move(character.baseSpeed, 0.2f);
                    }
                    if(MiscFunctions.HasReachedDestination(character.transform.position, nextSearchPoint, 0.2f))
                    {
                        character.anim.SetBool("IsWalking", false);
                        if (lookingDirection == Vector3.zero)
                            SetLookingDirections();
                        goNext = false;

                        if(!lookedLeft && MiscFunctions.IsLookingAtPosition(character.transform, left, 3.5f))
                        {
                            yield return new WaitForSeconds(1);
                            lookedLeft = true;
                        }
                        if(lookedLeft && !lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, right, 3.5f))
                        {
                            yield return new WaitForSeconds(1);
                            lookedRight = true;
                        }
                        if(lookedLeft && lookedRight && MiscFunctions.IsLookingAtPosition(character.transform, lookingDirection, 3.5f))
                        {
                            ResetLookingDirections();
                            break;
                        }
                    }
                }
                // --- ---


            }

            //look around the last known location of the target to see if we can find it.


            if (!searching || timer <= 0)
                break;

            yield return null;

        }

        controller.hasSearched = true;
        //Debug.Log("Going to another state");
        //character.GoToState(character.stateList[1]);

        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        searching = false;
        Debug.Log(character.Name + " Exited Search state");
        character.anim.SetBool("IsWalking", false);
        return base.OnStateExit();
    }

    void SetLookingDirections()
    {
        lookingDirection = character.transform.position + MiscFunctions.DirFromAngle(0,false, character.transform);
        left = character.transform.position + MiscFunctions.DirFromAngle(-35, false, character.transform);
        right = character.transform.position + MiscFunctions.DirFromAngle(35, false, character.transform);
    }

    void ResetLookingDirections()
    {
        lookingDirection = Vector3.zero;
        left = Vector3.zero;
        right = Vector3.zero;
    }

    Vector3 setNextSearchPoint(Vector3 nextSearchPoint)
    {
        if (nextSearchPoint == Vector3.zero)
            nextSearchPoint = lastKnownPosition + (Random.insideUnitSphere * searchRadius);
        nextSearchPoint.y = 0;
        return nextSearchPoint;
    }
}
