using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class HoldPositionNode: BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private CommandQueueComponent _queue;

        [Header("Attack chain")]
        [SerializeField] private KeepTargetDistanceNode _keepDistance;
        [SerializeField] private LookAtTargetNode _lookAt;
        [SerializeField] private AttackTargetNode _attack;

        [Header("Return")]
        [SerializeField] private MoveToTargetNode _returnToPoint;
        
        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.HoldPosition))
                return BehaviourResult.Success;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;
            
            bool hasEnemy = TryGetEnemyInRadius(out _);

            if (hasEnemy)
            {
                BehaviourResult r = _keepDistance.Run(deltaTime);
                if (r != BehaviourResult.Success) return r;

                r = _lookAt.Run(deltaTime);
                if (r != BehaviourResult.Success) return r;

                _attack.Run(deltaTime);
                return BehaviourResult.Running;
            }
            
            if (!_blackboard.TryGetValue(BlackboardAPI.HoldPositionPoint, out Vector3 point))
                return BehaviourResult.Failure;

            _blackboard.SetPrimitiveValue(BlackboardAPI.TargetPosition, point);
            BehaviourResult move = _returnToPoint.Run(deltaTime);
            
            return move == BehaviourResult.Success
                ? BehaviourResult.Running
                : move;
        }

        private bool TryGetEnemyInRadius(out GameObject enemy)
        {
            enemy = null;
            if (!_blackboard.TryGetValue(BlackboardAPI.Enemy, out GameObject candidate) || !candidate)
                return false;

            if (!_blackboard.TryGetValue(BlackboardAPI.HoldPositionPoint, out Vector3 point) ||
                !_blackboard.TryGetValue(BlackboardAPI.HoldAttackRadius, out float radius))
                return false;

            Vector3 delta = candidate.transform.position - point;
            delta.y = 0;
            if (delta.sqrMagnitude > radius * radius)
                return false;

            enemy = candidate;
            return true;
        }

        protected override void OnStop(BehaviourResult result)
        {
            if (_keepDistance.IsRunning) _keepDistance.Abort();
            if (_lookAt.IsRunning)       _lookAt.Abort();
            if (_attack.IsRunning)       _attack.Abort();
            if (_returnToPoint.IsRunning) _returnToPoint.Abort();
        }
    }
    
}