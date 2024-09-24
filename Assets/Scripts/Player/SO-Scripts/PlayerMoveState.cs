using GusteruStudio.Input;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GusteruStudio.PlayerStates
{
    [CreateAssetMenu(fileName = "PlayerMoveStateSO", menuName = "ChestQuest/Player/States/MoveState")]
    public class PlayerMoveState : PlayerBaseState
    {
        private CharacterController _controller = null;

        private CoroutineHandle _chMoveForward = default;

        private CoroutineHandle _chApplyGravity = default;

        private CoroutineHandle _chRotate = default;

        private CoroutineHandle _chMovementInput = default;

        private Vector2 _movementInput = Vector2.zero;

        private void SubToEvents()
        {
            InputEvents.Jump.performed += JumpEventProcessor;

            InputEvents.Move.performed += Move_performed;

            InputEvents.Rotate.performed += Rotate_performed;
            InputEvents.Rotate.canceled += Rotate_canceled;
        }

        private void UnsubFromEvents()
        {
            InputEvents.Jump.performed -= JumpEventProcessor;
            InputEvents.Move.performed -= Move_performed;
            InputEvents.Rotate.performed -= Rotate_performed;
            InputEvents.Rotate.canceled -= Rotate_canceled;
        }

        #region Input

        private void Rotate_performed(InputAction.CallbackContext obj)
        {
            _chRotate = Timing.RunCoroutine(Rotate());
        }

        private void Rotate_canceled(InputAction.CallbackContext obj)
        {
            Timing.KillCoroutines(_chRotate);
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            _movementInput = obj.ReadValue<Vector2>();

            if (!_chMovementInput.IsRunning)
                _chMovementInput = Timing.RunCoroutine(ReadMovementInput());
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
            Timing.KillCoroutines(_chRotate);
            Timing.KillCoroutines(_chMovementInput);
            UnsubFromEvents();
        }

        private void Jump()
        {
            if (_player.CharacterController.isGrounded)
                _player.PlayerStates.SetState<PlayerJumpState>();
        }

        private IEnumerator<float> ReadMovementInput()
        {
            while(_movementInput != Vector2.zero)
            {
                _movementInput = InputEvents.Move.ReadValue<Vector2>();
                Vector3 movement = _player.transform.forward * _movementInput.y + _player.transform.right * _movementInput.x;
                movement.y = 0;
                Vector3 moveVector = movement.normalized * _player.Config.RunSpeed;
                moveVector.y = _player.BlackBoard.MotionVector.y;

                _player.BlackBoard.MotionVector = moveVector;
                yield return 0;
            }
            _player.BlackBoard.MotionVector = new Vector3(0.0f,_player.BlackBoard.MotionVector.y,0.0f);
        }

        private IEnumerator<float> Rotate()
        {
            while (true)
            {
                Vector3 currentEulerRotation = _controller.transform.rotation.eulerAngles;
                Vector2 mouseXDelta = InputEvents.MouseDelta.ReadValue<Vector2>() * Time.deltaTime * _player.Config.RotationSpeed;

                Quaternion rotation = Quaternion.Euler(currentEulerRotation.x, currentEulerRotation.y + mouseXDelta.x, currentEulerRotation.z);
                _controller.transform.rotation = rotation;

                yield return 0;
            }
        }

        private IEnumerator<float> Move()
        {
            while (true)
            {
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
