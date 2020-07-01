using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Interact : State
{
    GameObject objectToInteractWith;
    bool interacting = false;
    public State_Interact(Character character) : base(character)
    {
        
        //Debug.Log(objectToInteractWith.name);
    }

    public override IEnumerator OnStateEnter()
    {

        objectToInteractWith = character.interactionObject;
        Debug.Log(character.Name + " Entered Interaction state");
        interacting = true;

        

        while (true)
        {
            if(character.characterType == Character.CharacterType.Player)
            {
                if (character.interactionObject != null)
                {
                    //yield return new WaitForSeconds(0.5f);
                    if (!objectToInteractWith.GetComponent<IInteract>().IsInteracting() && !objectToInteractWith.GetComponent<IInteract>().FinishedInteracting())
                        objectToInteractWith.GetComponent<IInteract>().Interact(character, objectToInteractWith);

                    //yield return new WaitForSeconds(1);
                    if (objectToInteractWith.GetComponent<IInteract>().FinishedInteracting())
                        interacting = false;
                }
                else
                    interacting = false;
            } else if(character.characterType == Character.CharacterType.NPC)
            {
                //character.positionOrDirection = character.transform.position;
                character.Move(0);
                //yield return new WaitForSeconds(1);
                if (character.gameObject.GetComponent<IInteract>().FinishedInteracting())
                {
                    //yield return new WaitForSeconds(3);
                    interacting = false;
                }
            }


            if (!interacting)
                break;

            yield return null;

        }
        character.hasInteracted = true;
        
        base.OnStateEnter();
    }

    public override IEnumerator OnStateExit()
    {
        Debug.Log(character.Name + " Exited Interaction state");
        if (character.characterType == Character.CharacterType.Player)
        {
            if (objectToInteractWith != null)
            {
                objectToInteractWith.GetComponent<IInteract>().SetFinished(false);
                objectToInteractWith.GetComponent<IInteract>().SetInteracting(false);
            }
        } else if(character.characterType == Character.CharacterType.NPC)
        {
            character.GetComponent<IInteract>().SetFinished(false);
            character.GetComponent<IInteract>().SetInteracting(false);

        }

        character.hasInteracted = false;
        return base.OnStateExit();
    }
}
