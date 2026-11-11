using System.Collections.Generic;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public enum CommandType { HoldPosition, MoveTo }

    public readonly struct QueuedCommand
    {
        public readonly CommandType Type;
        public readonly float Radius;
        public readonly Vector3 Position;      
        public readonly GameObject Target; 

        public QueuedCommand(CommandType type,float radius = 0f)
        {
            Type = type;
            Radius = radius;
            Position = default;
            Target = null;
            
        }
        private QueuedCommand(CommandType type, Vector3 position, GameObject target)
        {
            Type = type;
            Radius = 0f;
            Position = position;
            Target = target;
        }
        public static QueuedCommand MoveTo(Vector3 position) 
            => new QueuedCommand(CommandType.MoveTo, position, null);
    
        public static QueuedCommand MoveTo(GameObject target) 
            => new QueuedCommand(CommandType.MoveTo, default, target);
    }
    public class CommandQueueComponent: MonoBehaviour
    {
         [SerializeField] private Blackboard _blackboard;
        [SerializeField] private float _defaultHoldRadius = 6f;

        private readonly Queue<QueuedCommand> _queue = new();
        private QueuedCommand? _current;

        public bool HasCurrent => _current.HasValue;
        public int QueueCount => _queue.Count;
        
        public void Enqueue(QueuedCommand command)
        {
            if (_current.HasValue)
            {
                _queue.Enqueue(command);
                return;
            }
            StartCommand(command);
        }
        public void ForceCommand(QueuedCommand command)
        {
            StopAll();         
            Enqueue(command);   
        }
        public void StopAll()
        {
            _queue.Clear();
            if (_current.HasValue)
                FinishCurrent();
            _current = null;
        }
        

        private void StartCommand(QueuedCommand command)
        {
            _current = command;

            switch (command.Type)
            {
                case CommandType.HoldPosition:
                    _blackboard.SetPrimitiveValue(
                        BlackboardAPI.HoldPositionPoint,
                        transform.position);
                    _blackboard.SetPrimitiveValue(
                        BlackboardAPI.HoldAttackRadius,
                        command.Radius > 0 ? command.Radius : _defaultHoldRadius);
                    _blackboard.AddTag(BlackboardAPI.HoldPosition);
                    break;
                case CommandType.MoveTo:
                    // Если кликнули по сущности (Target != null)
                    if (command.Target != null)
                    {
                        _blackboard.SetReferenceValue(BlackboardAPI.MoveToTarget, command.Target);
                        _blackboard.DelValue(BlackboardAPI.MoveToPosition); // Очищаем точку
                    }
                    // Если кликнули по земле
                    else
                    {
                        _blackboard.SetPrimitiveValue(BlackboardAPI.MoveToPosition, command.Position);
                        _blackboard.DelValue(BlackboardAPI.MoveToTarget); // Очищаем сущность
                    }
                    _blackboard.AddTag(BlackboardAPI.MoveTo);
                    break;
            }

            SyncQueueTag();
        }
        
        public void NotifyCommandComplete()
        {
            if (!_current.HasValue) return;

            FinishCurrent();
            _current = null;

            if (_queue.Count > 0)
                StartCommand(_queue.Dequeue());
            else
                SyncQueueTag();
        }

        private void FinishCurrent()
        {
            switch (_current.Value.Type)
            {
                case CommandType.HoldPosition:
                    _blackboard.DelTag(BlackboardAPI.HoldPosition);
                    break;
                case CommandType.MoveTo:
                    _blackboard.DelTag(BlackboardAPI.MoveTo);
                    _blackboard.DelValue(BlackboardAPI.MoveToTarget);
                    break;
            }
        }

        private void SyncQueueTag()
        {
            if (_queue.Count > 0)
                _blackboard.AddTag(BlackboardAPI.HasQueuedCommand);
            else
                _blackboard.DelTag(BlackboardAPI.HasQueuedCommand);
        }
    }
    
}