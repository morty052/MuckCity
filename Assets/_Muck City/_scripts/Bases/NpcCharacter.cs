using System;
using System.Collections;
using System.Collections.Generic;
using DialogueEditor;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
using Invector.vItemManager;
using Invector.vShooter;
using Sirenix.OdinInspector;
using UnityEngine;


public class LootHandler
{
    public readonly vItemCollection _vItemCollection;
    public LootHandler(vItemCollection itemCollection)
    {
        _vItemCollection = itemCollection;
    }

    public void AddItemToCollection(ItemReference item)
    {
        _vItemCollection.items.Add(item);
        // Debug.Log($"<color=orange> Adding {item.name} to Loot handler new count is {_vItemCollection.items.Count} items </color>");
    }
}

public class NpcCharacter : MonoBehaviour, IInteractable
{
    [SerializeField] protected SpecialCharacters _id;

    public bool IsHighlighted { get; }
    [SerializeField] protected string _name;

    [SerializeField] protected NpcSO _npcSO;

    [SerializeField] protected NPCConversation _activeConversation;



    [SerializeField] bool _canInteract;
    public bool _canBeSearched = false;

    public GameObject _lootHandler;

    public List<Role> _roles = new();

    public SpecialCharacters ID { get => _id; }

    public vShooterWeapon _defaultWeaponPrefab;

    public GameObject _weaponHolder;
    public ActionText _actionText;

    public LootHandler LootHandler { get; protected set; }

    protected GameObject _activeWeapon;

    protected vAIShooterManager _shooterManager;

    public bool CanInteract => _canInteract;
    public string InteractionPrompt => $"Talk to {_name}";
    public bool IsQuestGiver => _roles.Contains(Role.QUEST_GIVER);

    public NPCConversation ActiveConversation => _activeConversation;

    public NpcSO Data { get => _npcSO; }

    public GameObject GameObject => gameObject;

    protected StateMachine _stateMachine;

    Animator _animator;

    public vControlAI _aiController;
    public vFSMBehaviourController _fsmController;

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }



    protected virtual void Awake()
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

    public virtual void HandleMessages(string message)
    {
        Debug.Log("HandleMessages" + message);
        EquipWeapon();
    }

    [Button("Equip Weapon")]
    void EquipWeapon()
    {
        GameObject w = Instantiate(_defaultWeaponPrefab.gameObject, _weaponHolder.transform);
        w.transform.localPosition = Vector3.zero;
        _activeWeapon = w.transform.parent.gameObject;
        _shooterManager.SetRightWeapon(w);
    }


    public virtual void Interact()
    {

    }

    public virtual void StartConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(_activeConversation);
        GameEventsManager.Instance.OnConversationStarted(_activeConversation);
    }

    public virtual void PrepareInteraction()
    {
        if (_canInteract)
        {
            HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
            Player.Instance.SetInteractableObject(this);
        }
    }

    public virtual void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
        Player.Instance.SetInteractableObject(null);
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

    public void ToggleDrawAttention()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDie()
    {
        StartCoroutine(DelayedInvoke(2, () =>
        {
            _canBeSearched = true;
            _lootHandler.gameObject.SetActive(true);
        }));
    }

    public void GiveSearchResultItems()
    {
        if (!_canBeSearched) return;
        Debug.Log("Loot Succesfull");
        Destroy(_lootHandler.gameObject);
    }

    public void ActivateSearch()
    {
        _canBeSearched = true;
        _actionText.gameObject.SetActive(true);
        Player.Instance.SetInteractableObject(this);
    }
    public void DisableSearch()
    {
        _canBeSearched = false;
        _actionText.gameObject.SetActive(false);
        Player.Instance.SetInteractableObject(null);
    }


    IEnumerator DelayedInvoke(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
