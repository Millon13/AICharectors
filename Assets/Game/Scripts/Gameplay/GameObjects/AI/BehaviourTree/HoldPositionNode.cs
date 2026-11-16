using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class HoldPositionNode: BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private CommandQueueComponent _queue;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.HoldPosition))
                return BehaviourResult.Success;

            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;
            
            if (_blackboard.TryGetValue(BlackboardAPI.Enemy, out GameObject enemy) && enemy != null)
            {
              
                _blackboard.SetReferenceValue(BlackboardAPI.AttackTarget, enemy);
                Debug.Log($"[Hold] Враг найден: {enemy.name}. Передаю в AttackTarget.");
            }
            else
            {
                _blackboard.DelValue(BlackboardAPI.AttackTarget);
                Debug.Log($"[Hold] Враг НЕ найден в ключе Enemy!");
            }
            return BehaviourResult.Running;
        }
    }
    
}