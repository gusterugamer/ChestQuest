using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerMoveStateSO", menuName = "ChestQuest/Player/States/MoveState")]
public class PlayerMoveState : PlayerBaseState
{
    private CharacterController _controller = null;

    private CoroutineHandle _chMoveForward = default;

    private CoroutineHandle _chApplyGravity = default;

    public override void Initialize(Player player)
    {
        base.Initialize(player);
        _controller = player.CharacterController;
    }

    public override void Enter()
    {
        base.Enter();
        _player.BlackBoard.MotionVector = Vector3.zero;
        _chMoveForward = Timing.RunCoroutine(Move());
        _chApplyGravity = Timing.RunCoroutine(ApplyGravity());

        //SubToEvents
    }

    public override void Exit()
    {
        base.Exit();
        Timing.KillCoroutines(_chMoveForward);
        Timing.KillCoroutines(_chApplyGravity);
    }

    public override void Update()
    {

    }

    private void JumpEventProcessor(InputAction.CallbackContext inputAction)
    {
        Jump();
    }

    private void Jump()
    {
        if (_player.CharacterController.isGrounded)
            _player.PlayerStates.SetState<PlayerJumpState>();
    }

    private IEnumerator<float> Move()
    {
        while (true)
        {
            _player.BlackBoard.MotionVector.z = _player.Config.RunSpeed;
            _controller.Move(_player.BlackBoard.MotionVector * Time.deltaTime);
            yield return 0f;
        }
    }

    private IEnumerator<float> ApplyGravity()
    {
        while (true)
        {
            if (_player.BlackBoard.MotionVector.y > _player.Config.JumpGravity && !_player.CharacterController.isGrounded)
            {
                _player.BlackBoard.MotionVector.y = Mathf.Clamp(_player.BlackBoard.MotionVector.y + _player.Config.JumpGravity * Time.deltaTime, _player.Config.JumpGravity, Mathf.Infinity);
            }
            else if (_player.CharacterController.isGrounded)
            {
                _player.BlackBoard.MotionVector.y = _player.Config.GroundedGravity;
            }
            yield return 0f;
        }
    }
}
