using System.Collections.Generic;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class PatrolNode : BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.Patrol))
                return BehaviourResult.Failure;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character) ||
                !_blackboard.TryGetValue(BlackboardAPI.Waypoints, out Transform[] waypoints) ||
                !_blackboard.TryGetValue(BlackboardAPI.WaypointIndex, out int index))
                return BehaviourResult.Failure;

            if (waypoints == null || waypoints.Length == 0)
            {
                _blackboard.DelTag(BlackboardAPI.Patrol);
                return BehaviourResult.Failure;
            }

            List<Transform> validWaypoints = new List<Transform>(waypoints);
            for (int i = validWaypoints.Count - 1; i >= 0; i--)
            {
                var wp = validWaypoints[i];
                if (wp == null)
                {
                    validWaypoints.RemoveAt(i);
                    continue;
                }

                if (wp.TryGetComponent(out HealthComponent health) && health.IsDead)
                {
                    validWaypoints.RemoveAt(i);
                    if (index >= validWaypoints.Count) index = 0;
                    
                }
            }

            if (validWaypoints.Count == 0)
            {
                _blackboard.DelTag(BlackboardAPI.Patrol);
                return BehaviourResult.Failure;
            }

            if (validWaypoints.Count != waypoints.Length)
            {
                _blackboard.SetReferenceValue(BlackboardAPI.Waypoints, validWaypoints.ToArray());
                _blackboard.SetPrimitiveValue(BlackboardAPI.WaypointIndex, index);
                waypoints = validWaypoints.ToArray();
            }

            if (_blackboard.TryGetValue(BlackboardAPI.AttackTarget, out GameObject enemy) && enemy != null)
            {
                character.GetComponent<MoveComponent>().MoveStep(Vector3.zero, 0f); // Стоим
                return BehaviourResult.Running;
            }

            Vector3 targetPos = waypoints[index].position;
            Vector3 delta = targetPos - character.transform.position;
            delta.y = 0;

            _blackboard.TryGetValue(BlackboardAPI.StoppingDistance, out float stoppingDist);

            if (delta.sqrMagnitude <= stoppingDist * stoppingDist)
            {
                index = (index + 1) % waypoints.Length;
                _blackboard.SetPrimitiveValue(BlackboardAPI.WaypointIndex, index);
                character.GetComponent<MoveComponent>().MoveStep(Vector3.zero, 0f);
                return BehaviourResult.Running;
            }

            character.GetComponent<MoveComponent>().MoveStep(delta.normalized, deltaTime);
            return BehaviourResult.Running;
        }
    }
}