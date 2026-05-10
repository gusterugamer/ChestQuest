# Unity StateTree Implementation Plan

A practical roadmap for building an Unreal-style **StateTree runtime in Unity**.

**Recommended target:** RPG / creature AI with hierarchical states, tasks, transitions, events, sensors, and optional utility scoring.

---

## Core idea

Build the runtime first. Do **not** start with a graph editor or full ScriptableObject authoring. First prove that the active state path, transitions, and task lifecycle work correctly in gameplay.

```text
StateTree = Hierarchical State Machine
          + Behavior Tree-style child selection
          + Tasks
          + Conditions
          + Transitions
          + Shared context / blackboard
          + Events
          + Optional utility scoring
```

### Recommended MVP

- Runtime tree built in C#.
- Active state path system.
- State `Enter / Tick / Exit` lifecycle.
- Parent and child states active together.
- `FirstValidChild` selection.
- Enter conditions and transition conditions.
- Tick transitions and event transitions.
- Transition priority.
- Shared `EnemyStateTreeContext`.
- Basic debug output for current path and last transition.

---

# 1. Unreal StateTree Concepts Mapped to Unity

| Unreal StateTree Feature | Unity Analogue | Implementation Note |
|---|---|---|
| StateTree Asset | C# builder first; ScriptableObject asset later | Avoid editor tooling until runtime is proven. |
| State | `StateNode` | Contains tasks, transitions, enter conditions, child states, and selection mode. |
| Active state path | `List<StateNode> _activePath` | Example: `Root → Alive → Combat → Chase`. |
| Enter Conditions | `Condition` classes | Small reusable predicates, such as `HasTarget` or `TargetInRange`. |
| Tasks | `StateTask` classes | Move, animate, attack, wait, rotate, cast spell, etc. |
| Transitions | `Transition` objects | Triggered by tick, event, or task/state completion. |
| Events | `StateTreeEvent` queue | Used for perception, damage, animation notifies, death, etc. |
| Parameters | Tuning fields / config assets | Attack range, speed, cooldowns, spell weights. |
| Context | `EnemyStateTreeContext` | Shared references and runtime data. |
| Evaluators / Global Tasks | Sensors / Services | Update distance, line of sight, target, cooldowns, threat. |
| Utility Selection | `UtilityConsideration` classes | Useful later for spell choice and non-rigid combat decisions. |
| Linked Assets / Subtrees | Reusable subtree assets | Add after ScriptableObject authoring exists. |
| Debugger | Inspector path + transition logs | Start simple before making a visual timeline. |

---

# 2. Target Architecture

```text
Enemy
├── EnemyStateTreeContext
├── EnemySensorUpdater
│   ├── TargetSensor
│   ├── DistanceSensor
│   └── LineOfSightSensor
├── EnemyStateTreeRunner
├── NavMeshAgent
└── Animator
```

## StateTree structure for a creature

```text
Root
├── Dead
└── Alive
    ├── Stunned
    ├── OutOfCombat
    │   ├── Idle
    │   └── Patrol
    └── Combat
        ├── CastSpell
        ├── AttackMelee
        ├── Chase
        └── Search
```

## Selection priority example

```text
Alive children:
1. Stunned
2. Combat
3. OutOfCombat

Combat children:
1. CastSpell
2. AttackMelee
3. Chase
4. Search
```

---

# 3. Implementation Phases

## Phase 1 — Runtime-only prototype

Start with a pure C# implementation. Do not use ScriptableObjects yet.

First target:

```text
Enemy enters Idle
Enemy sees player
Enemy transitions to Chase
Enemy reaches attack range
Enemy transitions to Attack
Enemy loses target
Enemy transitions back to Search/Idle
Enemy dies
Enemy transitions to Dead
```

This phase proves the execution model works.

## Phase 2 — Core runtime classes

Create these first:

```text
StateTreeRunner
StateNode
StateTask
Condition
Transition
StateTreeContext
StateTreeEvent
```

## Phase 3 — Active path system

The runner should not store one active state. It should store the full active path:

```csharp
private readonly List<StateNode> _activePath = new();
```

Example active path:

```text
Root → Alive → Combat → Chase
```

Every active state ticks:

```text
Tick Root
Tick Alive
Tick Combat
Tick Chase
```

## Phase 4 — Child state selection

When a state is entered, the runner should continue selecting children based on that state's selection mode.

Implement these first:

```text
EnterSelf
FirstValidChild
```

Add these later:

```text
RandomValidChild
HighestUtilityChild
WeightedRandomUtilityChild
```

## Phase 5 — Conditions and tasks

