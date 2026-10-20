using UnityEngine;
using Modules.AI;
[BlackboardAPI]
public static class BlackboardAPI
{
    public static BlackboardValueKey<GameObject> Character = new(nameof(Character));

    
    //Stop
    //Hold Position
    //Moving
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
    
    
    
}
