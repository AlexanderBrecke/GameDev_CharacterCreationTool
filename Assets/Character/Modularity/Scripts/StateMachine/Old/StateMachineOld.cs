using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachineOld : MonoBehaviour
{

    protected List<StateOld> stateList = new List<StateOld>();
    public StateOld startState;
    protected StateOld currentState;

    private void Start()
    {
        SetOrInitializeState(startState);
    }

    public StateOld GetCurrentState { get { return currentState; } }

    public virtual bool SetOrInitializeState(StateOld state)
    {
        bool success = false;
        if(state && state != currentState)
        {
            StateOld oldState = currentState;
            currentState = state;
            if (oldState)
                oldState.ExitState();
            if (stateList.Contains(currentState))
                currentState.EnterState();
            else
            {
                currentState.Initialize(this);
                stateList.Add(currentState);
            }
            success = true;
        }
        return success;
    }

    public virtual bool SetState(StateOld state)
    {
        //print(state.name);
        bool success = false;
        if(state && state != currentState)
        {
            StateOld oldState = currentState;
            currentState = state;
            if (oldState)
                oldState.ExitState();
            currentState.EnterState();
            success = true;
        }
        return success;
    }

    public void setState(StateOld state)
    {
        //print(state);
        SetState(state);
    }

    public virtual bool SetState<StateType>() where StateType : StateOld, new()
    {
        bool success;
        foreach (StateOld state in stateList)
        {
            if(state is StateType)
            {
                success = SetState(state);
                break;
            }
        }
        StateOld stateComponent = GetComponent<StateType>();
        if (stateComponent)
        {
            stateComponent.Initialize(this);
            stateList.Add(stateComponent);
            success = SetState(stateComponent);
            return success;
        }
        StateOld newState = gameObject.AddComponent<StateType>();
        newState.Initialize(this);
        stateList.Add(newState);
        success = SetState(newState);
        return success;
    }

    /*
    protected List<State> stateList = new List<State>();
    public State startingState;
    protected State currentState;

    private void Start()
    {
        SetState(startingState);
    }

    public State GetCurrentState { get { return currentState; } }

    public virtual bool SetState(State state)
    {
        bool success = false;
        if(state && state != currentState)
        {
            State oldState = currentState;
            currentState = state;
            if (oldState)
                oldState.ExitState();
            currentState.EnterState();
            success = true;
        }
        return success;
    }

    public void setState(State state)
    {
        SetState(state);
    }


    */

}
