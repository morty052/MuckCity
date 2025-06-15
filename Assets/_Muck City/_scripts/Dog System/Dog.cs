/*
 * POLYGON DOG ANIMATION SCRIPT
 * DESCRIPTION: This script demonstrates the range of animations and Prefabs included in 
 * Polygon Dog which can be customized for the users preference. Please attach this to the
 * Dog.Prefab asset then customize the keys for the particular animations.
 * 
 * PLEASE NOTE: This script is intended for demonstration purposes and user customization or 
 * third party animation plugins will be required for further animation options.
 * 
 */
using System;
using System.Collections;
using System.Threading.Tasks;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Invector;
using System.Collections.Generic;

public class Dog : MonoBehaviour
{

    public static Dog Instance { get; private set; }

    [TabGroup("Attack")]
    public LayerMask _enemyLayer;

    [HideInInspector]
    public Animator _animator;// Animator for the assigned dog

    CountdownTimer _stateTimer;// Timer for the current state

    public StateMachine _stateMachine;// State machine for the dog

    [TabGroup("State")]
    public float _stateUpdateTime = 1f;
    bool dogActionEnabled;

    private int countDown = 1;
    bool Movement_f;
    bool death_b = false;
    bool Sleep_b = false;
    bool Sit_b = false;

    [TabGroup("Movement")]
    private float w_movement = 0.0f; // Run value

    [TabGroup("Movement")]
    public float acceleration = 1.0f;

    [TabGroup("Movement")]
    public float decelleration = 1.0f;

    [TabGroup("Movement")]
    private float maxWalk = 0.5f;
    [TabGroup("Movement")]
    private float maxRun = 1.0f;

    [TabGroup("Movement")]
    private float currentSpeed;
    [Header("Particle FX")]
    [TabGroup("Particle FX")]
    public ParticleSystem poopFX;
    [TabGroup("Particle FX")]
    public ParticleSystem dirtFX;
    [TabGroup("Particle FX")]
    public ParticleSystem peeFX;
    [TabGroup("Particle FX")]
    public ParticleSystem waterFX;
    [TabGroup("Particle FX")]
    private Vector3 newSpawn = new Vector3();
    [TabGroup("Particle FX")]
    public Transform fxTransform;
    [TabGroup("Particle FX")]
    public Transform fxTail;

    [HideInInspector]
    public NavMeshAgent _agent;

    [HideInInspector]
    public DogSensor _dogSensor;

    [TabGroup("Movement")]
    public float _walkingStoppingDistance = 1.4f;
    [TabGroup("Movement")]
    public float _runningStoppingDistance = 2.8f;
    [TabGroup("Attack")]
    public float _combatDistance = 1f;
    [TabGroup("Attack")]
    public float _chaseDistance = 2.8f;

    [TabGroup("Movement")]
    public bool walkPressed = false;

    [TabGroup("Movement")]
    public bool runPressed = false;

    [TabGroup("Movement")]

    public bool _shouldRun;
    [TabGroup("State")]
    public bool _isChasing = false;

    [TabGroup("State")]

    public bool _searchingForPlayer = false;
    [TabGroup("State")]

    public Mode _mode;

    [TabGroup("Attack")]
    public Transform _currentTarget;

    [TabGroup("Attack")]
    public vObjectDamage _mouthTransform;
    [TabGroup("Attack")]
    public float _maxEnemyFollowDistance = 8f;

    [TabGroup("Debug")]
    public bool _debug;
    [TabGroup("Debug")]
    public DogDebugger _debugger;


    [TabGroup("State")]
    public bool _playerIsCalling;


    public HashSet<GameObject> _ignoreList = new();

    public bool _hasAccessToPlayer = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _dogSensor = GetComponent<DogSensor>();
            SetupTransitions();

        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnEnable()
    {
        DogInputsListener.OnCallRoverPressed += FindPlayer;
    }

    void OnDisable()
    {
        DogInputsListener.OnCallRoverPressed -= FindPlayer;
        _stateTimer.Dispose();
    }

