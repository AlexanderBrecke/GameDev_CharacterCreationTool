using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using System;

public class CharacterCreationTool : EditorWindow
{

    // --- Modification information ---
    AnimBool modifyCharacter;
    enum CreateOrModify { create, modify };
    CreateOrModify createOrModify; 
    GameObject characterToModify = null;

    // --- Character information ---
    string characterName = "";
    AnimBool appendIDToName;
    int characterID;

    enum PlayerOrNPC { player, NPC};
    PlayerOrNPC playerOrNPC;

    LayerMask characterLayer;
    string characterTag;
    int maxHealth = 100;

    // Player centered
    AnimBool showPlayerCenteredInfo;

    // ---

    // NPC centered
    AnimBool showNPCCenteredInfo;
    enum FriendOrFoe { friendly, enemy}
    FriendOrFoe friendOrFoe;
    enum PatrolType { circular, forthAndBack, roam}
    PatrolType patrolType;

    Transform patrolObject;
    AnimBool roaming;
    float roamRadius = 15;

    float attackRange = 5;


    // ---

    enum MovementType { WASD, ClickToMove}
    MovementType movementType;

    float baseSpeed = 10f;

    bool advancedCharacterInfo;

    // Field of view centered
    AnimBool shouldUseFieldOfView;
    Material fieldOfViewMat;
    LayerMask obstacleLayer;
    LayerMask targetLayer;
    int fieldOfViewRadius = 15;
    int fieldOfViewAngle = 85;
    // ---

    // --- ---

    // --- Object Information ---
    GameObject characterModel;

    bool advancedObjectInfo;
    bool modelUsesEmptyParent = false;

    AnimBool useSpecifiedParent;
    Transform specifiedParent;
    string mainHandName;
    string offHandName;
    AnimBool usesNameForFindingHands;
    AnimBool useSpecifiedTransformForHands;
    Transform mainHand;
    Transform offHand;

    Transform attackPoint;



    // --- ---

    // --- Prefab information ---
    AnimBool createPrefab;
    bool doCreatePrefab = false;
    string prefabPath;
    bool keepInScene;

    // --- ---

    // --- Animation Information ---
    RuntimeAnimatorController animator;

    // --- ---

    


