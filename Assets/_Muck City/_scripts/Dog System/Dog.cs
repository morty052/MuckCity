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
public class Dog : MonoBehaviour
{

    public static Dog Instance { get; private set; }
    public LayerMask _enemyLayer;
    Animator _animator;// Animator for the assigned dog

    CountdownTimer _stateTimer;// Timer for the current state

    StateMachine _stateMachine;// State machine for the dog

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

    public NavMeshAgent _agent;

    [HideInInspector]
    public DogSensor _dogSensor;

    [TabGroup("Movement")]
    public float _walkingStoppingDistance = 1.4f;
    [TabGroup("Movement")]
    public float _runningStoppingDistance = 2.8f;
    public float _combatDistance = 1f;
    public float _attackRange = 1f;
    public float _chaseDistance = 2.8f;

    [TabGroup("Movement")]
    public bool walkPressed = false;

    [TabGroup("Movement")]
    public bool runPressed = false;

    [TabGroup("Movement")]

    public bool _shouldRun;

    public bool _searchingForPlayer = false;

    public Transform _currentTarget;

    [TabGroup("Attack")]
    public Transform _mouthTransform;




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

    // void OnEnable()
    // {
    //     DogSensor.OnPlayerExitRange += MoveToPlayer;
    // }

    void OnDisable()
    {
        // DogSensor.OnPlayerExitRange -= MoveToPlayer;
        _stateTimer.Dispose();
    }

    void Start()
    {
        _stateTimer = new CountdownTimer(_stateUpdateTime);
        _stateTimer.OnTimerStop += UpdateState;
        _stateTimer.Start();
    }


    void Update() // Update loop for dog actions
    {
        if (_animator.GetBool("IsMoving"))
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

        }
        else
        {
            walkPressed = false;
            runPressed = false;
        }
        HandleMovement();
        // Debug.Log($"agent has Path: {_agent.hasPath}, agent remaining distance: {_agent.remainingDistance}, agent stopping distance: {_agent.stoppingDistance}, walkPressed: {walkPressed}, runPressed: {runPressed}, PathPending: {_agent.pathPending}  IsPlayerInRange: {_dogSensor.PlayerIsInRange} Active state {_stateMachine._currentState}");

        Debug.Log($"Player in Range {_dogSensor.PlayerIsInRange} state {_stateMachine._currentState.State.GetType().Name}");
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
        var idleState = new DogIdleState(_animator);
        var playerSearchState = new PlayerSearchState(_animator, this);
        var attackState = new DogAttackState(_animator, this);
        // var closeRangeAttackState = new CloseRangeAttackState(_animator, this);

        // //* TRANSITIONS


        //* FROM SEARCHING FOR PLAYER TO IDLE
        At(playerSearchState, idleState, new FuncPredicate(() => _dogSensor.PlayerIsInRange));

        //* FROM IDLE TO CHASE
        At(idleState, chaseState, new FuncPredicate(() => _dogSensor.EnemiesInSight && _dogSensor.PlayerIsInRange && !_searchingForPlayer));

        // //* FROM CHASING TO ATTACK
        At(chaseState, attackState, new FuncPredicate(() => _dogSensor.EnemiesInSight && CanAttack()));

        // //* FROM BITING TO CHASING
        // At(closeRangeAttackState, chaseState, new FuncPredicate(() => !_attackSensor.IsTargetInRange));
        //* TRANSITIONS END 

        //    ADD ANY TRANSITION
        Any(idleState, new FuncPredicate(() => _dogSensor.PlayerIsInRange && !_dogSensor.EnemiesInSight));
        Any(playerSearchState, new FuncPredicate(() => !_dogSensor.PlayerIsInRange));



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

    [Button("Move To Player")]
    public void MoveToPlayer()
    {
        _agent.SetDestination(Player.Instance.transform.position);
        _animator.SetBool("IsMoving", true);
        StartCoroutine(CheckIfAgentReachedDestination());
        // Bite();
    }
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

    IEnumerator CheckIfAgentReachedDestination()
    {
        yield return new WaitForSeconds(0.5f);
        while (_agent.remainingDistance >= _agent.stoppingDistance && !_agent.pathPending)
        {
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
        // _agent.ResetPath();
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

    [Button("Bite")]
    public async void Bite()
    {
        _animator.SetBool("AttackReady_b", true);
        _animator.SetInteger("AttackType_int", 1);

        await Task.Delay(800);
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;

        RayCastToTarget();

        Debug.Log(" animation length: " + animationLength);

        ResetAnimation("AttackReady_b", animationLength);
        ResetAnimation("AttackType_int", 0, animationLength);


    }

    bool CanAttack()
    {
        if (_currentTarget != null && Vector3.Distance(_mouthTransform.position, _currentTarget.transform.position) <= _walkingStoppingDistance)
        {
            return true;
        }
        return false;
    }

    [Button("RayCastToTarget")]
    void RayCastToTarget()
    {
        if (_currentTarget != null)
        {
            Collider[] colliders = Physics.OverlapSphere(_mouthTransform.position, _mouthTransform.localScale.x, _enemyLayer);
            if (colliders.Length > 0)
            {
                Debug.Log("colliders: " + colliders[0].name);
                vDamage damage = new vDamage(10, false);
                colliders[0].GetComponent<vHealthController>().TakeDamage(damage);
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


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (_currentTarget != null)
        {
            Gizmos.DrawSphere(_mouthTransform.position, _mouthTransform.localScale.x);
        }
    }
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
    public DogChaseState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }

    public override void OnEnter()
    {

        _dog._shouldRun = true;
        _dog._agent.stoppingDistance = 1f;

        Transform target = _dog._dogSensor.GetClosestEnemy().transform;

        Vector3 position = target.position + new Vector3(0, 0, 3f);
        _dog.MoveToTarget(target);
    }

    public override void Update()
    {
        _dog._shouldRun = true;
        _dog.MoveToTarget(_dog._dogSensor.GetClosestEnemy().transform);
        Debug.Log("Chasing " + Vector3.Distance(_dog._mouthTransform.position, _dog._currentTarget.transform.position));
    }
}
public class DogIdleState : BaseState
{
    public DogIdleState(Animator animator) : base(animator)
    {

    }
    public override void OnEnter()
    {
        // Debug.Log("Dog entered idle");
        _animator.SetBool("Sit_b", true);
    }
    public override void OnExit()
    {
        // Debug.Log("Dog exited idle");
        _animator.SetBool("Sit_b", false);
    }
}

public class PlayerSearchState : BaseState
{
    Dog _dog;
    public PlayerSearchState(Animator animator, Dog dog) : base(animator)
    {
        _dog = dog;
    }

    public override void OnEnter()
    {
        // Debug.Log("Searching for player");
        Dog.Instance.MoveToPlayer();
        _dog._searchingForPlayer = true;
    }
    public override void OnExit()
    {
        // Debug.Log("stopped Searching for player");
        _dog._searchingForPlayer = false;
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
        // Vector3 direction = (_dog._currentTarget.position - _dog.transform.position).normalized; // (target.position - _dog.transform.position).normalized;
        // _dog.transform.DORotate(direction, 0.1f);
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