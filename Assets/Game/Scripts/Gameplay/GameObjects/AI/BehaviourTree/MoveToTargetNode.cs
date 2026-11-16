namespace SampleGame.AI.BehaviourTree
{
    using Modules.AI;
    using UnityEngine;

    public class MoveToTargetNode : BehaviourNode
    {

        
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private CommandQueueComponent _queue;

        [SerializeField] [BlackboardValueKey(typeof(GameObject))]
        private string _targetKey;
        [SerializeField] private float _stoppingDistance = 0.5f;
        [SerializeField] [BlackboardValueKey(typeof(Vector3))]
        private string _positionKey;
        protected override BehaviourResult OnUpdate(float deltaTime)
        {
           /* if (!_blackboard.HasTag(BlackboardAPI.MoveTo))
                return BehaviourResult.Success;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;

           
            if (_blackboard.TryGetValue(BlackboardAPI.MoveToTarget, out GameObject target) && target != null)
            {
                
                if (!target || !target.activeInHierarchy)
                {
                    _queue.NotifyCommandComplete(); 
                    return BehaviourResult.Failure;
                }

                if (target.TryGetComponent(out HealthComponent health) && health.IsDead)
                {
                    _queue.NotifyCommandComplete();
                    return BehaviourResult.Failure;
                }
                _blackboard.SetPrimitiveValue(
                    BlackboardAPI.MoveToPosition,
                    target.transform.position);
            }

            if (!_blackboard.TryGetValue(BlackboardAPI.MoveToPosition, out Vector3 point))
                return BehaviourResult.Failure;

            Vector3 delta = point - character.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude <= _stoppingDistance * _stoppingDistance)
            {
                _queue.NotifyCommandComplete();
                return BehaviourResult.Success;
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
        }*/
           if (!_blackboard.HasTag(BlackboardAPI.MoveTo) && !_blackboard.HasTag(BlackboardAPI.AttackCommand))
               return BehaviourResult.Success;

           if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
               return BehaviourResult.Failure;

           // 2. Пытаемся получить цель (по ключу из инспектора)
           if (!string.IsNullOrEmpty(_targetKey) && 
               _blackboard.TryGetValue(_targetKey, out GameObject target) && target != null)
           {
               if (!target.activeInHierarchy || (target.TryGetComponent(out HealthComponent h) && h.IsDead))
               {
                   _queue.NotifyCommandComplete();
                   return BehaviourResult.Failure;
               }
               _blackboard.SetPrimitiveValue(_positionKey, target.transform.position);
           }
           
           if (!_blackboard.TryGetValue(_positionKey, out Vector3 point))
               return BehaviourResult.Failure;

           Vector3 delta = point - character.transform.position;
           delta.y = 0;

           if (delta.sqrMagnitude <= _stoppingDistance * _stoppingDistance)
           {
               return BehaviourResult.Success;
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