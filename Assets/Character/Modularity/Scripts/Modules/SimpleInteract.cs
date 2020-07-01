using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteract : MonoBehaviour, IInteract
{

    bool finished = false;
    bool interacting = false;
    public void Interact(Character interactingCharacter, GameObject objectInteractedWith)
    {
        interacting = true;
        finished = false;
        Debug.Log($"{interactingCharacter.name} is interacting with {objectInteractedWith.name}");
        finished = true;
        interacting = false;
    }

    public bool FinishedInteracting()
    {
        return finished;
    }

    public bool IsInteracting()
    {
        return interacting;
    }

    public void SetFinished(bool finished)
    {
        throw new System.NotImplementedException();
    }

    public void SetInteracting(bool interacting)
    {
        throw new System.NotImplementedException();
    }
}
