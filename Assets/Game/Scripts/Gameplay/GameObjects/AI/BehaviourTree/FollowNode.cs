using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class FollowNode : BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;

        [SerializeField] private CommandQueueComponent _queue;

        [SerializeField] private float _stoppingDistance = 2f;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.Follow))
                return BehaviourResult.Success;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;

            if (!_blackboard.TryGetValue(BlackboardAPI.FollowTarget, out GameObject target) || target == null)
            {
                _queue.NotifyCommandComplete();
                return BehaviourResult.Failure;
            }

            if (target.TryGetComponent(out HealthComponent health) && health.IsDead)
            {
                _queue.NotifyCommandComplete();
                return BehaviourResult.Failure;
            }

            Vector3 delta = target.transform.position - character.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude <= _stoppingDistance * _stoppingDistance)
            {
                return BehaviourResult.Running;
            }

            character.GetComponent<MoveComponent>().MoveStep(delta.normalized, deltaTime);
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