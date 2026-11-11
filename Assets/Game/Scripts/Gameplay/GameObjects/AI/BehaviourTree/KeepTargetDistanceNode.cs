using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class KeepTargetDistanceNode: BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;

        [SerializeField]
        [BlackboardValueKey(typeof(float))]
        private string _minDistanceKey;

        [SerializeField]
        [BlackboardValueKey(typeof(float))]
        private string _maxDistanceKey;

        [SerializeField]
        [BlackboardValueKey(typeof(GameObject))]
        private string _targetKey;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character) ||
                !_blackboard.TryGetValue(_targetKey, out GameObject target) || !target ||
                !_blackboard.TryGetValue(_minDistanceKey, out float minDistance) ||
                !_blackboard.TryGetValue(_maxDistanceKey, out float maxDistance))
                return BehaviourResult.Failure;

            Vector3 delta = target.transform.position - character.transform.position;
            delta.y = 0;

            float sqrDistance = delta.sqrMagnitude;

            if (sqrDistance < minDistance * minDistance)
            {
                character.GetComponent<MoveComponent>().MoveStep(-delta.normalized, deltaTime);
                return BehaviourResult.Running;
            }

            if (sqrDistance > maxDistance * maxDistance)
            {
                character.GetComponent<MoveComponent>().MoveStep(delta.normalized, deltaTime);
                return BehaviourResult.Running;
            }
            
            return BehaviourResult.Running;
        }

        protected override void OnStop(BehaviourResult result)
        {
            if (_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character) && character)
            {
                if (character.TryGetComponent(out MoveComponent move))
                    move.MoveStep(Vector3.zero, 0f);
            }
        }
    
    }
}