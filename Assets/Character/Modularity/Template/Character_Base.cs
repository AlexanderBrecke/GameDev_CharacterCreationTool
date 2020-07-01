using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Base : MonoBehaviour
{

    protected int statBase = 10;
    protected float speedBase = 7.5f;
    //protected bool reachedDestination = false;


    // --- Look at something ---

    /*protected void LookAt(Vector3 whereToLook)
     {
         float speed = 7.5f;
         Quaternion targetRotation = Quaternion.LookRotation(whereToLook - transform.position);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
     }*/

     // --- ---

     // --- Functions for modules ---
     protected void MoveToPosition(Vector3 positionToMoveTo, float speed)
     {
        //reachedDestination = false;
        GetComponent<IMovePosition>().SetMovePosition(positionToMoveTo, speed);
     }

    protected bool IsPositionReached(float radius = 0.2f) => GetComponent<IMovePosition>().HasReachedDestination(radius);
    protected void SetReachedRadius(float radius) => GetComponent<IMovePosition>().SetDestinationReachedRadius(radius);

    protected void StopUsingMovePosition() => GetComponent<IMovePosition>().StopUsingMovePosition();

     protected void MoveToward(Vector3 direction, float speed)
     {
        //reachedDestination = false;
        GetComponent<IMoveVelocity>().SetVelocity(direction, speed);
     }

    protected void Attack(Transform attackPoint, float radius, int damage)
    {
        GetComponent<IAttack>().Attack(attackPoint, radius, damage);
    }

    protected void Interact(Character interactingCharacter, GameObject objectInteractedWith)
    {
        GetComponent<IInteract>().Interact(interactingCharacter, objectInteractedWith);
    }
    // --- ---



    /* How to make a good character template
    
    This should function somewhat like an API?
    It needs to abstract all the things a character needs.

    We will need a library to implement the actual functionality.
    The library should explain different character behaviours.

    Do we make a library of character types? If so, do we need a second one for the behaviours?
    Or do the behaviours also set the types of characters?

    For instance, we want to be able to make a human character and a troll character. Should both of these be able to function as player characters?
    How would we make this work?


    Firstly, we will need to define an abstract character.
    These will need some variables for their stats.
    Then we will need to define functionality.

    Do I define character type in the template, and then modify things depending on the type there?
    That way, when I create a character, I could just choose the type of character and the implementation wil be done for me?
    For instance, if a troll has +2 to their strength stat, as well as a dark-vision thing, when I create a character I can just say I create a troll character
    then the +2 will be added to the stregnth stat and a dark-vision will be implemented when the character is created?

    class characterTemplate {

    enum characterType {troll, human}

    strengthBase = 10
    agibase = 10
    intbase = 10

    trollStrengthMod = +2
    trollAgiMod = 0
    trollIntMod = -2

    humanStrMod = 0
    humanAgiMod = 0
    humanIntMod = 0

    Movement function()
    Attack function()

    }

    class character: characterTemplate {

    

    public Character(characterType type) {
    this.character.someBase = someBase + type.someMod
    ^ Need some way for this to be able to happen.
    Need to be able to link a characterType with modifiers for different stats!

    }


    }


    This allows us to actualize these abstract characters in a different script (Library? or is this the controller?)
    Here we will define how the actual character implements the things from the abstract class.



    we will need a state machine as well as different states that dictates what kind of behaviours exist.
    Then we need the controllers that tie everything together and give the actual implementation of characters in the game.


    */
}
