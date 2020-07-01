using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolBox.Misc;
using ToolBox.Tools;

public class Player_CharacterController : MonoBehaviour
{   

    Character character;

    Character.MovementType movementType;
    Vector3 pointToGoTo;
    [HideInInspector] public bool clickToMove = false;


    //public Transform attackPosition;

    public bool usesFieldOfView = false;
    [HideInInspector]
    public Vector3 viewRadiusAnglesResolution = new Vector3(10,75,5);
    FieldOfView fow;
    public LayerMask[] obstacleAndTargetMasks = new LayerMask[2];
    public MeshFilter filter;
    public List<Transform> targets;

    //List<State> stateList;
    StateMachine machine;

    Char_Mod_Inventory inventory;
    public UI_Inventory uiInventory = null;

    //public State[] states = new State[2] { State_Idle, State_Walk};

    private void Awake()
    {
        character = GetComponent<Character>();
        machine = GetComponentInChildren<StateMachine>();
        character.stateMachine = machine;



        character.stateList = new List<State>()
        {
            new State_Idle(character),
            new State_Walk(character),
            new State_Attack(character),
            new State_Interact(character),
        };

        character.stateMachine.startState = character.stateList[0];

        
    }

    private void Start()
    {
        if (usesFieldOfView)
        {
            //filter = GameObject.Find("FowMesh").GetComponent<MeshFilter>();
            //filter = transform.FindChild("FOWMesh").GetComponent<MeshFilter>();
            obstacleAndTargetMasks = new LayerMask[2] { 1 << 11, 1 << 12 };
            fow = new FieldOfView(viewRadiusAnglesResolution, transform, filter, obstacleAndTargetMasks);
            //filter = transform.Find("FOWMesh").GetComponent<MeshFilter>();
            //filter = GetComponentInChildren<MeshFilter>();
            StartCoroutine(fow.FindTargetWithDelay());
        }

        //Char_Mod_WorldItem.SpawnWorldItem(Char_Mod_ItemAssets.Instance.itemList[0], new Vector3(-10, 0, 0));
        inventory = new Char_Mod_Inventory();
        uiInventory.SetInventory(inventory);
    }

    private void Update()
    {
        if(character.equippedMainHand == null && character.equippedMainHand != character.mainHand.gameObject)
        {
            character.equippedMainHand = character.mainHand.gameObject;
        }
        //print(character.canAttack);

        InputHandler();
        //CheckDestinationReached();
        //print(character.HasReachedDestination());
        if(movementType == Character.MovementType.MoveToward && character.stateMachine.GetCurrentState != character.stateList[2] && character.stateMachine.GetCurrentState != character.stateList[3])
            MiscFunctions.LookAtMouse(transform);

        MiscFunctions.HoveredUIElement();
    }

    private void LateUpdate()
    {
        ClickOrWASD();
        if(usesFieldOfView)
            UpdateFieldOfView();
    }

    private void OnTriggerStay(Collider other)
    {
        Char_Mod_WorldItem worldItem = other.GetComponent<Char_Mod_WorldItem>();
        if(worldItem != null)
        {
            //Touching item
            if (Input.GetKeyDown(KeyCode.E))
            {
                inventory.AddItem(worldItem.GetItem());
                worldItem.DestroySelf();
            }
        }
    }

