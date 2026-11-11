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
        private CommandQueueComponent _commandQueue;

        public override void Handle(ref InputContext context)
        {
            if (context.leftClick)
            {
                bool isShiftHeld = Input.GetKey(KeyCode.M); // Из ТЗ: ЛКМ + M
                bool hasEntity = context.target != null && context.target != _character;
                bool hasPoint = context.point.HasValue;
                
                if (hasEntity)
                {
                    var command = QueuedCommand.MoveTo(context.target);
                    
                    if (isShiftHeld)
                        _commandQueue.Enqueue(command);      // Отложенная
                    else
                        _commandQueue.ForceCommand(command); // Прямая (сброс текущей)

                    context.enqueueCommand = true;
                }
                else if (hasPoint)
                {
                    var command = QueuedCommand.MoveTo(context.point.Value);
                    
                    if (isShiftHeld)
                        _commandQueue.Enqueue(command);      // Отложенная
                    else
                        _commandQueue.ForceCommand(command); // Прямая (сброс текущей)

                    context.enqueueCommand = true;
                }
            }
            
            if (_next) 
                _next.Handle(ref context);
        }
        
    }
}