using System;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.Conditions
{
    
    [Serializable]
    public class HasTagCondition : ICondition
    {
        [SerializeField] private Blackboard _blackboard;
        [SerializeField, BlackboardTagKey] private string _tag = "HoldPosition";

        public bool Invoke() => _blackboard.HasTag(_tag);
    }
}