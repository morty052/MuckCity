using UnityEngine;
using UnityEngine.AI;



public class AiAgent : MonoBehaviour
{

    protected readonly int HIT_ANIM = Animator.StringToHash("Hit");

    public readonly int DEATH_ANIM = Animator.StringToHash("Die");
    // [SerializeField] protected Sensor _chaseSensor;
    public NavMeshAgent _agent;

    // protected StateMachine _stateMachine;

    [SerializeField] protected Animator _animator;

    public Transform _mouthTransform;
    public int _maxDetectionRays;

    public float _biteRadius = 2f;
    public float _midRangeAttackRadius = 2f;
    public int _hp = 100;
    public bool _hasLookTarget = false;

    public Transform _playerLookAtTransform;

    // [SerializeField] protected Rig _lookRig;

    public LayerMask _environmentMask = new();

    public bool _canSeePlayer = false;

    public virtual void Bite()
    {

    }


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetupTransitions();
    }


    protected virtual void SetupTransitions()
    {

    }
    public virtual void StareAtPlayer()
    {

    }

    public virtual void HandleChase()
    {

    }


    // protected void DetectPlayer()
    // {
    //     float halfAngle = 90f; // Half of the 180-degree arc
    //     for (int i = 0; i < _maxDetectionRays; i++)
    //     {
    //         // Calculate the angle for each ray within the 90-degree arc
    //         float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / (_maxDetectionRays - 1));
    //         Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

    //         // Cast the ray
    //         if (Physics.SphereCast(_mouthTransform.position + Vector3.up, 0.5f, direction, out RaycastHit hit, _chaseSensor._detectionRange.radius, _environmentMask))
    //         {
    //             Debug.Log($"Hit: {hit.collider.name}");
    //             if (hit.collider.CompareTag("Player"))
    //             {
    //                 _canSeePlayer = true;
    //                 return;
    //             }
    //         }

    //         Debug.DrawRay(_mouthTransform.position + Vector3.up, direction * _chaseSensor._detectionRange.radius, Color.red, 0.1f);
    //     }
    //     _canSeePlayer = false;
    // }


    // protected virtual void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    // protected virtual void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

}
