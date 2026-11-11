using System;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.Conditions
{
    [Serializable]

    public class HasTargetCondition : ICondition
    {

        [SerializeField] private Blackboard _blackboard;

        [SerializeField] [BlackboardValueKey(typeof(GameObject))]
        private string _targetKey;

        public bool Invoke()
        {
            return _blackboard.TryGetValue(_targetKey, out GameObject target) && target && target.activeInHierarchy;
        }
    }

}