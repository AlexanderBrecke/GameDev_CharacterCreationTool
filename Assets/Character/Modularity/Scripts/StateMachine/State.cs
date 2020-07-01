using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Character character;

    public State(Character character)
    {
        this.character = character;
    }

    public virtual IEnumerator OnStateEnter()
    {
        yield break;
    }

    public virtual IEnumerator OnStateExit()
    {
        yield break;
    }

}
