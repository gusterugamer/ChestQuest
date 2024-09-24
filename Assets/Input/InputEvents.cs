using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GusteruStudio.Input
{
    public class InputEvents : MonoBehaviour
    {
        private static GameControls _gameControls;
        public static InputAction Jump => _gameControls.Player.Jump;
        public static InputAction Move => _gameControls.Player.Move;

        public static InputAction Rotate => _gameControls.Player.Rotate;
        public static InputAction MouseDelta => _gameControls.Player.Mouse;

        void Awake()
        {
            _gameControls = new GameControls();
            _gameControls.Enable();
            DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            _gameControls.Disable();
        }
    }
}
