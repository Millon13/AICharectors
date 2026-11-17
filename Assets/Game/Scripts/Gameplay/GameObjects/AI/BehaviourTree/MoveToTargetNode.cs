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
            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;

            Vector3 targetPoint = Vector3.zero;
            bool hasTarget = false;
            
            if (!string.IsNullOrEmpty(_targetKey) &&
                _blackboard.TryGetValue(_targetKey, out GameObject target) && target != null)
            {
                if (target.activeInHierarchy && !(target.TryGetComponent(out HealthComponent h) && h.IsDead))
                {
                    targetPoint = target.transform.position;
                    hasTarget = true;
                }
            }
            
            if (!hasTarget && !string.IsNullOrEmpty(_positionKey))
            {
                if (_blackboard.TryGetValue(_positionKey, out Vector3 pos))
                {
                    targetPoint = pos;
                    hasTarget = true;
                }
            }
            if (!hasTarget)
            {
                Debug.LogWarning($"[MoveToTarget] Не найдена ни цель, ни позиция для возврата!");
                return BehaviourResult.Failure;
            }

            Vector3 delta = targetPoint - character.transform.position;
            delta.y = 0;

            if (delta.sqrMagnitude <= _stoppingDistance * _stoppingDistance)
            {
                if (_blackboard.HasTag(BlackboardAPI.MoveTo))
                {
                    _queue.NotifyCommandComplete();
                }
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