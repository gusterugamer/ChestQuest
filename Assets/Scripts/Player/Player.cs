using Sirenix.OdinInspector;
using System;
using UnityEngine;
using GusteruStudio.PlayerStates;
using System.Collections.Generic;
using MEC;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStates))]
[RequireComponent(typeof(Rigidbody))]
public sealed class Player : MonoBehaviour
{
    [BoxGroup("Config")]
    [SerializeField] private PlayerConfigSO _playerConfig;

    [BoxGroup("Components")]
    [SerializeField] private Rigidbody _rigidBody;

    [BoxGroup("Components")]
    [SerializeField] private Chronometer _chronometer;

    public LayerMask groundLayer;

    private Chronometer _chronometerInstance;

    private CoroutineHandle _chCheckGrounded = default;

    public PlayerConfigSO Config => _playerConfig;

    public PlayerStates PlayerStates { private set; get; }

    public CapsuleCollider CapsuleCollider { private set; get; }

    public Rigidbody Rigidbody => _rigidBody;

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

    private void Start()
    {
        _chCheckGrounded = Timing.RunCoroutine(CheckGrounded(), gameObject);
    }

    private IEnumerator<float> CheckGrounded()
    {
        Vector3 position = transform.position + Vector3.down * 0.6f;

        while (true)
        {
            BlackBoard.isGrounded = Physics.SphereCast(transform.position, 0.5f /* change this */,Vector3.down, out RaycastHit _, 0.6f,groundLayer) && Rigidbody.velocity.y <= 0f;
            yield return 0f;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        onControllerColliderHit?.Invoke(hit);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.down * 0.6f, 0.5f);
    }
}