    void Start()
    {
        _stateTimer = new CountdownTimer(_stateUpdateTime);
        _stateTimer.OnTimerStop += UpdateState;
        _stateTimer.Start();
        if (_debug)
        {
            InitDebugger();
        }
    }


    void Update() // Update loop for dog actions
    {
        if (_agent.hasPath && _agent.remainingDistance >= _agent.stoppingDistance)
        {
            if (!_shouldRun)
            {
                walkPressed = true;
                runPressed = false;
                // _agent.stoppingDistance = _walkingStoppingDistance;
            }

            else
            {
                runPressed = true;
                walkPressed = true;
                // _agent.stoppingDistance = _runningStoppingDistance;
            }

            _animator.SetBool("IsMoving", true);
        }
        else
        {
            walkPressed = false;
            runPressed = false;
            _animator.SetBool("IsMoving", false);
        }
        HandleMovement();

        if (_debug)
        {
            _debugger.Update();
        }
        // Debug.Log($"agent has Path: {_agent.hasPath}, agent remaining distance: {_agent.remainingDistance}, agent stopping distance: {_agent.stoppingDistance}, walkPressed: {walkPressed}, runPressed: {runPressed}, PathPending: {_agent.pathPending}  IsPlayerInRange: {_dogSensor.PlayerIsInRange} Active state {_stateMachine._currentState}");

        // Debug.Log($"Player in Range {_dogSensor.PlayerIsInRange} state {_stateMachine._currentState.State.GetType().Name}");
    }

