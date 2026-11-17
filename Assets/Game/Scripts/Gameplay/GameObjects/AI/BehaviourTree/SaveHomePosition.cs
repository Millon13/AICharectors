using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public class SaveHomePosition : MonoBehaviour
    {
        [SerializeField] private Blackboard _blackboard;

        private void Start()
        {
            _blackboard.SetPrimitiveValue(BlackboardAPI.HomePosition, transform.position);
        }
    }
}