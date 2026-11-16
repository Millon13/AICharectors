using SampleGame.AI.BehaviourTree;
using UnityEngine;

namespace SampleGame
{
    public sealed class MoveInputHandler : InputHandler
    {
        [SerializeField]
        private GameObject _character;

        [SerializeField]
        private InputHandler _next;
        
        [SerializeField]
        private KeyCode _keyCode = KeyCode.M;
        [SerializeField]
        private CommandQueueComponent _commandQueue;

        public override void Handle(ref InputContext context)
        {
            if (Input.GetKey(_keyCode) && context.leftClick)
            {
                bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                bool hasEntity = context.target != null && context.target != _character;
                bool hasPoint = context.point.HasValue;
                
                if (hasEntity)
                {
                    var command = QueuedCommand.MoveTo(context.target);
                    
                    if (isShiftHeld)
                        _commandQueue.Enqueue(command);     
                    else
                        _commandQueue.ForceCommand(command);

                    context.enqueueCommand = true;
                }
                else if (hasPoint)
                {
                    var command = QueuedCommand.MoveTo(context.point.Value);
                    
                    if (isShiftHeld)
                        _commandQueue.Enqueue(command);      
                    else
                        _commandQueue.ForceCommand(command); 

                    context.enqueueCommand = true;
                }
            }
            
            if (_next) 
                _next.Handle(ref context);
        }
        
    }
}