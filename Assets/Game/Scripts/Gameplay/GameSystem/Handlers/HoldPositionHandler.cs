using SampleGame.AI.BehaviourTree;
using UnityEngine;

namespace SampleGame
{
    public sealed class HoldPositionHandler : InputHandler
    {
        [SerializeField]
        private KeyCode _keyCode = KeyCode.H;

        [SerializeField]
        private GameObject _character;
        [SerializeField] private CommandQueueComponent _commandQueue;
        [SerializeField]
        private InputHandler _next;

        public override void Handle(ref InputContext context)
        {
            if (Input.GetKeyDown(_keyCode))
            {
                bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var cmd = new QueuedCommand(CommandType.HoldPosition);

                if (isShift) 
                    _commandQueue.Enqueue(cmd);     
                else 
                    _commandQueue.ForceCommand(cmd); 

                context.enqueueCommand = true;
            }
            
            else if (_next) 
                _next.Handle(ref context);
        }
    }
}