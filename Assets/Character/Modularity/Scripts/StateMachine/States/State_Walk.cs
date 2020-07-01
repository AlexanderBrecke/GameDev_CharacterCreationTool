using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox.Misc;

public class State_Walk : State
{
    public State_Walk(Character character) : base(character) { }

    bool stopMoving = false;

    public override IEnumerator OnStateEnter()
    {
        stopMoving = false;
        //Debug.Log("Entered Walk State");
        //Debug.Log(character.ReachedDestination());

        //character.Move();

        /*while (true)
        {
            if(character.movementType == Character.MovementType.MoveToPoint && !character.DestinationReached())
            {
                //Debug.Log("Entered Walk State");
                Debug.Log(character.positionOrDirection);
                SetCharacterPointOrVelocity();
                character.Move();
            }
            if(character.movementType == Character.MovementType.MoveToward && CheckMoveVelocity() != Vector3.zero)
            {
                Debug.Log("Entered Walk State");
                character.positionOrDirection = CheckMoveVelocity();
                //CheckMoveVelocity();
                //Debug.Log(character.positionOrDirection);
                character.Move();
            }

            break;
        }*/
        character.anim.SetBool("IsWalking", true);

        if(character.movementType == Character.MovementType.MoveToward)
        {
            character.positionOrDirection = character.transform.position;
            Debug.Log(character.Name + " Entered MoveToward walk state");
            while (CheckMoveVelocity() != Vector3.zero)
            {
                character.positionOrDirection = CheckMoveVelocity();
                if (character.isRunning)
                    character.Move(character.runSpeed);
                if (character.isSneaking)
                    character.Move(character.sneakSpeed);
                if(!character.isRunning && !character.isSneaking)
                    character.Move(character.baseSpeed);
                
                yield return null;
            }
        } else if(character.movementType == Character.MovementType.MoveToPoint)
        {
            Debug.Log(character.Name + " Entered MoveToPoint walk state");
            character.Move(character.baseSpeed);

            //if(Input.GetMouseButton(1))
            while (true)
            {
                //Debug.Log(character.DestinationReached());
                if (Input.GetMouseButton(1))
                {
                    character.positionOrDirection = SetPointToGoTo();
                }
                if(!Input.GetMouseButton(1) && character.DestinationReached())
                {
                    break;
                }
                character.Move(character.baseSpeed);
                //yield return null;
            }
        }

        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        Debug.Log(character.Name + " Exited Walk state");

        stopMoving = true;
        character.anim.SetBool("IsWalking", false);

        //This will need to be uncommented if you do not have the movePosition script on the character.
        /*if(character.movementType == Character.MovementType.MoveToward)
        {
            character.positionOrDirection = CheckMoveVelocity();
            character.Move();
        }*/

        return base.OnStateExit();
    }

    Vector3 SetPointToGoTo()
    {
        Vector3 pointToGoTo = MiscFunctions.FindMousePositionIn3DSpace();
        return pointToGoTo;
    }

    Vector3 SetCharacterPointOrVelocity()
    {
        Vector3 pointToGoTo = Vector3.zero;
        if (character.movementType == Character.MovementType.MoveToPoint && Input.GetMouseButton(1))
        {
            pointToGoTo = MiscFunctions.FindMousePositionIn3DSpace();
            //character.positionOrDirection = pointToGoTo;
        }
        else if (character.movementType == Character.MovementType.MoveToward)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput).normalized;
            if (moveVector != Vector3.zero)
                pointToGoTo = moveVector;
            else
                pointToGoTo = character.transform.position;
        }
        character.positionOrDirection = pointToGoTo;
        return pointToGoTo;
    }

    Vector3 CheckMoveVelocity()
    {
        Vector3 movementVector = Vector3.zero;
        
        if (!stopMoving)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            movementVector = new Vector3(horizontalInput, 0, verticalInput).normalized;
        }

        return movementVector;
    }


    Vector3 FindWhereToGo()
    {
        Vector3 pointToGoTo = Vector3.zero;
        if (character.movementType == Character.MovementType.MoveToPoint && Input.GetMouseButton(1))
        {
            pointToGoTo = MiscFunctions.FindMousePositionIn3DSpace();
            return pointToGoTo;
        }
        else if (character.movementType == Character.MovementType.MoveToward)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput).normalized;
            pointToGoTo = character.transform.position;
            return moveVector;
        }
        return pointToGoTo;
    }

}