    void OnAnimatorMove()
    {
        if (walkPressed || runPressed)
        {
            _agent.speed = (_animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }



    #region StateMachine
    protected virtual void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    protected virtual void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
    void SetupTransitions()
    {
        _stateMachine = new StateMachine();

        //* DECLARE STATES
        var locomotionState = new DogLocomotion(_animator);
        var chaseState = new DogChaseState(_animator, this);
        var idleState = new DogIdleState(_animator, this);
        var playerSearchState = new PlayerSearchState(_animator, this);
        var attackState = new DogAttackState(_animator, this);
        // var closeRangeAttackState = new CloseRangeAttackState(_animator, this);

        // //* TRANSITIONS


        //* FROM SEARCHING FOR PLAYER TO IDLE
        // At(playerSearchState, idleState, new FuncPredicate(() => _dogSensor.PlayerIsInRange));

        //* FROM IDLE TO CHASE
        At(idleState, chaseState, new FuncPredicate(() => _dogSensor.EnemiesInSight));

        // //* FROM CHASING TO ATTACK
        At(chaseState, attackState, new FuncPredicate(() => _dogSensor.EnemiesInSight && CanAttack()));

        // //* FROM ATTACK TO CHASING
        At(attackState, chaseState, new FuncPredicate(() => !CanAttack()));

        // //* FROM CHASING TO FIND PLAYER
        At(chaseState, playerSearchState, new FuncPredicate(() => _isChasing && IsOutOfChaseRange()));
        //* TRANSITIONS END 

        //    ADD ANY TRANSITION
        Any(idleState, new FuncPredicate(() => _dogSensor.PlayerIsInRange && !_dogSensor.EnemiesInSight));
        Any(playerSearchState, new FuncPredicate(() => !_dogSensor.PlayerIsInRange && _currentTarget == null && _hasAccessToPlayer));
        Any(playerSearchState, new FuncPredicate(() => _playerIsCalling && _hasAccessToPlayer));
        Any(chaseState, new FuncPredicate(() => CanChase()));




        //* SET INITIAL STATE

        _stateMachine.SetState(idleState);

        // if (_aiController != null)
        // {
        //     if (_aiController.waypointArea != null)
        //     {
        //         _stateMachine.SetState(locomotionState);
        //     }
        //     else
        //     {
        //         _stateMachine.SetState(idleState);
        //     }
        // }

    }

    void UpdateState()
    {
        _stateTimer.Start();
        _stateMachine.Update();
    }
    #endregion


    #region Movement

    [TabGroup("Debug")]
    [Button("Move To Player")]
    public void MoveToPlayer()
    {
        _animator.SetBool("IsMoving", true);
        _agent.SetDestination(Player.Instance.transform.position);
        StartCoroutine(CheckIfAgentReachedDestination(() => _playerIsCalling = false));
        // Bite();
    }
    public void FindPlayer()
    {
        _playerIsCalling = true;
        if (_currentTarget != null)
        {
            _ignoreList.Add(_currentTarget.gameObject);
            _currentTarget = null;
        }
        MoveToPlayer();
        // Bite();
    }


    [TabGroup("Debug")]
    public void MoveToTarget(Transform target, Vector3 direction = default)
    {
        if (target != null)
        {
            _currentTarget = target;
        }
        if (direction != default)
        {
            _agent.SetDestination(direction);
        }
        else
        {
            _agent.SetDestination(target.position);
        }
        _animator.SetBool("IsMoving", true);
        StartCoroutine(CheckIfAgentReachedDestination());
        // Bite();
    }

    IEnumerator CheckIfAgentReachedDestination(Action OnDestinationReached = null)
    {
        yield return new WaitForSeconds(0.5f);
        while (_agent.remainingDistance >= _agent.stoppingDistance && !_agent.pathPending)
        {
            yield return null;
        }

        if (_currentTarget != null)
        {
            Vector3 lookPos = _currentTarget.position - transform.position;
            lookPos.y = 0; // Keep only horizontal rotation
            if (lookPos != Vector3.zero)
                transform.DORotate(Quaternion.LookRotation(lookPos).eulerAngles, 0.5f);
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);
        }
        _animator.SetBool("IsMoving", false);
        OnDestinationReached?.Invoke();
    }

    void HandleMovement()
    {
        if (runPressed)
        {
            currentSpeed = maxRun;
        }
        if (!runPressed)
        {
            currentSpeed = maxWalk;
        }
        if (walkPressed && (w_movement < currentSpeed)) // If walking
        {
            w_movement += Time.deltaTime * acceleration;
        }
        if (walkPressed && !runPressed && w_movement > currentSpeed) // Slow down
        {
            w_movement -= Time.deltaTime * decelleration;

        }
        if (!walkPressed && w_movement > 0.0f) // If no longer walking
        {
            w_movement -= Time.deltaTime * decelleration;
        }

        _animator.SetTrigger("Blink_tr"); // Blink will continue unless asleep or dead
        _animator.SetFloat("Movement_f", w_movement); // Set movement speed for all required parameters
    }


    bool IsOutOfChaseRange()
    {

        // Debug.Log("Distance from player " + Vector3.Distance(transform.position, Player.Instance.transform.position));
        if (Vector3.Distance(transform.position, Player.Instance.transform.position) > _maxEnemyFollowDistance)
        {
            return true;
        }
        return false;
    }
    #endregion
    // void Start() // On start store dogKeyCodes
    // {
    //     dogAnim = GetComponent<Animator>(); // Get the animation component
    //     currentSpeed = 0.0f;
    //     DogNewTypes = new string[]
    //     {
    //     "Coyote",
    //     "Dalmatian",
    //     "DalmatianCollar",
    //     "Doberman",
    //     "DobermanCollar",
    //     "Fox",
    //     "GermanShepherd",
    //     "GermanShepherdCollar",
    //     "GoldenRetriever",
    //     "GoldenRetrieverCollar",
    //     "DogGreyhound",
    //     "GreyhoundCollar",
    //     "HellHound",
    //     "Husky",
    //     "HuskyCollar",
    //     "Labrador",
    //     "LabradorCollar",
    //     "Pointer",
    //     "PointerCollar",
    //     "Ridgeback",
    //     "RidgebackCollar",
    //     "Robot",
    //     "Scifi",
    //     "Shiba",
    //     "Shiba_Collar",
    //     "Wolf",
    //     "ZombieDoberman",
    //     "ZombieGermanShepherd"
    //     };
    //     dogKeyCodes = new KeyCode[]
    //     {
    //     Attack,
    //     secondAttack,
    //     forward,
    //     backward,
    //     left,
    //     right,
    //     action,
    //     jump,
    //     run,
    //     sit,
    //     sleep,
    //     ExitPlaymode,
    //     Death,
    //     Reset,
    //     action1,
    //     action2,
    //     action3,
    //     action4,
    //     action5,
    //     action6,
    //     action7,
    //     action8,
    //     action9,
    //     action10,
    //     action11,
    //     action12,
    //     action13
    //     };
    //     dogLabels = new string[] // Labels for UI
    //     {
    //     "Attack: ",
    //     "Second Attack: ",
    //     "Forward: ",
    //     "Backward: ",
    //     "Left: ",
    //     "Right: ",
    //     "Action: ",
    //     "Jump: ",
    //     "Run: ",
    //     "Sit: ",
    //     "Sleep: ",
    //     "Exit Playmode: ",
    //     "Death: ",
    //     "Reset: ",
    //     "Action 1: ",
    //     "Action 2: ",
    //     "Action 3: ",
    //     "Action 4: ",
    //     "Action 5: ",
    //     "Action 6: ",
    //     "Action 7: ",
    //     "Action 8: ",
    //     "Action 9: ",
    //     "Action 10: ",
    //     "Action 11: ",
    //     "Action 12: ",
    //     "Action 13: "
    //     };
    //     guiStyle.fontSize = 18;
    //     guiStyle.normal.textColor = Color.black;
    //     children = GetComponentsInChildren<Transform>();
    //     for (int x = 0; x < children.Length; x++) // Search for dog names
    //     {
    //         if (children[x].name.Contains("Dogs"))
    //         {
    //             getDogName = children[x];
    //         }
    //     }
    //     newSpawn = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    // }
    IEnumerator DogActions(int actionType) // Dog action coroutine
    {
        dogActionEnabled = true; // Enable the dog animation flag
        _animator.SetInteger("ActionType_int", actionType); // Enable Animation
        yield return new WaitForSeconds(countDown); // Countdown
        _animator.SetInteger("ActionType_int", 0); // Disable animation
        dogActionEnabled = false; // Disable the dog animation flag
    }


    #region Attack

    [TabGroup("Debug")]
    [Button("Bite")]
    public async void Bite()
    {
        _animator.SetBool("AttackReady_b", true);
        _animator.SetInteger("AttackType_int", 1);

        await Task.Delay(800);
        _animator.SetInteger("AttackType_int", 0);
        RayCastToTarget();
        // float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;



        // Debug.Log(" animation length: " + animationLength);

        // ResetAnimation("AttackReady_b", animationLength);
        // ResetAnimation("AttackType_int", 0, animationLength);


    }

    bool CanAttack()
    {
        if (_currentTarget != null && Vector3.Distance(_mouthTransform.transform.position, _currentTarget.transform.position) <= _combatDistance + 0.1f)
        {
            return true;
        }
        return false;
    }

    bool CanChase()
    {

        if (_dogSensor.EnemiesInSight && !CanAttack() && !IsOutOfChaseRange())
        {
            return true;
        }
        return false;
    }

    [TabGroup("Debug")]
    [Button("RayCastToTarget")]
    void RayCastToTarget()
    {
        if (_currentTarget != null)
        {
            Collider[] colliders = Physics.OverlapSphere(_mouthTransform.transform.position, _mouthTransform.transform.localScale.x, _enemyLayer);
            if (colliders.Length > 0)
            {

                if (colliders[0].TryGetComponent(out vHealthController health))
                {
                    if (_mouthTransform != null)
                    {
                        _mouthTransform.ApplyDamage(colliders[0], _currentTarget.transform.position);
                    }
                    Debug.Log("colliders: " + colliders[0].name);
                }

                else
                {
                    Debug.Log(" damage not applied");
                }
            }

        }
    }


    #endregion
    async void ResetAnimation(string boolName, float delay)
    {
        await Task.Delay((int)(delay * 1000));
        _animator.SetBool(boolName, false);
    }
    async void ResetAnimation(string intName, int index, float delay)
    {
        await Task.Delay((int)(delay * 1000));
        _animator.SetInteger(intName, index);
    }

    void InitDebugger()
    {
        Debug.Log("Init Debugger");
        _debugger = new(this);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, _combatDistance));
    }
}

