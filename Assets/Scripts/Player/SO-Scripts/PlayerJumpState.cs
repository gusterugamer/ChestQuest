using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.PlayerStates
{
    [CreateAssetMenu(fileName = "PlayerJumpStateSO", menuName = "ChestQuest/Player/States/JumpState")]
    public class PlayerJumpState : PlayerBaseState
    {
        private CoroutineHandle _chWaitForJumpToFinish = default;

        public override void Enter()
        {
            base.Enter();
            Jump();
            _chWaitForJumpToFinish = Timing.RunCoroutine(WaitForJumpToFinish(), _player.gameObject);
        }

        public override void Exit()
        {
            base.Exit();
            Timing.KillCoroutines(_chWaitForJumpToFinish);
        }

        private void Jump()
        {
            _player.BlackBoard.MotionVector.y = CalculateVelocity();
        }

        private float CalculateVelocity()
        {
            return Mathf.Sqrt(2f * Mathf.Abs(_player.Config.JumpGravity) * _player.Config.JumpHeight);
        }

        private IEnumerator<float> WaitForJumpToFinish()
        {
            while (_player.BlackBoard.MotionVector.y > 0.0f)
                yield return 0f;

            _player.PlayerStates.SetState<PlayerFallingState>();
        }
    }
}
