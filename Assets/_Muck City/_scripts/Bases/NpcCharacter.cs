using System.Collections.Generic;
using DialogueEditor;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
using UnityEngine;

public class NpcCharacter : MonoBehaviour, IInteractable
{
    [SerializeField] protected SpecialCharacters _id;
    [SerializeField] protected string _name;

    [SerializeField] protected NpcSO _npcSO;

    [SerializeField] protected NPCConversation _activeConversation;



    [SerializeField] bool _canInteract;


    public List<Role> _roles = new();

    public SpecialCharacters ID { get => _id; }
    public bool CanInteract => _canInteract;
    public string InteractionPrompt => $"Talk to {_name}";
    public bool IsQuestGiver => _roles.Contains(Role.QUEST_GIVER);

    public NPCConversation ActiveConversation => _activeConversation;

    public NpcSO Data { get => _npcSO; }

    protected StateMachine _stateMachine;

    Animator _animator;

    public vControlAI _aiController;
    public vFSMBehaviourController _fsmController;



    void Awake()
    {
        if (_npcSO != null)
        {
            _animator = GetComponent<Animator>();
            _aiController = GetComponent<vControlAI>();
            _fsmController = GetComponent<vFSMBehaviourController>();
            SetupData();
            // SetupTransitions();
        }
    }

    // void Update()
    // {
    //     _stateMachine?.Update();
    // }


    protected virtual void SetupData()
    {
        _name = _npcSO._name;
        _id = _npcSO._id;
        _roles = _npcSO._roles;
    }


    public virtual void Interact()
    {

    }

    public virtual void StartConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(_activeConversation);
        GameEventsManager.Instance.OnConversationStarted(_activeConversation);
    }

    public void PrepareInteraction()
    {
        if (_canInteract)
        {
            HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
        }
    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    protected virtual void SetupTransitions()
    {
        _stateMachine = new StateMachine();

        //* DECLARE STATES
        var locomotionState = new LocomotionState(_animator, this);
        var chaseState = new ChaseState(_animator, this);
        var idleState = new IdleState(_animator, this);
        // var closeRangeAttackState = new CloseRangeAttackState(_animator, this);

        // //* TRANSITIONS

        At(idleState, locomotionState, new FuncPredicate(() => _aiController.waypointArea != null));
        At(locomotionState, chaseState, new FuncPredicate(() => _aiController.currentTarget != null));

        // //* FROM CHASING TO BITING
        // At(chaseState, closeRangeAttackState, new FuncPredicate(() => _attackSensor.IsTargetInRange));

        // //* FROM BITING TO CHASING
        // At(closeRangeAttackState, chaseState, new FuncPredicate(() => !_attackSensor.IsTargetInRange));
        //* TRANSITIONS END


        //* SET INITIAL STATE

        if (_aiController != null)
        {
            if (_aiController.waypointArea != null)
            {
                _stateMachine.SetState(locomotionState);
            }
            else
            {
                _stateMachine.SetState(idleState);
            }
        }

    }

    protected virtual void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    protected virtual void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}
