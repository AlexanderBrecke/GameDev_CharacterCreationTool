using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableProp : MonoBehaviour
{

    public float radius = 1f;
    public Transform interactionTransform;
    //bool hasInteraction = true;
    GameObject interacter;

    private void Update()
    {
        foreach (var item in Physics.OverlapSphere(interactionTransform.position,radius))
        {
            if(item.GetComponent<Character>() != null && item.gameObject.tag == "Player")
            {
                interacter = item.gameObject;
                item.GetComponent<Character>().canInteract = true;
                item.GetComponent<Character>().interactionObject = gameObject;
            }

        }

        if(interacter != null)
        {
            if(Vector3.Distance(interactionTransform.position, interacter.transform.position) > radius)
            {
                interacter.GetComponent<Character>().canInteract = false;
                interacter.GetComponent<Character>().interactionObject = null;
                interacter = null;
            }
        }
        
    }



    private void OnDrawGizmos()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }


}
