using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox.Tools;
using ToolBox.Misc;


public class NPC_CharacterController : MonoBehaviour
{
    
    [HideInInspector]
    public enum FriendOrFoe { friendly, enemy};
    public FriendOrFoe friendOrFoe;
    Character.MovementType movementType;
    
    public State_Patrol.PatrolType patrolType;

    Character character;
    HealthHandler healthHandler;

    public GameObject patrolPointsObject;
    public float roamRadius = 1;
    List<Vector3> patrolPoints = new List<Vector3>();

    public bool usesFieldOfView = false;
    FieldOfView fow;
    public Vector3 viewRadiusAnglesResolution = Vector3.zero;
    public LayerMask[] obstaclesAndTargetMasks = new LayerMask[2];
    public MeshFilter fowFilter = null;


    public List<Transform> targets;
    [HideInInspector]
    public Vector3 lastKnownPosition;
    bool shouldSearch = false;
    [HideInInspector]
    public bool hasSearched = false;
    State stateToReturnTo;

    StateMachine machine;


    [SerializeField]
    Transform interactionTransform = null;
    [SerializeField]
    float interactionRadius = 2;
    [HideInInspector]
    GameObject interacter;



    private void Awake()
    {
        character = GetComponent<Character>();
        healthHandler = GetComponent<HealthHandler>();
        healthHandler.SetMaxHealth(character.maxHealth);

        movementType = Character.MovementType.MoveToPoint;
        character.movementType = movementType;
        

        foreach (Transform point in patrolPointsObject.GetComponentInChildren<Transform>())
        {
            patrolPoints.Add(point.position);
        }

        machine = GetComponentInChildren<StateMachine>();
        character.stateList = new List<State>()
        {
            new State_Idle(character),
            new State_Patrol(character, patrolPoints, patrolPointsObject.transform, roamRadius, patrolType),
            new State_Chase(character),
            new State_Search(character, this, 100f, 10f),
            new State_Attack(character),
            new State_Interact(character),
        };


        character.stateMachine = machine;
        character.stateMachine.startState = character.stateList[0];
        //machine.SetState(stateList[1]);

    }

    private void Start()
    {
        if (usesFieldOfView)
        {
            //obstaclesAndTargetMasks = new LayerMask[2] { 1 << 11, 1 << 10 };
            
            if (viewRadiusAnglesResolution != Vector3.zero)
                fow = new FieldOfView(viewRadiusAnglesResolution, transform, fowFilter, obstaclesAndTargetMasks);
            else
                fow = new FieldOfView(transform, fowFilter, obstaclesAndTargetMasks);

            StartCoroutine(fow.FindTargetWithDelay());

        }

        MiscFunctions.DelayedFunction(gameObject, 0.2f, () => character.GoToState(character.stateList[1]));

    }

    private void Update()
    {
        if (character.equippedMainHand == null && character.equippedMainHand != character.mainHand.gameObject)
        {
            character.equippedMainHand = character.mainHand.gameObject;
        }

        SetTheTarget();
        StateMachineLogic();
        SetCanBeInteractedWith();

        //if (character.currentTarget != null)
        //{
        //    if (character.stateMachine.GetCurrentState != character.stateList[2] && machine.GetCurrentState != character.stateList[3])
        //        stateToReturnTo = machine.GetCurrentState;
        //    if(machine.GetCurrentState != character.stateList[4])
        //        character.GoToState(character.stateList[2]);



        //    //machine.SetState(stateList[2]);
        //    //print(character.lastState);
        //}
        //if (character.currentTarget == null && shouldSearch)
        //{
        //    character.GoToState(character.stateList[3]);
        //    //machine.SetState(stateList[3]);
        //    shouldSearch = false;
        //}
        //if(machine.GetCurrentState == character.stateList[3] && hasSearched)
        //{
        //    foreach (State state in character.stateList)
        //    {
        //        if (state == stateToReturnTo)
        //        {
        //            GoToState(state);
        //            //machine.SetState(state);
        //            break;
        //        }
        //    }
        //    //hasSearched = false;
        //}

        
    }