Conditions should be small and reusable.

Examples:

```text
HasTarget
TargetInRange
TargetOutOfRange
HealthBelow
IsDead
IsStunned
SpellReady
CooldownReady
```

Tasks should do one job.

Examples:

```text
MoveToTargetTask
PlayAnimationTask
WaitTask
AttackTask
CastSpellTask
RotateToTargetTask
StopMovementTask
```

## Phase 6 — Transitions

For the first version, support:

```text
OnTick
OnEvent
```

Add later:

```text
OnStateSucceeded
OnStateFailed
OnStateCompleted
```

Recommended transition checking order:

```text
1. Check deepest active state first.
2. Then check parent states upward.
3. Highest priority valid transition wins.
```

Priority convention:

```text
Death / hard interrupts: 1000
Stun / crowd control: 800
Combat reactions: 500
Normal state changes: 0
Cosmetic transitions: -100
```

## Phase 7 — Events

Use events for important external changes:

```text
SawTarget
LostTarget
TookDamage
AttackFinished
AnimationFinished
Died
ReachedDestination
```

Events are cleaner than checking everything every frame.

## Phase 8 — Debugging

Before building a visual editor, expose:

```text
Current active path
Last transition
Last event received
Failed enter condition
Current task statuses
```

Example log:

```text
[StateTree] Root / Alive / Combat / Chase
[StateTree] Chase → Attack because TargetInRangeCondition passed
[StateTree] Alive → Dead because IsDeadCondition passed
```

## Phase 9 — Task completion

After transitions work, add task status:

```text
Running
Succeeded
Failed
```

Also add state task completion policy:

```text
None
Any
All
```

Use `None` as the default. Otherwise helper tasks can accidentally complete the whole state.

## Phase 10 — ScriptableObjects and subtrees

Only add these once the runtime works.

Add:

```text
StateTreeAsset
StateAsset
TaskAsset
ConditionAsset
TransitionAsset
Reusable subtree assets
```

Important rule:

```text
ScriptableObject = shared config
Runtime instance = per-enemy state
```

---

# 4. Suggested Folder Structure

```text
Assets/
└── Game/
    └── AI/
        └── StateTree/
            ├── Runtime/
            │   ├── StateTreeRunner.cs
            │   ├── StateNode.cs
            │   ├── StateTask.cs
            │   ├── Condition.cs
            │   ├── Transition.cs
            │   ├── StateTreeContext.cs
            │   ├── StateTreeEvent.cs
            │   └── StateTreeEnums.cs
            │
            ├── Tasks/
            │   ├── MoveToTargetTask.cs
            │   ├── PlayAnimationTask.cs
            │   ├── WaitTask.cs
            │   ├── AttackTask.cs
            │   └── RotateToTargetTask.cs
            │
            ├── Conditions/
            │   ├── HasTargetCondition.cs
            │   ├── TargetInRangeCondition.cs
            │   ├── HealthBelowCondition.cs
            │   ├── IsDeadCondition.cs
            │   └── CooldownReadyCondition.cs
            │
            └── Examples/
                └── EnemyStateTreeBuilder.cs
```

---

# 5. Core Runtime Classes

## `EnemyStateTreeContext`

This is the Unity equivalent of Unreal context + parameters + evaluator outputs.

```csharp
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateTreeContext : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Target;

    [Header("Runtime Data")]
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float DistanceToTarget;
    public bool HasLineOfSight;
    public bool IsStunned;
    public bool IsDead;

    [Header("Tuning")]
    public float AttackRange = 2f;
    public float ChaseRange = 15f;
    public float MoveSpeed = 3.5f;

    public float HealthPercent => MaxHealth <= 0f ? 0f : Health / MaxHealth;
    public bool HasTarget => Target != null;
}
```

Later, split it into:

```text
EnemyContext
EnemyCombatData
EnemyPerceptionData
EnemyMovementData
EnemyAnimationData
```

## `Condition`

```csharp
public abstract class Condition
{
    public abstract bool Evaluate(EnemyStateTreeContext context);
}
```

## `StateTask`

```csharp
public abstract class StateTask
{
    public virtual void OnEnter(EnemyStateTreeContext context) {}
    public virtual void OnTick(EnemyStateTreeContext context, float deltaTime) {}
    public virtual void OnExit(EnemyStateTreeContext context) {}

    public virtual TaskStatus Status => TaskStatus.Running;
}

public enum TaskStatus
{
    Running,
    Succeeded,
    Failed
}
```

## `StateNode`

