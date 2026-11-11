using UnityEngine;
using Modules.AI;
[BlackboardAPI]
public static class BlackboardAPI
{
    public static BlackboardValueKey<GameObject> Character = new(nameof(Character));

    
    //Stop
    //Hold Position
    //Moving
    // MoveTo command
    public static readonly BlackboardTagKey MoveTo = new(nameof(MoveTo));
    public static readonly BlackboardValueKey<Vector3> MoveToPosition = new(nameof(MoveToPosition));
    public static readonly BlackboardValueKey<GameObject> MoveToTarget = new(nameof(MoveToTarget));
    //Patrol
    public static readonly BlackboardValueKey<Transform[]> Waypoints = new(nameof(Waypoints)); 
    public static readonly BlackboardValueKey<int> WaypointIndex = new(nameof(WaypointIndex));
    public static readonly BlackboardValueKey<Vector3> TargetPosition = new(nameof(TargetPosition));
    public static readonly BlackboardValueKey<float> StoppingDistance = new(nameof(StoppingDistance));
    public static readonly BlackboardValueKey<float> StoppingAngle = new(nameof(StoppingAngle));
    //Attack 
    public static BlackboardValueKey<GameObject> Enemy = new(nameof(Enemy));
    public static BlackboardValueKey<float> MeleeDistance = new(nameof(MeleeDistance));
    public static BlackboardValueKey<float> MinRangeDistance = new(nameof(MinRangeDistance));
    public static BlackboardValueKey<float> MaxRangeDistance = new(nameof(MaxRangeDistance));
    //Follow
    
    // Command Queue
    public static readonly BlackboardTagKey HasQueuedCommand = new(nameof(HasQueuedCommand));
    public static readonly BlackboardTagKey HoldPosition = new(nameof(HoldPosition));
    public static readonly BlackboardValueKey<Vector3> HoldPositionPoint = new(nameof(HoldPositionPoint));
    public static readonly BlackboardValueKey<float> HoldAttackRadius = new(nameof(HoldAttackRadius));
    
    
}
