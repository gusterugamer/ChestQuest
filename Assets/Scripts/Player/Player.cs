using Sirenix.OdinInspector;
using System;
using UnityEngine;
using GusteruStudio.PlayerStates;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStates))]
[RequireComponent(typeof(CharacterController))]
public sealed class Player : MonoBehaviour
{
    [BoxGroup("Config")]
    [SerializeField] private PlayerConfigSO _playerConfig;

    [BoxGroup("Components")]
    [SerializeField] private CharacterController _characterController;

    [BoxGroup("Components")]
    [SerializeField] private Chronometer _chronometer;

    private Chronometer _chronometerInstance;

    public PlayerConfigSO Config => _playerConfig;

    public PlayerStates PlayerStates { private set; get; }

    public CapsuleCollider CapsuleCollider { private set; get; }

    public CharacterController CharacterController => _characterController;

    public Chronometer Chronometer => _chronometerInstance;

    [BoxGroup("RuntimeValues")]
    [ShowInInspector]
    public BlackBoard BlackBoard { private set; get; } = new BlackBoard();

    public event Action<ControllerColliderHit> onControllerColliderHit;

    private void Awake()
    {
        PlayerStates = GetComponent<PlayerStates>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        _chronometerInstance = Instantiate(_chronometer);
        _chronometerInstance.Init(gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        onControllerColliderHit?.Invoke(hit);
    }
}
