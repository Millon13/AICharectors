using SampleGame.AI.BehaviourTree;
using UnityEngine;

namespace SampleGame
{
    public sealed class AttackInputHandler : InputHandler
    {
        [SerializeField]
        private KeyCode _keyCode = KeyCode.A;

        [SerializeField]
        private GameObject _character;

        [SerializeField]
        private InputHandler _next;
        [SerializeField]
        private CommandQueueComponent _commandQueue;

        public override void Handle(ref InputContext context)
        {
            if (Input.GetKey(_keyCode) && context.leftClick)
            {
                bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (context.point != null)
                {
                    var cmd = QueuedCommand.Attack(context.point.Value);
                    if (isShift) _commandQueue.Enqueue(cmd);
                    else _commandQueue.ForceCommand(cmd);
                    
                    context.enqueueCommand = true;
                }
                else if (context.target != null && context.target != _character)
                {
                    if (_character.TryGetComponent(out TeamComponent myTeam) && myTeam.IsEnemy(context.target))
                    {
                        var cmd = QueuedCommand.Attack(context.target);
                        if (isShift) _commandQueue.Enqueue(cmd);
                        else _commandQueue.ForceCommand(cmd);
                        
                        context.enqueueCommand = true;
                    }
                    else
                    {
                        Debug.Log("Цель не является врагом!");
                    }
                    return;
                }
            }
            else if (_next)
                _next.Handle(ref context);
        }
    }
}