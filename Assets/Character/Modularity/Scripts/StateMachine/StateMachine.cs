using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected State state;
    protected State lastState;
    [HideInInspector]
    public State startState;

    private void Start()
    {
        SetState(startState);
    }

    public void SetState(State state)
    {
        if(state != GetCurrentState && GetCurrentState != null)
        {
            StartCoroutine(this.state.OnStateExit());
            this.state = state;
            StartCoroutine(this.state.OnStateEnter());
        } else if(GetCurrentState == null)
        {
            this.state = state;
            StartCoroutine(this.state.OnStateEnter());
        }

        /*lastState = this.state;
        //print(this.state);
        this.state = state;
        OnStateChanged();*/
    }

    void OnStateChanged()
    {
        if (lastState != null)
            StartCoroutine(lastState.OnStateExit());
        StartCoroutine(state.OnStateEnter());
    }

    public State GetCurrentState { get { return state; } }
}
