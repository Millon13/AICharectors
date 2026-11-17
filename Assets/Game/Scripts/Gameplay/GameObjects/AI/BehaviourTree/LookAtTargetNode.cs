using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class LookAtTargetNode : BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;

        [SerializeField] [BlackboardValueKey(typeof(GameObject))]
        private string _targetKey;

        [SerializeField] private float _minAngle = 5f;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character) ||
                !_blackboard.TryGetValue(_targetKey, out GameObject target) || !target)
                return BehaviourResult.Failure;

            Vector3 delta = target.transform.position - character.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude < 0.0001f)
                return BehaviourResult.Success;

            Vector3 direction = delta.normalized;
            float angle = Vector3.Angle(character.transform.forward, direction);

            if (angle <= _minAngle)
                return BehaviourResult.Success;

            RotateTransformComponent rotateComponent =
                character.GetComponent<RotateTransformComponent>();
            rotateComponent.RotateTowards(direction, deltaTime);

            return BehaviourResult.Running;
        }
    }
}