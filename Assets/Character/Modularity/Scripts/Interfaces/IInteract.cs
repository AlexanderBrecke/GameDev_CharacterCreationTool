using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteract 
{
    void Interact(Character interactingCharacter, GameObject objectBeingInteractedWith);
    bool FinishedInteracting();
    bool IsInteracting();
    void SetFinished(bool finished);
    void SetInteracting(bool interacting);
}