```csharp
using System.Collections.Generic;

public class StateNode
{
    public string Name;

    public List<Condition> EnterConditions = new();
    public List<StateTask> Tasks = new();
    public List<Transition> Transitions = new();
    public List<StateNode> Children = new();

    public StateSelectionMode SelectionMode = StateSelectionMode.FirstValidChild;
    public TaskCompletionMode CompletionMode = TaskCompletionMode.None;

    public StateNode(string name)
    {
        Name = name;
    }

    public bool CanEnter(EnemyStateTreeContext context)
    {
        foreach (var condition in EnterConditions)
        {
            if (!condition.Evaluate(context))
                return false;
        }

        return true;
    }

    public void Enter(EnemyStateTreeContext context)
    {
        foreach (var task in Tasks)
            task.OnEnter(context);
    }

    public void Tick(EnemyStateTreeContext context, float deltaTime)
    {
        foreach (var task in Tasks)
            task.OnTick(context, deltaTime);
    }

    public void Exit(EnemyStateTreeContext context)
    {
        for (int i = Tasks.Count - 1; i >= 0; i--)
            Tasks[i].OnExit(context);
    }
}
```

## `Transition`

```csharp
using System.Collections.Generic;

public class Transition
{
    public StateNode Target;
    public TransitionTrigger Trigger;
    public List<Condition> Conditions = new();
    public int Priority;
    public string EventName;

    public Transition(StateNode target, TransitionTrigger trigger)
    {
        Target = target;
        Trigger = trigger;
    }

    public bool CanTransition(EnemyStateTreeContext context)
    {
        foreach (var condition in Conditions)
        {
            if (!condition.Evaluate(context))
                return false;
        }

        return true;
    }
}
```

## Enums

```csharp
public enum StateSelectionMode
{
    EnterSelf,
    FirstValidChild,
    RandomValidChild,
    HighestUtilityChild,
    WeightedRandomUtilityChild
}

public enum TransitionTrigger
{
    OnTick,
    OnEvent,
    OnStateSucceeded,
    OnStateFailed,
    OnStateCompleted
}

public enum TaskCompletionMode
{
    None,
    Any,
    All
}
```

---

# 6. The Most Important Algorithm: Changing Active Paths

This is the hardest and most important part of the system.

When transitioning from one state to another, find the common parent between the old active path and the new active path.

Example:

```text
Old path:
Root → Alive → Combat → Chase

New path:
Root → Alive → Combat → Attack

Common path:
Root → Alive → Combat

Exit:
Chase

Enter:
Attack
```

## More drastic transition

```text
Old path:
Root → Alive → Combat → Chase

New path:
Root → Dead

Common path:
Root

Exit:
Chase
Combat
Alive

Enter:
Dead
```

## Implementation rule

```text
1. Build the target path from Root to target state.
2. Extend that path by selecting valid child states.
3. Compare old path and new path from the start.
4. Find the first index where they differ.
5. Exit old states from deepest to that index.
6. Enter new states from that index to deepest.
7. Replace _activePath with the new path.
```

If this part is solid, the rest of the StateTree becomes much easier.

---

# 7. Example Conditions and Tasks

## `HasTargetCondition`

```csharp
public class HasTargetCondition : Condition
{
    public override bool Evaluate(EnemyStateTreeContext context)
    {
        return context.Target != null;
    }
}
```

## `TargetInRangeCondition`

```csharp
public class TargetInRangeCondition : Condition
{
    private readonly float _range;

    public TargetInRangeCondition(float range)
    {
        _range = range;
    }

    public override bool Evaluate(EnemyStateTreeContext context)
    {
        if (context.Target == null)
            return false;

        return context.DistanceToTarget <= _range;
    }
}
```

## `TargetOutOfRangeCondition`

```csharp
public class TargetOutOfRangeCondition : Condition
{
    private readonly float _range;

    public TargetOutOfRangeCondition(float range)
    {
        _range = range;
    }

    public override bool Evaluate(EnemyStateTreeContext context)
    {
        if (context.Target == null)
            return true;

        return context.DistanceToTarget > _range;
    }
}
```

## `IsDeadCondition`

```csharp
public class IsDeadCondition : Condition
{
    public override bool Evaluate(EnemyStateTreeContext context)
    {
        return context.Health <= 0f || context.IsDead;
    }
}
```

## `MoveToTargetTask`