    void StateMachineLogic()
    {
        //Enemy logic
        //print(character.stateList[5]);
        if(friendOrFoe == FriendOrFoe.enemy)
        {
            //if(FindObjectOfType<Player_CharacterController>() == null)
            //{
            //    character.GoToState(character.stateList[0]);
            //}

            if(character.currentTarget != null)
            {
                if (character.stateMachine.GetCurrentState != character.stateList[4])
                    character.GoToState(character.stateList[2]);
            }
            else if(character.currentTarget == null && character.stateMachine.GetCurrentState == character.stateList[4])
            {
                character.GoToState(character.stateList[3]);
            }
            if(character.stateMachine.GetCurrentState == character.stateList[2] && character.currentTarget == null && shouldSearch)
            {
                character.GoToState(character.stateList[3]);
                shouldSearch = false;
            }
            else if(character.stateMachine.GetCurrentState == character.stateList[3] && hasSearched || character.stateMachine.GetCurrentState == character.stateList[5] && character.hasInteracted)
            {
                character.GoToState(character.stateList[1]);
            }
            if (character.stateMachine.GetCurrentState == character.stateList[0] && character.currentTarget != null && Vector3.Distance(character.currentTarget.position, transform.position) < character.attackRange && character.canAttack)
            {
                character.GoToState(character.stateList[4]);
            }
            else if (character.stateMachine.GetCurrentState == character.stateList[0] && character.currentTarget == null && lastKnownPosition != Vector3.zero)
            {
                character.GoToState(character.stateList[3]);
                //print(lastKnownPosition);
            }

            
        }

        // ---
    }

    private void LateUpdate()
    {
        if(usesFieldOfView)
            UpdateFieldOfView();
    }

    //void GoToState(State stateToGoTo)
    //{
    //    if (machine.GetCurrentState != stateToGoTo)
    //        machine.SetState(stateToGoTo);
    //}

    void SetTheTarget()
    {
        if (targets.Count != 0 && targets[0] != null)
        {
            character.currentTarget = targets[0];
            lastKnownPosition = targets[0].transform.position;
            shouldSearch = true;
            //print(shouldSearch);
            //hasSearched = false;
        }
        else
        {
            character.currentTarget = null;
        }
    }

    void SetCanBeInteractedWith()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        

        foreach (var item in Physics.OverlapSphere(interactionTransform.position, interactionRadius))
        {
            if (item.GetComponent<Character>() != null && item.gameObject.tag == "Player")
            {
                interacter = item.gameObject;
                item.GetComponent<Character>().canInteract = true;
                item.GetComponent<Character>().interactionObject = gameObject;
            }
        }

        if (interacter != null)
        {
            if (Vector3.Distance(interactionTransform.position, interacter.transform.position) > interactionRadius)
            {
                interacter.GetComponent<Character>().canInteract = false;
                interacter.GetComponent<Character>().interactionObject = null;
                interacter = null;
            }
        }
    }

    public void Interacting()
    {
        character.GoToState(character.stateList[5]);
    }

    public void HasBeenAttacked(Character attacker)
    {

        //StartCoroutine(LookAtAttacker(attacker));
        lastKnownPosition = attacker.transform.position;
        if (character.stateMachine.GetCurrentState != character.stateList[3])
            character.GoToState(character.stateList[3]);
        else
        {
            character.GoToState(character.stateList[0]);
            character.GoToState(character.stateList[3]);
        }

        print($"I am being attacked by {attacker.name}, and should defend myself");
    }




    void UpdateFieldOfView()
    {
        targets = fow.visibleTargets;
        fow.UpdateFieldOfView(viewRadiusAnglesResolution);

        //SetTheTarget();

        foreach (var target in targets)
        {
            if(target != null)
                Debug.DrawLine(transform.position, target.position);
        }
    }

    IEnumerator LookAtAttacker(Character attacker)
    {
        while (true)
        {
            Debug.Log("FOO");
            if (MiscFunctions.IsLookingAt(transform, attacker.transform.position))
                break;
            yield return null;

        }
        yield break;
    }




}
