using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputEvents : MonoBehaviour
{
    private static GameControls _gameControls;
    public static InputAction Jump => _gameControls.Player.Jump;

    void Awake()
    {
        _gameControls = new GameControls();
        DontDestroyOnLoad(gameObject);
        _gameControls.Enable();
    }
    void OnDestroy()
    {
        _gameControls.Disable();
    }
}
