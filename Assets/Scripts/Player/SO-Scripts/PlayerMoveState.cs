using GusteruStudio.Input;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GusteruStudio.PlayerStates {
    [CreateAssetMenu(fileName = "PlayerMoveStateSO", menuName = "ChestQuest/Player/States/MoveState")]
    public class PlayerMoveState : PlayerBaseState
    {
        private CharacterController _controller = null;

        private CoroutineHandle _chMoveForward = default;

        private CoroutineHandle _chApplyGravity = default;

        private Vector2 _keyboardInput = Vector2.zero;


        private void SubToEvents()
        {
            InputEvents.Jump.performed += JumpEventProcessor;
            InputEvents.Move.performed += Move_performed;
            InputEvents.Move.canceled += Move_canceled;
        }

        private void UnsubFromEvents()
        {
            InputEvents.Jump.performed -= JumpEventProcessor;
            InputEvents.Move.performed -= Move_performed;
            InputEvents.Move.canceled -= Move_canceled;
        }

        #region Input
        private void Move_canceled(InputAction.CallbackContext obj)
        {
            _keyboardInput = Vector2.zero;
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            _keyboardInput = obj.ReadValue<Vector2>();
        }

        private void JumpEventProcessor(InputAction.CallbackContext inputAction)
        {
            Jump();
        }
        #endregion

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

            SubToEvents();
           
        }

        public override void Exit()
        {
            base.Exit();
            Timing.KillCoroutines(_chMoveForward);
            Timing.KillCoroutines(_chApplyGravity);
            UnsubFromEvents();
        }

        private void Jump()
        {
            if (_player.CharacterController.isGrounded)
                _player.PlayerStates.SetState<PlayerJumpState>();
        }

        private IEnumerator<float> Move()
        {
            yield return 0;
            while (true)
            {
                Vector3 moveVector = new Vector3(_keyboardInput.x, 0.0f, _keyboardInput.y).normalized * _player.Config.RunSpeed;
                moveVector.y = _player.BlackBoard.MotionVector.y;
                _player.BlackBoard.MotionVector = moveVector;
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
}
