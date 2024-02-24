using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerStates : MonoBehaviour
{
    [BoxGroup("States")]
    [SerializeField]
    private List<PlayerBaseState> _states;

    [BoxGroup("States")]
    [SerializeField]
    private PlayerBaseState _defaultState;

    private List<PlayerBaseState> _instantiatedStates = new List<PlayerBaseState>();

    private PlayerBaseState _currentState;

    public PlayerBaseState currentState => _currentState;

    #region UnityMessages

    private void Awake()
    {
        foreach (var state in _states)
        {
            _instantiatedStates.Add(Instantiate(state));
        }

        foreach (var state in _instantiatedStates)
        {
            state.Initialize(GetComponent<Player>());
        }
    }

    private void Start()
    {
        SetState(_defaultState.GetType());
    }

    private void Update()
    {
        foreach (var state in _instantiatedStates)
        {
            if (state.IsActive)
                state.Update();
        }
    }

    private void FixedUpdate()
    {
        foreach (var state in _instantiatedStates)
        {
            if (state.IsActive)
                state.FixedUpdate();
        }
    }
    #endregion

    private void ChangeState(PlayerBaseState newState)
    {
        if (newState.IsRootState)
        {
            TransferSubState(newState);
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();

            Debug.Log("Entered STATE: " + newState.name);
        }
        else
        {
            if (!newState.IsActive)
            {
                if (_currentState.SetSubState(newState))
                {
                    newState.Enter();
                    Debug.Log("Entered SUBstate: " + newState.name);
                }
            }
        }
    }

    private void TransferSubState(PlayerBaseState newState)
    {
        if (_currentState == null) return;
        List<PlayerBaseState> invalidStates = newState.SetSubStates(_currentState.SubStates);

        string message = "Invalid states: ";
        foreach (var state in invalidStates)
        {
            message += state.name + ": ";
            state.Exit();
        }

        Debug.Log(message);
    }

    public T GetState<T>() where T : PlayerBaseState
    {
        foreach (PlayerBaseState state in _instantiatedStates)
        {
            if (state.GetType() == typeof(T))
            {
                return (T)state;
            }
        }
        return null;
    }

    public bool SetState<T>() where T : PlayerBaseState
    {
        return SetState(typeof(T));
    }

    public bool SetState(System.Type type)
    {
        foreach (PlayerBaseState state in _instantiatedStates)
        {
            if (state.GetType() == type)
            {
                ChangeState(state);
                return true;
            }
        }
        return false;
    }
}
