using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig",menuName = "ChestQuest/Player/Config")]
public class PlayerConfigSO : ScriptableObject
{
    [BoxGroup("Settings")]
    [SerializeField]
    private float _groundedGravity = -2f;

    //[BoxGroup("Settings")]
    //private float _rampGravity = -100f;

    [BoxGroup("Settings")]
    [SerializeField]
    private float _jumpGravity = -9.81f;

    [BoxGroup("Settings")]
    [SerializeField]
    private float _jumpHeight = 5f;

    [BoxGroup("Settings")]
    [SerializeField]
    private float _runSpeed = 5f;

    [BoxGroup("Settings")]
    [SerializeField]
    private float _sideStepSpeed = 15f;

    public float GroundedGravity { get => _groundedGravity; }
    public float JumpGravity { get => _jumpGravity; }
    public float JumpHeight { get => _jumpHeight; }
    public float RunSpeed { get => _runSpeed;}
    public float SideStepSpeed { get => _sideStepSpeed;}
  //  public float RampGravity { get => _rampGravity; }
}