```csharp
public class MoveToTargetTask : StateTask
{
    public override void OnEnter(EnemyStateTreeContext context)
    {
        if (context.Agent == null)
            return;

        context.Agent.isStopped = false;
        context.Agent.speed = context.MoveSpeed;
    }

    public override void OnTick(EnemyStateTreeContext context, float deltaTime)
    {
        if (context.Agent == null || context.Target == null)
            return;

        if (!context.Agent.isOnNavMesh)
            return;

        context.Agent.SetDestination(context.Target.position);
    }

    public override void OnExit(EnemyStateTreeContext context)
    {
        if (context.Agent != null && context.Agent.isOnNavMesh)
            context.Agent.ResetPath();
    }
}
```

## `AttackTask`

```csharp
public class AttackTask : StateTask
{
    private readonly string _attackTrigger;

    public AttackTask(string attackTrigger)
    {
        _attackTrigger = attackTrigger;
    }

    public override void OnEnter(EnemyStateTreeContext context)
    {
        if (context.Agent != null && context.Agent.isOnNavMesh)
        {
            context.Agent.ResetPath();
            context.Agent.isStopped = true;
        }

        if (context.Animator != null)
            context.Animator.SetTrigger(_attackTrigger);
    }
}
```

## `StopMovementTask`

```csharp
public class StopMovementTask : StateTask
{
    public override void OnEnter(EnemyStateTreeContext context)
    {
        if (context.Agent == null || !context.Agent.isOnNavMesh)
            return;

        context.Agent.ResetPath();
        context.Agent.isStopped = true;
    }
}
```

## `PlayAnimationTask`

```csharp
public class PlayAnimationTask : StateTask
{
    private readonly string _stateName;

    public PlayAnimationTask(string stateName)
    {
        _stateName = stateName;
    }

    public override void OnEnter(EnemyStateTreeContext context)
    {
        if (context.Animator != null)
            context.Animator.Play(_stateName);
    }
}
```

---

# 8. First Test Tree Builder

```csharp
public static class EnemyStateTreeBuilder
{
    public static StateNode Build()
    {
        var root = new StateNode("Root");

        var dead = new StateNode("Dead");
        dead.EnterConditions.Add(new IsDeadCondition());
        dead.Tasks.Add(new StopMovementTask());
        dead.Tasks.Add(new PlayAnimationTask("Die"));

        var alive = new StateNode("Alive");

        var combat = new StateNode("Combat");
        combat.EnterConditions.Add(new HasTargetCondition());

        var chase = new StateNode("Chase");
        chase.EnterConditions.Add(new HasTargetCondition());
        chase.Tasks.Add(new MoveToTargetTask());

        var attack = new StateNode("Attack");
        attack.EnterConditions.Add(new TargetInRangeCondition(2f));
        attack.Tasks.Add(new AttackTask("Attack"));

        var idle = new StateNode("Idle");
        idle.Tasks.Add(new PlayAnimationTask("Idle"));

        root.Children.Add(dead);
        root.Children.Add(alive);

        alive.Children.Add(combat);
        alive.Children.Add(idle);

        combat.Children.Add(attack);
        combat.Children.Add(chase);

        chase.Transitions.Add(new Transition(attack, TransitionTrigger.OnTick)
        {
            Conditions =
            {
                new TargetInRangeCondition(2f)
            }
        });

        attack.Transitions.Add(new Transition(chase, TransitionTrigger.OnTick)
        {
            Conditions =
            {
                new TargetOutOfRangeCondition(2.5f)
            }
        });

        alive.Transitions.Add(new Transition(dead, TransitionTrigger.OnTick)
        {
            Priority = 1000,
            Conditions =
            {
                new IsDeadCondition()
            }
        });

        return root;
    }
}
```

Expected behavior:

```text
No target:
Root → Alive → Idle

Target exists, outside range:
Root → Alive → Combat → Chase

Target in attack range:
Root → Alive → Combat → Attack

Health <= 0:
Root → Dead
```

---

# 9. Event-Driven Transitions

Add this after tick transitions work.

```csharp
public readonly struct StateTreeEvent
{
    public readonly string Name;
    public readonly object Payload;

    public StateTreeEvent(string name, object payload = null)
    {
        Name = name;
        Payload = payload;
    }
}
```

Runner event queue:

```csharp
private readonly Queue<StateTreeEvent> _eventQueue = new();

public void SendEvent(StateTreeEvent evt)
{
    _eventQueue.Enqueue(evt);
}
```

Animation event example:

```csharp
public void Animation_AttackFinished()
{
    _stateTreeRunner.SendEvent(new StateTreeEvent("AttackFinished"));
}
```

Transition example:

```text
Attack
  OnEvent AttackFinished
  If TargetInRange → Attack
  Else → Chase
```

---

# 10. Sensors / Evaluators

Do not make every condition calculate everything. Update shared context data once through sensors.

