using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.PlayerStates
{
    [CreateAssetMenu(fileName = "PlayerFallingStateSO", menuName = "ChestQuest/Player/States/FallingState")]
    public class PlayerFallingState : PlayerBaseState
    {
        private CoroutineHandle _chWaitForJumpToFinish = default;
        public override void Enter()
        {
            base.Enter();
            _chWaitForJumpToFinish = Timing.RunCoroutine(WaitForJumpToFinish(), _player.gameObject);
        }

        public override void Exit()
        {
            base.Exit();
            Timing.KillCoroutines(_chWaitForJumpToFinish);
        }

        private IEnumerator<float> WaitForJumpToFinish()
        {
            while (!_player.CharacterController.isGrounded)
                yield return 0f;

            _player.PlayerStates.SetState<PlayerGroundedState>();
        }
    }
}
