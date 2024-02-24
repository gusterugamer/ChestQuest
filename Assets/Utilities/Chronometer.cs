using GusteruStudio.Extensions;
using GusteruStudio.ReactiveVariables;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

//This class is a wrapper for time coroutines

[CreateAssetMenu(menuName = "ChestQuest/Utilities/Chronometer")]
public class Chronometer : ScriptableObject
{
    [NonSerialized] private Dictionary<CoroutineHandle, Action> _stopHandleAction = new Dictionary<CoroutineHandle, Action>();
    [NonSerialized] private Dictionary<CoroutineHandle, Action> _pauseHandleAction = new Dictionary<CoroutineHandle, Action>();
    [NonSerialized] private Dictionary<CoroutineHandle, Action> _resumeHandleAction = new Dictionary<CoroutineHandle, Action>();

    private GameObject _player;

    public void Init(GameObject player)
    {
        //Stop all timers to make sure there is no garbage left in registers ( e.g when the player has a power up on puase and changes scenes)
        StopAllTimers();
        _player = player;
    }

    public void StopAllTimers()
    {
        foreach (var stopAction in _stopHandleAction)
        {
            Timing.KillCoroutines(stopAction.Key);
            stopAction.Value.Fire();
        }
        _stopHandleAction.Clear();
        _pauseHandleAction.Clear();
        _resumeHandleAction.Clear();
    }

    public void StopTimer(ref CoroutineHandle ch)
    {
        if (_stopHandleAction.ContainsKey(ch))
        {
            Timing.KillCoroutines(ch);
            _stopHandleAction[ch].Fire();
            _stopHandleAction.Remove(ch);
        }
    }

    public void PauseTimer(ref CoroutineHandle ch)
    {
        if (_pauseHandleAction.ContainsKey(ch))
        {
            if (!ch.IsAliveAndPaused && ch.IsRunning)
            {
                Timing.PauseCoroutines(ch);
                _pauseHandleAction[ch].Fire();
            }
        }
    }

    public void PauseTimerNoCallback(ref CoroutineHandle ch)
    {
        if (_pauseHandleAction.ContainsKey(ch))
        {
            Timing.PauseCoroutines(ch);
        }
    }

    public void ResumeTimer(ref CoroutineHandle ch)
    {
        if (_resumeHandleAction.ContainsKey(ch))
        {
            if (ch.IsAliveAndPaused)
            {
                Timing.ResumeCoroutines(ch);
                _resumeHandleAction[ch].Fire();
            }
        }
    }

    public void StartTimer(ref CoroutineHandle ch, FloatVariable currentTime, TimerEvents events)
    {
        ch = Timing.RunCoroutine(CountDown(currentTime, events.onComplete).CancelWith(_player));

        RegisterAction(_stopHandleAction, ref ch, events.onStop);
        RegisterAction(_pauseHandleAction, ref ch, events.onPause);
        RegisterAction(_resumeHandleAction, ref ch, events.onResume);
    }

    private void RegisterAction(Dictionary<CoroutineHandle, Action> actionHandle, ref CoroutineHandle ch, Action action)
    {
        if (actionHandle.ContainsKey(ch))
            actionHandle[ch] = action;
        else
            actionHandle.Add(ch, action);
    }

    private IEnumerator<float> CountDown(FloatVariable currentTime, Action onComplete)
    {
        while (currentTime.Value > 0.0f)
        {
            currentTime.Value -= Time.deltaTime;
            yield return 0;
        }
        onComplete.Fire();
    }
}

//This is just a configuration struct for classes that use Chronometer
//You fill it with the methods you want to call when the specific event is triggered
public struct TimerEvents
{
    public Action onStop;
    public Action onPause;
    public Action onComplete;
    public Action onResume;
}
