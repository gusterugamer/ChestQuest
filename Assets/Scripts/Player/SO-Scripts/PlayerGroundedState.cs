using MEC;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerGroundedStateSO", menuName = "ChestQuest/Player/States/GroundedState")]
public class PlayerGroundedState : PlayerBaseState
{
    // private CoroutineHandle _chChangeGravity = default;
    private CoroutineHandle _chWaitPlayerToFall = default;

    override public void Initialize(Player player)
    {
        base.Initialize(player);

    }

    public override void Enter()
    {
        base.Enter();
        _player.PlayerStates.SetState<PlayerMoveState>();
        _player.BlackBoard.MotionVector.y = _player.Config.GroundedGravity;

        _chWaitPlayerToFall = Timing.RunCoroutine(WaitPlayerToFall(), _player.gameObject);
        //   _chChangeGravity = Timing.RunCoroutine(ChangeGravity(), Segment.FixedUpdate,_player.gameObject);

        InputEvents.Jump.performed += JumpEventProcessor;
    }

    private void JumpEventProcessor(InputAction.CallbackContext context)
    {
       _player.PlayerStates.SetState<PlayerJumpState>();
    }

    public override void Exit()
    {
        base.Exit();
        Timing.KillCoroutines(_chWaitPlayerToFall);
        // Timing.KillCoroutines(_chChangeGravity);
    }

    private IEnumerator<float> WaitPlayerToFall()
    {
        while (_player.CharacterController.isGrounded)
            yield return 0f;

        _player.PlayerStates.SetState<PlayerFallingState>();
    }

    //private IEnumerator<float> ChangeGravity()
    //{
    //    while(true)
    //    {
    //        if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hitInfo,Mathf.Infinity))
    //        {
    //            if (hitInfo.normal.y > 0.0f && hitInfo.normal.z > 0.0f)
    //            {
    //                _player.BlackBoard.MotionVector.y = _player.Config.RampGravity;
    //            }
    //            else
    //            {
    //                _player.BlackBoard.MotionVector.y = _player.Config.GroundedGravity;
    //            }
    //        }
    //        yield return 0;
    //    }
    //}
}