```csharp
using UnityEngine;

public class EnemySensorUpdater : MonoBehaviour
{
    private EnemyStateTreeContext _context;

    private void Awake()
    {
        _context = GetComponent<EnemyStateTreeContext>();
    }

    private void Update()
    {
        if (_context.Target == null)
        {
            _context.DistanceToTarget = float.PositiveInfinity;
            _context.HasLineOfSight = false;
            return;
        }

        _context.DistanceToTarget = Vector3.Distance(
            transform.position,
            _context.Target.position
        );

        _context.HasLineOfSight = true; // Replace with raycast later.
    }
}
```

This keeps conditions simple:

```csharp
return context.DistanceToTarget <= range;
```

---

# 11. Add Later: Task Completion and Utility Selection

## Task completion

Add task completion after the basic transition system works.

Examples:

```text
MoveToPatrolPoint succeeds when destination is reached.
WaitTask succeeds after 2 seconds.
AttackTask succeeds when animation event fires.
CastSpellTask succeeds when cast animation finishes.
```

Then transitions can use:

```text
OnStateSucceeded
OnStateFailed
OnStateCompleted
```

## Utility selection

Use utility selection for combat choices that should not be rigid priority lists.

Example:

```text
Combat
Selection: HighestUtilityChild

Children:
- CastFireball
- CastHeal
- MeleeAttack
- Flee
- Chase
```

Base class:

```csharp
public abstract class UtilityConsideration
{
    public abstract float Score(EnemyStateTreeContext context);
}
```

Example:

```csharp
public class LowHealthUtility : UtilityConsideration
{
    public override float Score(EnemyStateTreeContext context)
    {
        return 1f - context.HealthPercent;
    }
}
```

Use utility where priority logic becomes too rigid, especially for spell selection.

---

# 12. ScriptableObject Authoring Rules

Add ScriptableObject authoring only when the runtime is stable.

Bad:

```csharp
public class WaitTaskAsset : ScriptableObject
{
    public float Timer; // Bad: shared runtime state.
}
```

Good:

```csharp
public class WaitTaskAsset : ScriptableObject
{
    public float Duration;

    public StateTask CreateRuntimeTask()
    {
        return new WaitTask(Duration);
    }
}
```

Rule:

```text
Asset = configuration
Runtime instance = actual state for one enemy
```

This prevents multiple enemies from accidentally sharing timers, task status, cooldown data, or temporary references.

---

# 13. Milestone Checklist

| Milestone | Definition of Done |
|---|---|
| M1: Minimal runtime | Enemy starts in Idle and the runner displays `Root / Alive / Idle`. |
| M2: Child selection | Adding a target selects `Combat / Chase` automatically. |
| M3: Tick transitions | `Chase` transitions to `Attack` when `DistanceToTarget <= AttackRange`. |
| M4: Parent transition | Any `Alive` state transitions to `Dead` when health reaches zero. |
| M5: Event transition | `AttackFinished` event moves `Attack` back into correct combat selection. |
| M6: Debug visibility | Inspector/log shows current path, last transition, and failed conditions. |
| M7: Task status | `Wait`, `Move`, and `Attack` tasks can report `Running`, `Succeeded`, or `Failed`. |
| M8: ScriptableObject conversion | Tree can be authored as assets without sharing runtime state. |

---

# 14. Common Pitfalls

- **Starting with a graph editor.** This delays the real work. Build the runtime first.
- **Putting all logic in one task.** Tasks become hard to reuse and debug.
- **Using `OnTick` transitions for everything.** Use events for perception, damage, animation, and ability completion.
- **Forgetting that parent states are active.** Parent tasks and transitions can affect child states.
- **Letting helper tasks complete states.** Use `TaskCompletionMode.None` as the default.
- **Storing runtime data in ScriptableObject assets.** This causes enemies to accidentally share state.
- **Not logging failed conditions.** Most StateTree bugs are selection or transition failures.

---

# 15. Final Recommendation

For RPG creature AI, build a **real StateTree runtime** instead of forcing a Behavior Tree to behave like one.

Start with:

```text
Root
├── Dead
└── Alive
    ├── Stunned
    ├── OutOfCombat
    │   ├── Idle
    │   └── Patrol
    └── Combat
        ├── CastSpell
        ├── AttackMelee
        ├── Chase
        └── Search
```

Use `FirstValidChild` selection first:

```text
Alive children:
1. Stunned
2. Combat
3. OutOfCombat

Combat children:
1. CastSpell
2. AttackMelee
3. Chase
4. Search
```

Build gameplay first, tools second. A reliable runtime with simple debug output is more valuable than a fancy editor that runs unstable logic.
