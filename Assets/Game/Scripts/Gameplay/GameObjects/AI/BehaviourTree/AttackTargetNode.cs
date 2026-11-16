namespace SampleGame.AI.BehaviourTree
{
    using Modules.AI;
    using UnityEngine;

    public class AttackTargetNode:BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;

        [SerializeField] [BlackboardValueKey(typeof(float))]
        private string _distanceKey;

        [SerializeField] private CommandQueueComponent _queue;
        [SerializeField] [BlackboardValueKey(typeof(GameObject))]
        private string _targetKey;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;
            if (!_blackboard.TryGetValue(_targetKey, out GameObject target) || target == null)
                return BehaviourResult.Running;
            if (!_blackboard.TryGetValue(_distanceKey, out float attackDistance))
                return BehaviourResult.Running;
           
            if (!target || !target.activeInHierarchy || !target.TryGetComponent(out HealthComponent health) ||
                health.IsDead)
            {
                _queue.NotifyCommandComplete();
                return BehaviourResult.Success;
            }

            Vector3 delta = target.transform.position - character.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude > attackDistance * attackDistance)
                return BehaviourResult.Running;
                //return BehaviourResult.Failure;
            //AttackComponent component = character.GetComponent<AttackComponent>();
            if (!character.TryGetComponent(out AttackComponent component))
                return BehaviourResult.Failure;

            component.Attack(target);
            return BehaviourResult.Running;

        }
    }
}