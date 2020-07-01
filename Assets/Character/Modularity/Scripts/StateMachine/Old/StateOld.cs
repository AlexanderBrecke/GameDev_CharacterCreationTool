using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOld : MonoBehaviour
{

    public StateMachineOld machine;
    protected Character character;
    
    public static implicit operator bool(StateOld state)
    {
        return state != null;
    }

    public void Initialize(StateMachineOld machine)
    {
        this.machine = machine;
        character = GetComponentInParent<Character>();
        OnStateInitialize(machine);
        enabled = true;
        EnterState();
    }

    protected virtual void OnStateInitialize(StateMachineOld machine = null)
    {

    }

    public void EnterState()
    {
        enabled = true;
        OnStateEnter();
    }

    protected virtual void OnStateEnter()
    {

    }

    public void ExitState()
    {
        OnStateExit();
        enabled = false;
    }

    protected virtual void OnStateExit()
    {

    }
    public void OnEnable()
    {
        if(this != machine.GetCurrentState)
        {
            enabled = false;
            Debug.LogWarning("Do not enable States directly. Use StateMachine.SetState");
        }
    }

    public void OnDisable()
    {
        if(this == machine.GetCurrentState)
        {
            enabled = true;
            Debug.LogWarning("Do not disable States directly. Use StateMachine.SetState");
        }
    }

}
