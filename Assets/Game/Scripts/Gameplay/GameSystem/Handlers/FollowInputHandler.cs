using SampleGame.AI.BehaviourTree;
using UnityEngine;

namespace SampleGame
{
    public sealed class FollowInputHandler : InputHandler
    {
        [SerializeField]
        private KeyCode _keyCode = KeyCode.F;

        [SerializeField]
        private GameObject _character;
        [SerializeField]
        private CommandQueueComponent _commandQueue;
        [SerializeField]
        private InputHandler _next;

        public override void Handle(ref InputContext context)
        {
            if (Input.GetKey(_keyCode) && context.leftClick)
            {
                bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (context.point != null)
                {
                    var cmd = QueuedCommand.MoveTo(context.point.Value);
                    if (isShift) _commandQueue.Enqueue(cmd);
                    else _commandQueue.ForceCommand(cmd);
                    
                    context.enqueueCommand = true;
                }
                else if (context.target != null && context.target != _character)
                {
                    var cmd = QueuedCommand.Follow(context.target);
                    if (isShift) _commandQueue.Enqueue(cmd);
                    else _commandQueue.ForceCommand(cmd);
                    
                    context.enqueueCommand = true;
                }
                return;
            }
            
            else if (_next) 
                _next.Handle(ref context);
        }
    }
}