    void InputHandler()
    {

        switch (movementType)
        {
            case Character.MovementType.MoveToward:
                if(MovementVector() != Vector3.zero && machine.GetCurrentState == character.stateList[0])
                {
                    character.GoToState(character.stateList[1]);
                }
                else if(MovementVector() == Vector3.zero && machine.GetCurrentState == character.stateList[1])
                {
                    character.GoToState(character.stateList[0]);
                } else if (machine.GetCurrentState == character.stateList[2] && character.hasAttacked)
                {
                    character.GoToState(character.stateList[0]);
                } else if(character.stateMachine.GetCurrentState == character.stateList[3] && character.hasInteracted)
                {
                    character.GoToState(character.stateList[0]);
                    //character.hasInteracted = false;
                }
                break;
            case Character.MovementType.MoveToPoint:
                if (Input.GetMouseButton(1) && machine.GetCurrentState != character.stateList[2])
                {
                    //if(!character.DestinationReached())
                        character.GoToState(character.stateList[1]);

                    //character.positionOrDirection = FindWhereToGo();
                    //print(character.positionOrDirection);
                    //if (machine.GetCurrentState != stateList[1] || !character.DestinationReached())
                        //machine.SetState(stateList[1]);
                    //Move();
                }
                else
                {
                    if(character.DestinationReached())
                       character.GoToState(character.stateList[0]);

                    //character.positionOrDirection = FindWhereToGo();
                    //if (machine.GetCurrentState != stateList[0] && character.DestinationReached())
                      //  machine.SetState(stateList[0]);
                    //Move();
                }
                break;
            default:
                break;
        }

        if (!character.isSneaking && !character.isRunning && Input.GetKeyDown(KeyCode.LeftShift))
            character.isRunning = true;
        if (character.isRunning && Input.GetKeyUp(KeyCode.LeftShift) || character.isRunning && character.isSneaking)
            character.isRunning = false;

        if (!character.isSneaking && Input.GetKeyDown(KeyCode.LeftControl))
        {
            character.isSneaking = true;
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
        if (character.isSneaking && Input.GetKeyUp(KeyCode.LeftControl))
        {
            character.isSneaking = false;
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (!MiscFunctions.MouseIsHoveringUI() && Input.GetMouseButtonDown(0) && character.canAttack)
        {
            //print("Should attack");
            //print(character.canAttack);
            character.GoToState(character.stateList[2]);
            //character.DoAttack(attackPosition, 1, 15);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            uiInventory.OpenOrCLoseInventory(!uiInventory.InventoryIsOpen());
        }

        if (character.interactionObject != null && character.canInteract && MiscFunctions.IsLookingAt(transform, character.interactionObject.transform.position, 75))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                character.GoToState(character.stateList[3]);
                if(character.interactionObject.GetComponent<Character>() != null)
                {
                    character.interactionObject.GetComponent<NPC_CharacterController>().Interacting();
                    //character.interactionObject.GetComponent<Character>().GoToState(character.interactionObject.GetComponent<Character>().stateList[3]);
                }
                //character.interactionObject.GetComponent<IInteract>().Interact(character, character.interactionObject);
            }
        }



        /*if (movementType == Character.MovementType.MoveToPoint)
        {
            if (Input.GetMouseButton(1))
            {
                character.positionOrDirection = FindWhereToGo();
                if (machine.GetCurrentState != stateList[1])
                    machine.SetState(stateList[1]);
                character.Move();
            }
            else
            {
                if (machine.GetCurrentState != stateList[0])
                    machine.SetState(stateList[0]);
                character.Move();
            }
        }
        else if (movementType == Character.MovementType.MoveToward)
        {
            if (FindWhereToGo() != Vector3.zero)
            {
                character.positionOrDirection = FindWhereToGo();
                if (machine.GetCurrentState != stateList[1])
                    machine.SetState(stateList[1]);
                character.Move();
            }
            else
            {
                if (machine.GetCurrentState != stateList[0])
                    machine.SetState(stateList[0]);
                character.Move();
            }
        }
        else
            character.Move();*/

        /*if (movementType == Character.MovementType.MoveToPoint && Input.GetMouseButton(1))
        {
            //print(FindWhereToGo());

        }
        else if (movementType == Character.MovementType.MoveToward && FindWhereToGo() != Vector3.zero)
        {
            //print(FindWhereToGo());
            character.positionOrDirection = FindWhereToGo();
            Move();
        }
        else
            Move();
            */
    }

    void GoToState(State stateToGoTo)
    {
        if (machine.GetCurrentState != stateToGoTo)
            machine.SetState(stateToGoTo);
    }

    Vector3 MovementVector()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput).normalized;

        return moveVector;
    }

    Vector3 FindWhereToGo()
    {
        if (movementType == Character.MovementType.MoveToPoint && Input.GetMouseButton(1))
        {
            pointToGoTo = MiscFunctions.FindMousePositionIn3DSpace();
            return pointToGoTo;
        }
        else if (movementType == Character.MovementType.MoveToward)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput).normalized;
            pointToGoTo = transform.position;
            return moveVector;
        }
        return pointToGoTo;
    }

    

    void ClickOrWASD()
    {
        //Some bug if you change movement type while character is moving. Need to set the state to idle if this is changed in runtime.

        if (clickToMove)
        {
            movementType = Character.MovementType.MoveToPoint;
            character.movementType = movementType;
        }
        else
        {
            movementType = Character.MovementType.MoveToward;
            character.movementType = movementType;
            
        }
        //character.positionOrDirection = FindWhereToGo();
    }

    void UpdateFieldOfView()
    {
        //viewRangeAngleRes = new Vector3(viewRange, viewAngle, resolution);
        targets = fow.visibleTargets;
        fow.UpdateFieldOfView(Vector3.zero);

        foreach (var target in targets)
        {
            if(target != null)
                Debug.DrawLine(transform.position, target.position);
        }
    }

}