public enum Mode
{
    OFFENSIVE,
    EVASIVE
}

public class DogLocomotion : BaseState
{
    public DogLocomotion(Animator animator) : base(animator)
    {

    }
}
public class DogChaseState : BaseState
{
    Dog _dog;
    Transform _target;


    public DogChaseState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }

    public override void OnEnter()
    {
        _target = _dog._dogSensor.GetClosestEnemy().transform;

        _dog._isChasing = true;
        _dog._shouldRun = true;
        _dog._agent.stoppingDistance = _dog._combatDistance;
    }
    public override void OnExit()
    {

        _dog._isChasing = false;
    }

    public override void Update()
    {
        // Vector3 position = _target.position - new Vector3(0, 0, _dog._combatDistance);
        _dog.MoveToTarget(_target.transform);
    }
}
public class DogIdleState : BaseState
{

    Dog _dog;
    public DogIdleState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }
    public override void OnEnter()
    {
        // Debug.Log("Dog entered idle");
        _animator.SetBool("Sit_b", true);
        if (_dog._currentTarget != null)
        {
            _dog._currentTarget = null;
        }
    }
    public override void OnExit()
    {
        // Debug.Log("Dog exited idle");
        _animator.SetBool("Sit_b", false);
    }
}

public class PlayerSearchState : BaseState
{
    readonly Dog _dog;

