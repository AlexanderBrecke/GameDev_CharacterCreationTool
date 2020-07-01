using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{

    public State_Idle(Character character) : base(character) { 
    }

    public override IEnumerator OnStateEnter()
    {
        Debug.Log(character.Name + " Entered Idle state");
        return base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        Debug.Log(character.Name + " Exited Idle state");
        return base.OnStateExit();
    }
}