    [MenuItem("Tools/Character/CreateCharacter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CharacterCreationTool));
    }

    private void OnEnable()
    {
        modifyCharacter = EnableAnimBool(modifyCharacter);
        // --- Character centered ---
        appendIDToName = EnableAnimBool(appendIDToName);
        shouldUseFieldOfView = EnableAnimBool(shouldUseFieldOfView);
        showPlayerCenteredInfo = EnableAnimBool(showPlayerCenteredInfo);
        showNPCCenteredInfo = EnableAnimBool(showNPCCenteredInfo);
        roaming = EnableAnimBool(roaming);
        // --- ---

        // --- Object centered ---
        useSpecifiedParent = EnableAnimBool(useSpecifiedParent);
        usesNameForFindingHands = EnableAnimBool(usesNameForFindingHands);
        useSpecifiedTransformForHands = EnableAnimBool(useSpecifiedTransformForHands);

        // --- ---

        // --- Prefab centered ---
        createPrefab = EnableAnimBool(createPrefab);

        // --- ---
    }

    private void OnGUI()
    {

        // --- GUI Styles ---
        GUIStyle textStyle1 = new GUIStyle();
        textStyle1.fontSize = 12;
        textStyle1.fontStyle = FontStyle.Italic;

        GUIStyle title = new GUIStyle();
        title.fontSize = 14;
        title.fontStyle = FontStyle.Bold;

        GUIStyle boldText = new GUIStyle();
        boldText.margin.left = 3;
        boldText.fontStyle = FontStyle.Bold;


        // --- ---

        // --- Introduction! ---
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        if (createOrModify == CreateOrModify.modify)
            GUILayout.Label("Character Modification", title);
        else
            GUILayout.Label("Character Creation", title);
            GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        // --- ---

        EditorGUI.BeginChangeCheck();
        createOrModify = (CreateOrModify)EditorGUILayout.EnumPopup("Create or Modify", createOrModify);
        if (createOrModify == CreateOrModify.create && EditorGUI.EndChangeCheck())
            ResetCreationData();


        if (createOrModify == CreateOrModify.create)
            modifyCharacter.target = false;
        else
            modifyCharacter.target = true;
        
        if (EditorGUILayout.BeginFadeGroup(modifyCharacter.faded))
        {
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            characterToModify = EditorGUILayout.ObjectField("Character to modify", characterToModify, typeof(GameObject), true) as GameObject;
            if (characterToModify != null && EditorGUI.EndChangeCheck())
                FillModifyData();

            EditorGUI.indentLevel--;
            
        }
        EditorGUILayout.EndFadeGroup();
        DrawHorizontalUILine(Color.gray, 1, 4, 45);
        EditorGUILayout.Space();

        // --- Character centered information ---
        GUILayout.Label("Character Information", boldText);
        
        characterName = EditorGUILayout.TextField(new GUIContent("Character Name"), characterName);

        characterLayer = EditorGUILayout.LayerField(new GUIContent("Layer for the character", "This is optional but reccomended"), characterLayer);
        characterTag = EditorGUILayout.TagField(new GUIContent("Tag for the character", "This is optional but reccomended"), characterTag);
        maxHealth = EditorGUILayout.IntField(new GUIContent("Max health", "Will default to 100"), maxHealth);


        // --- ---

        // --- Logic centered information ---

        if(createOrModify == CreateOrModify.create)
            playerOrNPC = (PlayerOrNPC)EditorGUILayout.EnumPopup("Player or NPC", playerOrNPC);


        if (playerOrNPC == PlayerOrNPC.player)
        {
            showNPCCenteredInfo.target = false;
            showPlayerCenteredInfo.target = true;
        }
        else
        {
            showNPCCenteredInfo.target = true;
            showPlayerCenteredInfo.target = false;
        }

        // --- Player centered logic ---
        if (EditorGUILayout.BeginFadeGroup(showPlayerCenteredInfo.faded))
        {
            DrawHorizontalUILine(Color.gray, 1, 4, 45);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Player centered logic", textStyle1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel++;

            movementType = (MovementType)EditorGUILayout.EnumPopup("Movement Type", movementType);

            EditorGUI.indentLevel--;
            DrawHorizontalUILine(Color.gray, 1, 4, 45);
        }
        EditorGUILayout.EndFadeGroup();
        // ---

        // --- NPC centered Logic ---
        if (EditorGUILayout.BeginFadeGroup(showNPCCenteredInfo.faded))
        {
            DrawHorizontalUILine(Color.gray, 1, 4, 45);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("NPC centered logic", textStyle1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel++;

            //As Friendly NPC Logic is not implemented, we will not allow for this for now and will instead just set friendOrFoe to enemy.
            //friendOrFoe = (FriendOrFoe)EditorGUILayout.EnumPopup("Friendly or Enemy", friendOrFoe);
            friendOrFoe = FriendOrFoe.enemy;
            // This will be fixed by removing the friendOrFoe = enemy and uncommenting the line above it when friendly Logic has been implemented

            if(friendOrFoe == FriendOrFoe.enemy)
            {
                patrolType = (PatrolType)EditorGUILayout.EnumPopup("Patrol Type", patrolType);
                patrolObject = EditorGUILayout.ObjectField(new GUIContent("Patrol object", "If patrol type != 'Roam', this is the parent object and will need the patrol points childed. If it is, this will be the center of the roam patrol"), patrolObject, typeof(Transform), true) as Transform;
                if (patrolType == PatrolType.roam)
                    roaming.target = true;
                else
                    roaming.target = false;
                if(EditorGUILayout.BeginFadeGroup(roaming.faded))
                {
                    EditorGUI.indentLevel++;
                    roamRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the roam patrol type"), roamRadius);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("Friendly NPC behaviour has not been implemented yet, and will be coming shortly", MessageType.Info);
            }

            attackRange = EditorGUILayout.FloatField(new GUIContent("Attack range", "The range an enemy will need to be from the player to start attacking. It is defaulted to 5 because of the reach of the attack"), attackRange);


            EditorGUI.indentLevel--;
            DrawHorizontalUILine(Color.gray, 1, 4, 45);
        }
        EditorGUILayout.EndFadeGroup();
        // ---

        EditorGUILayout.Space();

        baseSpeed = EditorGUILayout.Slider(new GUIContent("Base speed", "Base speed before any modification. This is in units traveled per second"), baseSpeed, 3.5f, 15f);
        
        // --- ---

        // --- Advanced character information ---
        advancedCharacterInfo = EditorGUILayout.Foldout(advancedCharacterInfo, "Advanced character info", true);
        if (advancedCharacterInfo)
        {
            EditorGUI.indentLevel++;

            if(createOrModify != CreateOrModify.modify)
            {
                appendIDToName.target = EditorGUILayout.ToggleLeft(new GUIContent("Append ID to Name", "Appends a nummerical ID to the end of the name"), appendIDToName.target);
                if (EditorGUILayout.BeginFadeGroup(appendIDToName.faded))
                {
                    EditorGUI.indentLevel++;

                    characterID = EditorGUILayout.IntField("Character ID", characterID);

                    EditorGUI.indentLevel--;
                    DrawHorizontalUILine(Color.gray, 1, 4, 45);
                }
                EditorGUILayout.EndFadeGroup();
            }

            shouldUseFieldOfView.target = EditorGUILayout.ToggleLeft("Use Field Of View", shouldUseFieldOfView.target);
            if (EditorGUILayout.BeginFadeGroup(shouldUseFieldOfView.faded))
            {
                EditorGUI.indentLevel++;

                fieldOfViewMat = EditorGUILayout.ObjectField("Material for the FOW", fieldOfViewMat, typeof(Material), false) as Material;

                obstacleLayer = EditorGUILayout.LayerField(new GUIContent("Obstacle layer", "Sets the layer for obstacles for the FOW"), obstacleLayer);
                targetLayer = EditorGUILayout.LayerField(new GUIContent("Target layer", "Sets the layer for targets for the FOW"), targetLayer);

                fieldOfViewRadius = EditorGUILayout.IntField(new GUIContent("Field of view radius", "The radius of the field of view"), fieldOfViewRadius);
                if (fieldOfViewRadius < 0)
                    fieldOfViewRadius = 0;
                fieldOfViewAngle = EditorGUILayout.IntSlider(new GUIContent("Field of view angle", "The angle of the field of view"), fieldOfViewAngle, 0, 360);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();


            EditorGUI.indentLevel--;
        }
        // ---


        DrawHorizontalUILine(Color.gray);
        // --- ---

        EditorGUILayout.Space();

        // --- Object centered basic information ---
        GUILayout.Label("Basic Object information", boldText);
        //GUILayout.Box("FOO", GUILayout.ExpandWidth(true));
        
        characterModel = EditorGUILayout.ObjectField("Character Model:", characterModel, typeof(GameObject), true) as GameObject;

        // --- Advanced object info ---
        advancedObjectInfo= EditorGUILayout.Foldout(advancedObjectInfo, "Advanced object info", true);
        if (advancedObjectInfo)
        {
            EditorGUI.indentLevel++;

            if(createOrModify != CreateOrModify.modify)
            {
                modelUsesEmptyParent = EditorGUILayout.ToggleLeft(new GUIContent("Model uses empty parent", "Will not child the model to an empty Game Object"), modelUsesEmptyParent);

                useSpecifiedParent.target = EditorGUILayout.ToggleLeft(new GUIContent("Use specified parent object", "will use specified Game Object as parent"), useSpecifiedParent.target);
                if (EditorGUILayout.BeginFadeGroup(useSpecifiedParent.faded))
                {

                    specifiedParent = EditorGUILayout.ObjectField(new GUIContent("Parent Object", "This needs to be a scene object"), specifiedParent, typeof(Transform), true) as Transform;

                } else
                {
                    specifiedParent = null;
                }
                EditorGUILayout.EndFadeGroup();
            }

            useSpecifiedTransformForHands.target = EditorGUILayout.ToggleLeft(new GUIContent("Use specified transform for hands"), useSpecifiedTransformForHands.target);
            if (EditorGUILayout.BeginFadeGroup(useSpecifiedTransformForHands.faded))
            {
                mainHand = EditorGUILayout.ObjectField(new GUIContent("Main hand transform", "If left empty, the tool will create one at position 0.75,0.75,0"), mainHand, typeof(Transform), true) as Transform;
                offHand = EditorGUILayout.ObjectField(new GUIContent("Off hand transform", "If left empty, the tool will create one at position -0.75,0.75,0"), offHand, typeof(Transform), true) as Transform;
            }
            EditorGUILayout.EndFadeGroup();
            if (useSpecifiedTransformForHands.value == false)
            {
                usesNameForFindingHands.target = true;
            }
            else
                usesNameForFindingHands.target = false;
            if (EditorGUILayout.BeginFadeGroup(usesNameForFindingHands.faded))
            {
                mainHandName = EditorGUILayout.TextField(new GUIContent("Main hand name in model", "If left empty, or object cannot be found, the tool will create one at position 0.75,0.75,0"), mainHandName);
                offHandName = EditorGUILayout.TextField(new GUIContent("Off hand name in model", "if left empty, or object cannot be found, the tool will create one at position -0.75,0.75,0"), offHandName);
            }
            EditorGUILayout.EndFadeGroup();

            attackPoint = EditorGUILayout.ObjectField(new GUIContent("Attack point", "This is the point where an attack is calculated around. It is implemented because of the way the system currently works. If left empty, the tool will create one at position 0,1,2"), attackPoint, typeof(Transform), true) as Transform;


            //CreateFadeGroup<Transform>(useSpecifiedParent, "Use specified parent object", specifiedParent, "parent object");

            

            EditorGUI.indentLevel--;
        }
        // ---

        DrawHorizontalUILine(Color.gray);

        EditorGUILayout.Space();
        // --- ---

        // --- Do you want a prefab with that? ---
        if(createOrModify != CreateOrModify.modify)
        {
            GUILayout.Label("Prefab information", boldText);
            createPrefab.target = EditorGUILayout.ToggleLeft("Save as a Prefab", createPrefab.target);
            if (EditorGUILayout.BeginFadeGroup(createPrefab.faded))
            {
                EditorGUI.indentLevel++;

                prefabPath = EditorGUILayout.TextField("Prefab Path", prefabPath == string.Empty ? "'Assets/SomeFolder...'" : prefabPath);
                doCreatePrefab = true;
                keepInScene = EditorGUILayout.ToggleLeft("Keep in scene?", keepInScene);

                EditorGUI.indentLevel--;
            }
            else
            {
                doCreatePrefab = false;
                keepInScene = false;
            }
            EditorGUILayout.EndFadeGroup();

            DrawHorizontalUILine(Color.gray);

            EditorGUILayout.Space();
        }
        // --- ---

        // --- Animation stuff ---
        GUILayout.Label("Animator information", boldText);

        animator = EditorGUILayout.ObjectField(new GUIContent("Animator controller"), animator, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
        //animator = EditorGUILayout.PropertyField(new GUIContent("Animator goes here"), animator, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
        //EditorGUILayout.fiel
        // --- ---



        // --- Creation/Modification button ---
        if(createOrModify == CreateOrModify.create)
        {
            EditorGUI.BeginDisabledGroup(createButtonDisableParams());

            if(GUILayout.Button("Create Character"))
            {
                CreateCharacter();
            }
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUI.BeginDisabledGroup(createButtonDisableParams());

            if(GUILayout.Button("Modify Character"))
            {
                ModifyCharacter();
            }
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.Space();
        // --- ---






        //Stuff for errors here..

        // --- Error messages ---

        // Modify stuff
        if (modifyCharacter.value == true && characterToModify == null)
            EditorGUILayout.HelpBox("Please insert character to modify", MessageType.Warning);
        if (modifyCharacter.value == true && characterToModify != null && characterName == string.Empty)
            EditorGUILayout.HelpBox("Please give a Name to the Character", MessageType.Warning);

        // ---

        // Create stuff


        if (characterName == string.Empty && modifyCharacter.value == false)
            EditorGUILayout.HelpBox("Please give a Name to the Character", MessageType.Warning);
        
        if(characterModel == null && !modifyCharacter.value == true)
            EditorGUILayout.HelpBox("Please insert Character Model", MessageType.Warning);

        if (playerOrNPC == PlayerOrNPC.NPC && patrolObject == null)
            EditorGUILayout.HelpBox("Please insert a patrol object", MessageType.Warning);

        if (shouldUseFieldOfView.value == true && fieldOfViewMat == null)
            EditorGUILayout.HelpBox("Please supply a material for the field of view", MessageType.Warning);

        if (shouldUseFieldOfView.value == true && obstacleLayer == LayerMask.NameToLayer("Default"))
            EditorGUILayout.HelpBox("Obstacle layer should not be default", MessageType.Warning);

        if (shouldUseFieldOfView.value == true && targetLayer == LayerMask.NameToLayer("Default"))
            EditorGUILayout.HelpBox("Target layer should not be default", MessageType.Warning);

        if (useSpecifiedParent.value == true && specifiedParent == null)
            EditorGUILayout.HelpBox("Please supply a parent object", MessageType.Warning);

        if (useSpecifiedParent.value == true && specifiedParent != null && EditorUtility.IsPersistent(specifiedParent))
            EditorGUILayout.HelpBox("Specified parent object needs to be a scene object", MessageType.Warning);

        if (createPrefab.value == true && prefabPath == "'Assets/SomeFolder...'")
            EditorGUILayout.HelpBox("Please insert a valid path name for your prefab", MessageType.Warning);

        // ---

        // --- ---
    }







    bool createButtonDisableParams()
    {
        bool disabled = false;
        
        // Creation stuff
        if (characterName == string.Empty)
            disabled = true;
        if (characterModel == null)
            disabled = true;
        if (playerOrNPC == PlayerOrNPC.NPC && patrolObject == null)
            disabled = true;
        if (shouldUseFieldOfView.value == true && fieldOfViewMat == null)
            disabled = true;
        if (shouldUseFieldOfView.value == true && obstacleLayer == LayerMask.NameToLayer("Default"))
            disabled = true;
        if (shouldUseFieldOfView.value == true && targetLayer == LayerMask.NameToLayer("Default"))
            disabled = true;
        if (useSpecifiedParent.value == true && specifiedParent != null && EditorUtility.IsPersistent(specifiedParent) || useSpecifiedParent.value == true && specifiedParent == null)
            disabled = true;
        if (createPrefab.value == true && prefabPath == "'Assets/SomeFolder...'")
            disabled = true;

        // ---

        // Modification stuff
        if (modifyCharacter.value == true && characterToModify == null)
            disabled = true;

        // ---


        return disabled;
    }


    void FillModifyData()
    {
        characterName = characterToModify.name;
        characterLayer = characterToModify.layer;
        characterTag = characterToModify.tag;
        if (characterToModify.GetComponent<Player_CharacterController>() != null)
        {
            playerOrNPC = PlayerOrNPC.player;
            if (characterToModify.GetComponent<Player_CharacterController>().clickToMove)
                movementType = MovementType.ClickToMove;
            else
                movementType = MovementType.WASD;
        }
        if (characterToModify.GetComponent<NPC_CharacterController>() != null)
        {
            playerOrNPC = PlayerOrNPC.NPC;
            if (characterToModify.GetComponent<NPC_CharacterController>().friendOrFoe == NPC_CharacterController.FriendOrFoe.enemy)
            {
                friendOrFoe = FriendOrFoe.enemy;
                if (characterToModify.GetComponent<NPC_CharacterController>().patrolType == State_Patrol.PatrolType.Circular)
                    patrolType = PatrolType.circular;
                if (characterToModify.GetComponent<NPC_CharacterController>().patrolType == State_Patrol.PatrolType.ForthAndBack)
                    patrolType = PatrolType.forthAndBack;
                if (characterToModify.GetComponent<NPC_CharacterController>().patrolType == State_Patrol.PatrolType.Roam)
                    patrolType = PatrolType.roam;
                patrolObject = characterToModify.GetComponent<NPC_CharacterController>().patrolPointsObject.transform;
                attackRange = characterToModify.GetComponent<Character>().attackRange;
            }
            else
            {
                friendOrFoe = FriendOrFoe.friendly;

            }


        }
        if (characterToModify.GetComponent<Animator>().runtimeAnimatorController != null)
            animator = characterToModify.GetComponent<Animator>().runtimeAnimatorController;
        if (characterToModify.GetComponent<Character>().mainHand != null)
        {
            useSpecifiedTransformForHands.target = true;
            mainHand = characterToModify.GetComponent<Character>().mainHand;
        }
        if(characterToModify.GetComponent<Character>().offHand != null)
        {
            useSpecifiedTransformForHands.target = true;
            offHand = characterToModify.GetComponent<Character>().offHand;
        }
        if (characterToModify.GetComponent<Character>().attackPoint != null)
            attackPoint = characterToModify.GetComponent<Character>().attackPoint;
        baseSpeed = characterToModify.GetComponent<Character>().baseSpeed;

        if (characterToModify.GetComponent<Player_CharacterController>() != null && characterToModify.GetComponent<Player_CharacterController>().usesFieldOfView || characterToModify.GetComponent<NPC_CharacterController>() != null && characterToModify.GetComponent<NPC_CharacterController>().usesFieldOfView)
        {
            shouldUseFieldOfView.target = true;
            if(playerOrNPC == PlayerOrNPC.player)
            {
                fieldOfViewMat = characterToModify.GetComponent<Player_CharacterController>().filter.gameObject.GetComponent<MeshRenderer>().sharedMaterial;

                //Some way to get the obstacle and target layer here

                obstacleLayer = Mathf.RoundToInt(Mathf.Log(characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[0].value, 2));
                targetLayer = Mathf.RoundToInt(Mathf.Log(characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[1].value, 2));

                fieldOfViewRadius = Mathf.RoundToInt(characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.x);
                fieldOfViewAngle = Mathf.RoundToInt(characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.y);

            }
            else
            {
                fieldOfViewMat = characterToModify.GetComponent<NPC_CharacterController>().fowFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial;

                //Some way to get the obstacle and target layer here
                obstacleLayer = Mathf.RoundToInt(Mathf.Log(characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[0].value, 2));
                targetLayer = Mathf.RoundToInt(Mathf.Log(characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[1].value, 2));

                fieldOfViewRadius = Mathf.RoundToInt(characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.x);
                fieldOfViewAngle = Mathf.RoundToInt(characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.y);
            }
        }

        foreach (Transform transform in characterToModify.GetComponentInChildren<Transform>())
        {
            if (transform.gameObject.name == "CharacterModel")
                characterModel = transform.gameObject;
        }
    }

    void ResetCreationData()
    {
        characterToModify = null;
        characterName = "";
        characterTag = "Default";
        characterLayer = 0;
        playerOrNPC = PlayerOrNPC.player;
        //FriendOrFoe does not have friendly implemented yet.
        //friendOrFoe = FriendOrFoe.friendly;
        patrolType = PatrolType.circular;
        patrolObject = null;
        attackRange = 5;
        baseSpeed = 10;
        advancedCharacterInfo = false;
        shouldUseFieldOfView.target = false;
        fieldOfViewMat = null;
        obstacleLayer = 0;
        targetLayer = 0;
        fieldOfViewRadius = 15;
        fieldOfViewAngle = 82;
        characterModel = null;
        advancedObjectInfo = false;
        modelUsesEmptyParent = false;
        useSpecifiedParent.target = false;
        useSpecifiedTransformForHands.target = false;
        mainHand = null;
        offHand = null;
        mainHandName = "";
        offHandName = "";
        attackPoint = null;
        specifiedParent = null;
        doCreatePrefab = false;
    }


    void CreateCharacter()
    {

        string charName = characterName;
        if(appendIDToName.value == true)
        {
            charName += characterID;
            characterID++;
        }

        if (modelUsesEmptyParent)
        {
            GameObject characterObject = Instantiate(characterModel, Vector3.zero, Quaternion.identity);
            characterObject.name = charName;

            AddComponentsToParent(characterObject);
            

            if (doCreatePrefab)
                CreatePrefab(characterObject, charName);

            

        }
        else if(!modelUsesEmptyParent || useSpecifiedParent.value == true)
        {
            GameObject parentObject;
            if (useSpecifiedParent.value == true)
            {
                parentObject = specifiedParent.gameObject;
                parentObject.name = charName;
            }
            else
                parentObject = new GameObject(charName);

            AddModelToParent(parentObject);
            AddComponentsToParent(parentObject);

            if (doCreatePrefab)
                CreatePrefab(parentObject, charName);
        }
    }

    void ModifyCharacter()
    {
        //Debug.Log("Character Modification not implemented yet");
        string charName = characterName;
        if(appendIDToName.value == true)
        {
            charName += characterID;
            characterID++;
        }
        if(characterToModify.name != charName)
            characterToModify.name = charName;
        characterToModify.GetComponent<Character>().Name = characterName;

        if (characterLayer != LayerMask.NameToLayer("Default"))
            characterToModify.layer = characterLayer;
        if (characterTag != "Untagged")
            characterToModify.tag = characterTag;

        if(playerOrNPC == PlayerOrNPC.player)
        {
            if(characterToModify.GetComponent<Player_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == false)
            {
                characterToModify.GetComponent<Player_CharacterController>().usesFieldOfView = false;
                RemoveComponents(characterToModify);
            } else if(!characterToModify.GetComponent<Player_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == true)
            {
                AddFieldOfView(characterToModify);
            }
            else if (characterToModify.GetComponent<Player_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == true)
            {
                if (fieldOfViewMat != characterToModify.GetComponent<Player_CharacterController>().filter.gameObject.GetComponent<MeshRenderer>().sharedMaterial)
                    characterToModify.GetComponent<Player_CharacterController>().filter.gameObject.GetComponent<MeshRenderer>().sharedMaterial = fieldOfViewMat;
                if (1 << obstacleLayer.value != characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[0])
                    characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[0] = 1 << obstacleLayer.value;
                if (1 << targetLayer.value != characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[1])
                    characterToModify.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[1] = 1 << targetLayer.value;
                if (fieldOfViewRadius != characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.x)
                    characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.x = fieldOfViewRadius;
                if (fieldOfViewAngle != characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.y)
                    characterToModify.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution.y = fieldOfViewAngle;
            }
        } else
        {
            if(friendOrFoe == FriendOrFoe.enemy)
            {
                if (patrolType == PatrolType.circular)
                    characterToModify.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.Circular;
                if (patrolType == PatrolType.forthAndBack)
                    characterToModify.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.ForthAndBack;
                if (patrolType == PatrolType.roam)
                    characterToModify.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.Roam;
                if (patrolObject != null)
                    characterToModify.GetComponent<NPC_CharacterController>().patrolPointsObject = patrolObject.gameObject;
                if (characterToModify.GetComponent<Character>().attackRange != attackRange)
                    characterToModify.GetComponent<Character>().attackRange = attackRange;
                if (characterToModify.GetComponent<NPC_CharacterController>().roamRadius != roamRadius)
                    characterToModify.GetComponent<NPC_CharacterController>().roamRadius = roamRadius;
            } else
            {

            }

            if(characterToModify.GetComponent<NPC_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == false)
            {
                characterToModify.GetComponent<NPC_CharacterController>().usesFieldOfView = false;
                RemoveComponents(characterToModify);
            } else  if(!characterToModify.GetComponent<NPC_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == true)
            {
                AddFieldOfView(characterToModify);
            } else if(characterToModify.GetComponent<NPC_CharacterController>().usesFieldOfView && shouldUseFieldOfView.value == true)
            {
                if (fieldOfViewMat != characterToModify.GetComponent<NPC_CharacterController>().fowFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial)
                    characterToModify.GetComponent<NPC_CharacterController>().fowFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial = fieldOfViewMat;
                if (1 << obstacleLayer.value != characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[0])
                    characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[0] = 1 << obstacleLayer.value;
                if (1 << targetLayer.value != characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[1])
                    characterToModify.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[1] = 1 << targetLayer.value;
                if (fieldOfViewRadius != characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.x)
                    characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.x = fieldOfViewRadius;
                if (fieldOfViewAngle != characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.y)
                    characterToModify.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution.y = fieldOfViewAngle;
            }
        }

        if (characterToModify.GetComponent<Character>().baseSpeed != baseSpeed)
            characterToModify.GetComponent<Character>().baseSpeed = baseSpeed;
        if (characterToModify.GetComponent<Character>().maxHealth != maxHealth)
            characterToModify.GetComponent<Character>().maxHealth = maxHealth;

        GameObject currentCharModel = null;
        foreach (Transform transform in characterToModify.GetComponentInChildren<Transform>())
        {
            if (transform.gameObject.name == "CharacterModel")
                currentCharModel = transform.gameObject;
        }
        if (currentCharModel != characterModel)
        {
            DestroyImmediate(currentCharModel);
            AddModelToParent(characterToModify);
        }

        if (characterToModify.GetComponent<Animator>().runtimeAnimatorController != animator)
            characterToModify.GetComponent<Animator>().runtimeAnimatorController = animator;

    }


    

    void AddModelToParent(GameObject parentObject)
    {
        GameObject charModel = Instantiate(characterModel, parentObject.transform, false);
        Vector3 characterPosition = new Vector3(parentObject.transform.position.x, parentObject.transform.position.y + (charModel.transform.localScale.y / 2), parentObject.transform.position.z);
        charModel.transform.position = characterPosition;
        charModel.name = "CharacterModel";
    }

    void AddComponentsToParent(GameObject parentObject)
    {
        parentObject.layer = characterLayer;
        if (characterTag == null || characterTag == string.Empty)
            parentObject.tag = "Untagged";
        else
            parentObject.tag = characterTag;

        parentObject.AddComponent<Character>();
        parentObject.GetComponent<Character>().Name = characterName;
        parentObject.GetComponent<Character>().baseSpeed = baseSpeed;
        parentObject.GetComponent<Character>().maxHealth = maxHealth;
        
        if(useSpecifiedTransformForHands.value == true)
        {
            if(mainHand == null)
            {
                GameObject mainHandObj = new GameObject("MainHand");
                mainHandObj.transform.SetParent(parentObject.transform);
                mainHandObj.transform.position = new Vector3(0.75f,0.75f,0f);
                parentObject.GetComponent<Character>().mainHand = mainHandObj.transform;
            } else
            {
                parentObject.GetComponent<Character>().mainHand = mainHand;
            }
            if(offHand == null)
            {
                GameObject offHandObj = new GameObject("OffHand");
                offHandObj.transform.SetParent(parentObject.transform);
                offHandObj.transform.position = new Vector3(-0.75f, 0.75f, 0);
                parentObject.GetComponent<Character>().offHand = offHandObj.transform;
            } else
            {
                parentObject.GetComponent<Character>().mainHand = mainHand;
            }
        }
        else
        {
            if(mainHandName == string.Empty || mainHandName == null)
            {
                GameObject mainHandObj = new GameObject("MainHand");
                mainHandObj.transform.SetParent(parentObject.transform);
                mainHandObj.transform.position = new Vector3(0.75f, 0.75f, 0f);
                parentObject.GetComponent<Character>().mainHand = mainHandObj.transform;
            } else
            {
                foreach (Transform item in parentObject.transform)
                {
                    if (item.name == mainHandName)
                        parentObject.GetComponent<Character>().mainHand = item;
                }
            }
            if(offHandName == string.Empty || offHandName == null)
            {
                GameObject offHandObj = new GameObject("OffHand");
                offHandObj.transform.SetParent(parentObject.transform);
                offHandObj.transform.position = new Vector3(-0.75f, 0.75f, 0);
                parentObject.GetComponent<Character>().offHand = offHandObj.transform;
            } else
            {
                foreach (Transform item in parentObject.transform)
                {
                    if (item.name == offHandName)
                        parentObject.GetComponent<Character>().offHand = item;
                }
            }
        }
        
        parentObject.GetComponent<Character>().attackRange = attackRange;
        if(attackPoint != null)
            parentObject.GetComponent<Character>().attackPoint = attackPoint;
        else
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(parentObject.transform);
            attackPointObj.transform.position = new Vector3(0, 1, 2);
            parentObject.GetComponent<Character>().attackPoint = attackPointObj.transform;
        }
        


        if (playerOrNPC == PlayerOrNPC.player)
        {
            parentObject.AddComponent<Player_CharacterController>();
            parentObject.GetComponent<Character>().characterType = Character.CharacterType.Player;
            if (movementType == MovementType.ClickToMove)
                parentObject.GetComponent<Player_CharacterController>().clickToMove = true;
            //parentObject.GetComponent<Player_CharacterController>().uiInventory = FindObjectOfType<UI_Inventory>();
            parentObject.GetComponent<Player_CharacterController>().uiInventory = Resources.FindObjectsOfTypeAll<UI_Inventory>()[0];
        }
        else
        {
            // NPC additions
            parentObject.GetComponent<Character>().characterType = Character.CharacterType.NPC;
            parentObject.AddComponent<NPC_CharacterController>();
            if(friendOrFoe == FriendOrFoe.enemy)
            {
                // Enemy setup
                parentObject.GetComponent<NPC_CharacterController>().friendOrFoe = NPC_CharacterController.FriendOrFoe.enemy;
                if (patrolType == PatrolType.circular)
                    parentObject.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.Circular;
                if (patrolType == PatrolType.forthAndBack)
                    parentObject.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.ForthAndBack;
                if (patrolType == PatrolType.roam)
                {
                    parentObject.GetComponent<NPC_CharacterController>().patrolType = State_Patrol.PatrolType.Roam;
                    parentObject.GetComponent<NPC_CharacterController>().roamRadius = roamRadius;
                }

                parentObject.GetComponent<NPC_CharacterController>().patrolPointsObject = patrolObject.gameObject;
                parentObject.AddComponent<StealthKillInteraction>();
                
                // ---
            }
            else
            {
                // Friendly NPC setup
                parentObject.GetComponent<NPC_CharacterController>().friendOrFoe = NPC_CharacterController.FriendOrFoe.friendly;

                // ---
            }
        }

        

        parentObject.AddComponent<MoveVelocity>();

        // --- This would ideally be how you do it, but since I am showcasing here, and want the tester to be able to check or uncheck this movement
        // at runtime I will add the script no matter what.
        //if (movementType == MovementType.ClickToMove)
        parentObject.AddComponent<MovePosition>();
        parentObject.AddComponent<HealthHandler>();
        parentObject.AddComponent<SimpleAttack>();


        AddStateMachine(parentObject);
        if (shouldUseFieldOfView.value == true)
            AddFieldOfView(parentObject);
        
        
        parentObject.AddComponent<Rigidbody>();
        parentObject.GetComponent<Rigidbody>().useGravity = false;
        parentObject.GetComponent<Rigidbody>().freezeRotation = true;
        parentObject.AddComponent<BoxCollider>();
        if (!modelUsesEmptyParent)
        {
            parentObject.GetComponent<BoxCollider>().center = new Vector3(0, characterModel.transform.localScale.y / 2, 0);
            parentObject.GetComponent<BoxCollider>().size = new Vector3(characterModel.transform.localScale.x, characterModel.transform.localScale.y, characterModel.transform.localScale.z);
        }
        else
        {
            parentObject.GetComponent<BoxCollider>().center = new Vector3(0, FindModelSize(parentObject).y / 2, 0);
            parentObject.GetComponent<BoxCollider>().size = FindModelSize(parentObject);
        }
        parentObject.AddComponent<Animator>();
        if (animator != null)
            parentObject.GetComponent<Animator>().runtimeAnimatorController = animator;
    }

    Vector3 FindModelSize(GameObject parentObject)
    {
        Vector3 size = Vector3.zero;
        foreach (Transform item in parentObject.transform)
        {
            if (item.name == "CharacterModel")
                size = item.localScale;
        }
        return size;
    }

    void AddStateMachine(GameObject parentObject)
    {
        GameObject stateMachineObject = new GameObject("StateMachine");
        stateMachineObject.transform.SetParent(parentObject.transform);
        stateMachineObject.AddComponent<StateMachine>();
    }

    void AddFieldOfView(GameObject parentObject)
    {
        GameObject fowObject = new GameObject("FOWMesh");
        fowObject.transform.SetParent(parentObject.transform);
        fowObject.AddComponent<MeshFilter>();
        fowObject.AddComponent<MeshRenderer>();
        fowObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        fowObject.GetComponent<MeshRenderer>().receiveShadows = false;
        fowObject.GetComponent<MeshRenderer>().material = fieldOfViewMat;
        if (playerOrNPC == PlayerOrNPC.player)
        {
            parentObject.GetComponent<Player_CharacterController>().filter = fowObject.GetComponent<MeshFilter>();
            parentObject.GetComponent<Player_CharacterController>().usesFieldOfView = true;
            parentObject.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[0] = 1 << obstacleLayer.value;
            parentObject.GetComponent<Player_CharacterController>().obstacleAndTargetMasks[1] = 1 << targetLayer.value;
            parentObject.GetComponent<Player_CharacterController>().viewRadiusAnglesResolution = new Vector3(fieldOfViewRadius, fieldOfViewAngle, 2);
        } else if(playerOrNPC == PlayerOrNPC.NPC)
        {
            parentObject.GetComponent<NPC_CharacterController>().fowFilter = fowObject.GetComponent<MeshFilter>();
            parentObject.GetComponent<NPC_CharacterController>().usesFieldOfView = true;
            parentObject.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[0] = 1 << obstacleLayer.value;
            parentObject.GetComponent<NPC_CharacterController>().obstaclesAndTargetMasks[1] = 1 << targetLayer.value;
            parentObject.GetComponent<NPC_CharacterController>().viewRadiusAnglesResolution = new Vector3(fieldOfViewRadius, fieldOfViewAngle, 2);
        }
    }

    void RemoveComponents(GameObject parentObject)
    {
        if(parentObject.GetComponent<NPC_CharacterController>() != null)
        {
            GameObject fowChecker = null;
            foreach (Transform transform in parentObject.GetComponentInChildren<Transform>())
            {
                if (transform.gameObject.name == "FOWMesh")
                    fowChecker = transform.gameObject;
            }
            if (fowChecker != null && !parentObject.GetComponent<NPC_CharacterController>().usesFieldOfView)
                DestroyImmediate(fowChecker);
        }
    }



    void CreatePrefab(GameObject parentObject, string characterName)
    {
        if (doCreatePrefab)
        {
            PrefabUtility.SaveAsPrefabAsset(parentObject, prefabPath + "/" + characterName + ".prefab");
            if (!keepInScene)
                DestroyImmediate(parentObject);
        }
    }

    void CreateFadeGroup<T>(AnimBool animationBool, string firstLabel, T type, string secondLabel, string tooltip = "")
    {
        animationBool.target = EditorGUILayout.ToggleLeft(firstLabel, animationBool.target);
        if (EditorGUILayout.BeginFadeGroup(animationBool.faded))
        {
            EditorGUI.indentLevel++;

            //Some function here to set the label field
            //if(tooltip == "")
            //{
            //    type = EditorGUILayout.PropertyField();

            //}

            //type = EditorGUILayout.ObjectField(secondLabel, type, typeof(T), false) as T;

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
    }

    
    AnimBool EnableAnimBool(AnimBool animBool)
    {
        animBool = new AnimBool(false);
        animBool.valueChanged.AddListener(Repaint);
        return animBool;
    }




    // --- Style utility methods ---

    static void DrawHorizontalUILine(Color color, int thickness = 1, int padding = 6, int margin = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height= thickness;
        r.y += padding / 2;
        r.width -= margin;
        r.x += margin / 2;
        EditorGUI.DrawRect(r, color);
    }

    // --- ---


}