    public PlayerSearchState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }

    public override void OnEnter()
    {
        _dog._searchingForPlayer = true;
    }
    public override void OnExit()
    {
        _dog._searchingForPlayer = false;
    }

    public override void Update()
    {
        Dog.Instance.MoveToPlayer();
    }
}

public class DogAttackState : BaseState
{

    Dog _dog;
    float _attackCoolDown = 2f;
    float _timeSinceLastAttack;
    public DogAttackState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }


    public override void OnEnter()
    {
        _timeSinceLastAttack = Time.time;
        _animator.SetBool("IsMoving", false);
        _animator.SetBool("AttackReady_b", true);
    }

    public override void OnExit()
    {
        _timeSinceLastAttack = Time.time;
        _animator.SetBool("AttackReady_b", false);
        _animator.SetInteger("AttackType_int", 0);
    }

    public override void Update()
    {

        if (Time.time - _timeSinceLastAttack > _attackCoolDown)
        {
            Debug.Log("Attacking");
            _dog.Bite();
            _timeSinceLastAttack = Time.time;
        }
    }

}

[Serializable]
public class DogDebugger
{
    Dog _dog;
    NavMeshAgent _agent;
    [TabGroup("Agent")]
    public float _remainingDistance;
    [TabGroup("Agent")]
    public float _stoppingDistance;
    [TabGroup("Agent")]
    public bool _hasPath;
    [TabGroup("Agent")]
    public bool _pathPending;
    [TabGroup("Agent")]
    public bool _pathStale;
    [TabGroup("State")]
    public string _currentState;

    public void Update()
    {
        _remainingDistance = _agent.remainingDistance;
        _hasPath = _agent.hasPath;
        _stoppingDistance = _agent.stoppingDistance;
        _pathPending = _agent.pathPending;
        _pathStale = _agent.isPathStale;
        _currentState = _dog._stateMachine._currentState.State.GetType().Name;
    }

    public DogDebugger(Dog dog)
    {
        _dog = dog;
        _agent = dog._agent;
        _currentState = _dog._stateMachine._currentState.State.GetType().Name;
    }
}