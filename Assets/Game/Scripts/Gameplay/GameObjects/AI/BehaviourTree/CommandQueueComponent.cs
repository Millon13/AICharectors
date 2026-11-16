using System.Collections.Generic;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public enum CommandType { HoldPosition, MoveTo,Follow,Attack }

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
        public static QueuedCommand Follow(GameObject target)
            => new QueuedCommand(CommandType.Follow, default, target);
        public static QueuedCommand Attack(Vector3 position) 
            => new QueuedCommand(CommandType.Attack, position, null);

        public static QueuedCommand Attack(GameObject target) 
            => new QueuedCommand(CommandType.Attack, default, target);
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
                    _blackboard.AddTag(BlackboardAPI.HoldPosition);
                    _blackboard.SetPrimitiveValue(BlackboardAPI.HoldPositionPoint, transform.position);
                    _blackboard.SetPrimitiveValue(BlackboardAPI.HoldAttackRadius, command.Radius > 0 ? command.Radius : _defaultHoldRadius);
                    _blackboard.AddTag(BlackboardAPI.HoldPosition);
                    break;
                case CommandType.MoveTo:
                    if (command.Target != null)
                    {
                        _blackboard.SetReferenceValue(BlackboardAPI.MoveToTarget, command.Target);
                        _blackboard.DelValue(BlackboardAPI.MoveToPosition); 
                    }
                    else
                    {
                        _blackboard.SetPrimitiveValue(BlackboardAPI.MoveToPosition, command.Position);
                        _blackboard.DelValue(BlackboardAPI.MoveToTarget); 
                    }
                    _blackboard.AddTag(BlackboardAPI.MoveTo);
                    break;
                case CommandType.Follow:
                    _blackboard.AddTag(BlackboardAPI.Follow); 
                    if (command.Target != null)
                        _blackboard.SetReferenceValue(BlackboardAPI.FollowTarget, command.Target);
                    break;
                case CommandType.Attack:
                    _blackboard.AddTag(BlackboardAPI.AttackCommand);
                    if (command.Target != null)
                    {
                        _blackboard.SetReferenceValue(BlackboardAPI.AttackTarget, command.Target);
                        _blackboard.DelValue(BlackboardAPI.AttackMovePosition);
                    }
                    else
                    {
                        _blackboard.SetPrimitiveValue(BlackboardAPI.AttackMovePosition, command.Position);
                        _blackboard.DelValue(BlackboardAPI.AttackTarget);
                    }
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
                case CommandType.Follow:
                    _blackboard.DelTag(BlackboardAPI.Follow);
                    _blackboard.DelValue(BlackboardAPI.FollowTarget);
                    break;
                case CommandType.Attack:
                    _blackboard.DelTag(BlackboardAPI.AttackCommand);
                    _blackboard.DelValue(BlackboardAPI.AttackMovePosition);
                    _blackboard.DelValue(BlackboardAPI.AttackTarget);
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