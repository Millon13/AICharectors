namespace SampleGame.AI.BehaviourTree
{
    using Modules.AI;
    using UnityEngine;

    public class MoveToTargetNode : BehaviourNode
    {

        /*  [SerializeField] private Blackboard _blackboard;

          [SerializeField] [BlackboardValueKey(typeof(GameObject))]
          private string _targetKey;

          [SerializeField] [BlackboardValueKey(typeof(float))]
          private string _stoppingDistance;

          protected override BehaviourResult OnUpdate(float deltaTime)
          {
              if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character) ||
                  !_blackboard.TryGetValue(_targetKey, out GameObject resource) || !resource ||
                  !_blackboard.TryGetValue(_stoppingDistance, out float stoppingDistance))
                  return BehaviourResult.Failure;

              Vector3 delta = resource.transform.position - character.transform.position;
              delta.y = 0;

              if (delta.sqrMagnitude <= stoppingDistance * stoppingDistance)
                  return BehaviourResult.Success;

              character.GetComponent<MoveComponent>().MoveStep(delta.normalized, deltaTime);
              return BehaviourResult.Running;
          }

      }*/
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private CommandQueueComponent _queue;

        [SerializeField] private float _stoppingDistance = 0.5f;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.MoveTo))
                return BehaviourResult.Success;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;

            // 1. Если цель — сущность, проверяем её валидность и обновляем позицию
            if (_blackboard.TryGetValue(BlackboardAPI.MoveToTarget, out GameObject target) && target != null)
            {
                // Команда прерывается, если цель исчезла / умерла / деактивировалась
                if (!target || !target.activeInHierarchy)
                {
                    _queue.NotifyCommandComplete(); // снимает тег, стартует следующую в очереди
                    return BehaviourResult.Failure;
                }

                if (target.TryGetComponent(out HealthComponent health) && health.IsDead)
                {
                    _queue.NotifyCommandComplete();
                    return BehaviourResult.Failure;
                }

                // цель жива — обновляем позицию погони (бежим за ней)
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
                // дошли — команда завершена, очередь запускает следующую
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
        }
    }
}