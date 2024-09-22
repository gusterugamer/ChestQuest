using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract partial class PlayerBaseState : ScriptableObject
{
    [BoxGroup("Settings")]
    [SerializeField]
    private bool _isRootState;

    [BoxGroup("States")]
    [OnCollectionChanged(After = "EditorCheckState")]
    [SerializeField]
    [Tooltip("States which are NOT root and can exist when the root state is changed")]
    private List<PlayerBaseState> _validSubStates;

    protected Player _player;

    protected bool _isActive = false;

    private List<PlayerBaseState> _subState = new List<PlayerBaseState>();

    private PlayerBaseState _superState = null;

    public bool IsRootState => _isRootState;

    public bool IsActive => _isActive;

    public List<PlayerBaseState> SubStates => _subState;

    public virtual void Initialize(Player player)
    {
        _player = player;
    }

    public virtual void Enter()
    {
        _isActive = true;
    }

    public virtual void Exit()
    {
        _isActive = false;
    }

    public List<PlayerBaseState> SetSubStates(List<PlayerBaseState> subStates)
    {
        _subState.Clear();
        List<PlayerBaseState> invalidStates = new List<PlayerBaseState>();

        string message = "Transfered states: ";
        foreach (PlayerBaseState state in subStates)
        {
            if (!SetSubState(state))
            {
                invalidStates.Add(state);
            }
            else
            {
                message += state + " ";
            }
        }
        Debug.Log(message);
        return invalidStates;
    }

    public bool SetSubState(PlayerBaseState newState)
    {
        if (IsStateValid(newState))
        {
            _subState.Add(newState);
            newState.SetSuperState(this);
            return true;
        }
        return false;
    }

    private void SetSuperState(PlayerBaseState state)
    {
        _superState = state;
    }

    private bool IsStateValid(PlayerBaseState state)
    {
        foreach (var validState in _validSubStates)
        {
            if (validState.GetType() == state.GetType())
            {
                return true;
            }
        }
        return false;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}
