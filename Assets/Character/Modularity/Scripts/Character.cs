using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox;
using ToolBox.Misc;

public class Character: Character_Base
{
    public string Name;
    public enum MovementType { MoveToward, MoveToPoint}
    [HideInInspector]
    public MovementType movementType;
    public enum CharacterType { Player, NPC}
    //[HideInInspector]
    public CharacterType characterType;

    public int maxHealth;


    //public int strength;
    public int agility;
    //public int intelligence;


    [HideInInspector]
    public float attackSpeed;
    [HideInInspector]
    public float attackRate;


    public float baseSpeed;
    [HideInInspector]
    public float sneakSpeed;
    [HideInInspector]
    public float runSpeed;
    [HideInInspector]
    public float radiusToReachTarget = 0.2f;

    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public bool isSneaking = false;

    [HideInInspector]
    public bool canAttack = true;
    [HideInInspector]
    public bool hasAttacked = false;

    [HideInInspector]
    public bool canInteract = false;
    [HideInInspector]
    public bool hasInteracted = false;
    [HideInInspector]
    public GameObject interactionObject = null;


    [HideInInspector]
    public Vector3 positionOrDirection;

    [HideInInspector]
    public Transform currentTarget;

    [HideInInspector]
    public List<State> stateList;
    [HideInInspector]
    public StateMachine stateMachine;

    // Animation stuff
    [HideInInspector]
    public Animator anim;


    // ---


    //For purposes of getting attacking before items are implemented

    public float attackRange;
    public Transform attackPoint;
    public Transform mainHand;
    public Transform offHand;
    

    public GameObject equippedMainHand;
    //public GameObject equippedWeapon;
    public float weaponAttackRadius = 1;
    public int weaponDamage = 10;

    // ---

    //[HideInInspector]
    //public bool shouldMove = false;


    private void Awake()
    {
        agility = statBase;
        //attackSpeed = agility * 0.1f;
        attackSpeed = 0.5f;
        attackRate = attackSpeed;
        //print($"{name}'s attackrate is {attackRate}");
        if(baseSpeed == 0)
            baseSpeed = speedBase;
        runSpeed = baseSpeed * 1.5f;
        sneakSpeed = baseSpeed * 0.5f;
        if(GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();
    }

    public void GoToState(State stateToGoTo)
    {
        if (stateMachine.GetCurrentState != stateToGoTo)
            stateMachine.SetState(stateToGoTo);
    }

    public void Move(float speed, float radiusToReach = 3f)
    {
        SetReachedRadius(radiusToReach);

        switch (movementType)
        {
            case MovementType.MoveToward:
                //This will need to be commented out if the "MovePosition" script is not on the character
                StopUsingMovePosition();
                // ---

                MoveToward(positionOrDirection, speed);
                break;
            case MovementType.MoveToPoint:
                MoveToPosition(positionOrDirection, speed);
                break;
            default:
                break;
        }

        /*if (movementType == MovementType.MoveToPoint)
        {

            MoveToPosition(positionOrDirection, speed);
        }
        else if (movementType == MovementType.MoveToward)
        {
            //This will need to be commented out if the movePosition script is not on the character
            StopUsingMovePosition();
            // ---

            MoveToward(positionOrDirection, speed);
        }*/
    }

    public void Move(Vector3 direction, float speed)
    {
        MoveToward(direction, speed);
    }

    public void StopMove()
    {
        switch (movementType)
        {
            case MovementType.MoveToward:
                StopUsingMovePosition();
                MoveToward(Vector3.zero, 0);
                break;
            case MovementType.MoveToPoint:
                MoveToPosition(this.transform.position, 0);
                break;
            default:
                break;
        }
    }

    public bool DestinationReached() => IsPositionReached(radiusToReachTarget);
    public void SetTheReachedRadius(float radius = 0.2f) => SetReachedRadius(radius);

    public void DoAttack(Transform attackPoint, float radius, int damage)
    {
        Attack(attackPoint, radius, damage);
    }

    public void Interaction(Character interactingCharacter, GameObject interactedObject)
    {
        Interact(interactingCharacter, interactedObject);
    }

    public void StartAttackCooldown()
    {
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        //print("Started attack cooldown");
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
        //print("Finished attack cooldown");
        yield break;
    }

    public void DoMoveForSetTime(Vector3 direction, float speed = 7.5f, float time = 1)
    {
        StartCoroutine(MoveForSetTime(direction, speed, time));
    }

    IEnumerator MoveForSetTime(Vector3 direction, float speed = 7.5f, float time = 1)
    {
        float t = 0;
        //print(direction);
        while (true)
        {
            //print("FOO");
            t += Time.deltaTime;
            Move(direction, speed);

            if (t >= time)
                break;

            yield return null;
        }
        yield break;
    }

    /*public void DestinationReached()
    {
        DestinationReached(true);
    }

    public void ResetDestinationReached()
    {
        DestinationReached(false);
    }

    public bool HasReachedDestination() => reachedDestination;*/




    /*public int strength;
    public int agility;
    public int intelligence;
    public float speed;
    GameObject characterObject;


    public Character(int strMod, int agiMod, int intMod, float speedMod, GameObject characterObject)
    {
        this.strength = baseStat + strMod;
        this.agility = baseStat + agiMod;
        this.intelligence = baseStat + intMod;
        this.speed = baseStat + speedMod;
        this.characterObject = characterObject;
    }

    public override void Move(Vector3 positionOrDirection, float speed)
    {
        base.Move(positionOrDirection,speed);
        if(characterObject.GetComponent<IMovePosition>() != null)
        {
            characterObject.GetComponent<IMovePosition>().SetMovePosition(positionOrDirection, speed);
        } else
        {
            characterObject.GetComponent<IMoveVelocity>().SetVelocity(positionOrDirection, speed);
        }
    }*/





}
