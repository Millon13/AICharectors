using SampleGame.AI.BehaviourTree;
using UnityEngine;

namespace SampleGame
{
    public sealed class PatrolInputHandler : InputHandler
    {
        [SerializeField]
        private KeyCode _keyCode = KeyCode.P;
        
        [SerializeField]
        private GameObject _character;
        [SerializeField] private CommandQueueComponent _commandQueue;
        [SerializeField]
        private InputHandler _next;

        public override void Handle(ref InputContext context)
        {
            if (Input.GetKey(_keyCode) && context.leftClick)
            {
                bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (context.point != null)
                {
                    var cmd = QueuedCommand.Patrol(context.point.Value, isShift);
                    if (isShift) _commandQueue.Enqueue(cmd);
                    else _commandQueue.ForceCommand(cmd);
                    context.enqueueCommand = true;
                }
                else if (context.target != null && context.target != _character)
                {
                    var cmd = QueuedCommand.Patrol(context.target, isShift);
                    if (isShift) _commandQueue.Enqueue(cmd);
                    else _commandQueue.ForceCommand(cmd);
                    context.enqueueCommand = true;
                }

                if (context.enqueueCommand)
                {
                   //Очередь команд создать
                }
                else
                {
                    // TODO: Switch to patrol
                }
            }
            else if (_next)
                _next.Handle(ref context);
        }
    }
}