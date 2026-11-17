using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class FindEnemyNode : BehaviourNode
    {
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private float _searchRadius = 10f;

        protected override BehaviourResult OnUpdate(float deltaTime)
        {
            if (!_blackboard.TryGetValue(BlackboardAPI.Character, out GameObject character))
                return BehaviourResult.Failure;

            Collider[] hits = Physics.OverlapSphere(character.transform.position, _searchRadius);
            GameObject closestEnemy = null;
            float minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.gameObject == character) continue;

                if (hit.TryGetComponent(out TeamComponent foundTeam) && foundTeam.IsEnemy(character))
                {
                    float dist = (hit.transform.position - character.transform.position).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestEnemy = hit.gameObject;
                    }
                }
            }

            if (closestEnemy != null)
            {
                _blackboard.SetReferenceValue(BlackboardAPI.AttackTarget, closestEnemy);
                _blackboard.SetReferenceValue(BlackboardAPI.FollowTarget, closestEnemy);
            }
            else
            {
                _blackboard.DelValue(BlackboardAPI.AttackTarget);
                _blackboard.DelValue(BlackboardAPI.FollowTarget);
            }

            return BehaviourResult.Running;
        }
    }
}