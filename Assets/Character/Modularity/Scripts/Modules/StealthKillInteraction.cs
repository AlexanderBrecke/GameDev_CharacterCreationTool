using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthKillInteraction : MonoBehaviour, IInteract
{

    bool finished = false;
    bool interacting = false;

    IEnumerator Something(Character interactingCharacter, GameObject objectBeingInteractedWith)
    {
        //interacting = true;
        //finished = false;
        yield return new WaitForSeconds(0.5f);
        if(objectBeingInteractedWith.GetComponent<HealthHandler>() != null)
        {
            objectBeingInteractedWith.GetComponent<HealthHandler>().ChangeHealth(-objectBeingInteractedWith.GetComponent<HealthHandler>().currentHealth, interactingCharacter);
        }
        //print("FOO");
        finished = true;
        yield break;
    }
    public bool FinishedInteracting()
    {
        return finished;
    }

    public void Interact(Character interactingCharacter, GameObject objectBeingInteractedWith)
    {
        print("BAR");
        interacting = true;
        finished = false;
        StartCoroutine(Something(interactingCharacter, objectBeingInteractedWith));
    }

    public bool IsInteracting()
    {
        return interacting;
    }

    void IInteract.SetFinished(bool finished)
    {
        this.finished = finished;
    }

    void IInteract.SetInteracting(bool interacting)
    {
        this.interacting = interacting;
    }
}
