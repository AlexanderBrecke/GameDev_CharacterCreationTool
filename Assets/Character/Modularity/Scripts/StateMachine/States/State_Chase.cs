using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox.Misc;

public class State_Chase : State
{
    Transform targetToChase;
    bool chasing = false;

    public State_Chase(Character character) : base(character)
    {
    }

    public override IEnumerator OnStateEnter()
    {
        Debug.Log(character.Name + " Entered Chase state");
        character.anim.SetBool("IsWalking", true);
        chasing = true;
        targetToChase = character.currentTarget;

        while (true)
        {
            if (!chasing)
                break;
            else
            {
                character.positionOrDirection = targetToChase.position;
                MiscFunctions.LookAtTarget(character.transform, targetToChase);
                character.Move(character.baseSpeed);
                if (Vector3.Distance(character.transform.position, targetToChase.position) <= character.attackRange && character.canAttack)
                    character.stateMachine.SetState(character.stateList[4]);

                if (targetToChase == null)
                    chasing = false;
            }


            yield return null;
        }



        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        chasing = false;
        //character.positionOrDirection = character.transform.position;
        character.Move(0);
        Debug.Log(character.Name + " Exited Chase state");
        character.anim.SetBool("IsWalking", false);
        return base.OnStateExit();
    }
}
