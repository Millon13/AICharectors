using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class HoldPositionNode : BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private CommandQueueComponent _queue;

        private TeamComponent _enemyTeam;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.HasTag(BlackboardAPI.HoldPosition))
                return BehaviourResult.Failure;


            return BehaviourResult.Running;
        }
    }
}