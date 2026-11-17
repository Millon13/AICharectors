using System.Collections.Generic;
using Modules.AI;
using UnityEngine;

namespace SampleGame.AI.BehaviourTree
{
    public enum CommandType
    {
        HoldPosition,
        MoveTo,
        Follow,
        Attack,
        Patrol,
        Stop
    }

    public readonly struct QueuedCommand
    {
        public readonly CommandType Type;
        public readonly float Radius;
        public readonly Vector3 Position;
        public readonly GameObject Target;
        public readonly bool AddToPatrolList;

        [System.Serializable]
        public struct WayPoint
        {
            public Vector3 Position;
            public GameObject Entity;

            public WayPoint(Vector3 position, GameObject entity)
            {
                Position = position;
                Entity = entity;
            }
        }

        public QueuedCommand(CommandType type, float radius = 0f, bool addToPatrol = false)
        {
            Type = type;
            Radius = radius;
            Position = default;
            Target = null;
            AddToPatrolList = false;
        }

        private QueuedCommand(CommandType type, Vector3 position, GameObject target, bool addToPatrol = false)
        {
            Type = type;
            Radius = 0f;
            Position = position;
            Target = target;
            AddToPatrolList = addToPatrol;
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

        public static QueuedCommand Patrol(Vector3 position, bool addToList = false)
            => new QueuedCommand(CommandType.Patrol, position, null, addToList);

        public static QueuedCommand Patrol(GameObject target, bool addToPatrol = false)
            => new QueuedCommand(CommandType.Patrol, default, target, addToPatrol);
    }

    public class CommandQueueComponent : MonoBehaviour
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


            SyncQueueTag();
        }


        private void StartCommand(QueuedCommand command)
        {
            _current = command;

            switch (command.Type)
            {
                case CommandType.Patrol:
                    GameObject marker = new GameObject("PatrolPoint_Marker");

                    if (command.Target != null)
                    {
                        marker.transform.position = command.Target.transform.position;
                        marker.name = $"PatrolPoint_{command.Target.name}";
                    }
                    else
                    {
                        marker.transform.position = command.Position;
                    }

                    if (command.AddToPatrolList &&
                        _blackboard.TryGetValue(BlackboardAPI.Waypoints, out Transform[] existing))
                    {
                        var newArr = new Transform[existing.Length + 1];
                        existing.CopyTo(newArr, 0);
                        newArr[existing.Length] = marker.transform;
                        _blackboard.SetReferenceValue(BlackboardAPI.Waypoints, newArr);
                    }
                    else
                    {
                        GameObject baseMarker = new GameObject("PatrolPoint_Base");
                        baseMarker.transform.position = transform.position;

                        var newArr = new Transform[] { baseMarker.transform, marker.transform };
                        _blackboard.SetReferenceValue(BlackboardAPI.Waypoints, newArr);
                        _blackboard.SetPrimitiveValue(BlackboardAPI.WaypointIndex, 0);
                        _blackboard.AddTag(BlackboardAPI.Patrol);
                    }

                    break;
                case CommandType.HoldPosition:
                    _blackboard.AddTag(BlackboardAPI.HoldPosition);
                    _blackboard.SetPrimitiveValue(BlackboardAPI.HoldPositionPoint, transform.position);
                    _blackboard.SetPrimitiveValue(BlackboardAPI.HoldAttackRadius,
                        command.Radius > 0 ? command.Radius : _defaultHoldRadius);
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
                case CommandType.Patrol:
                    _blackboard.DelTag(BlackboardAPI.Patrol);
                    if (_blackboard.TryGetValue(BlackboardAPI.Waypoints, out Transform[] wps) && wps != null)
                    {
                        foreach (var wp in wps)
                        {
                            if (wp != null && wp.name.Contains("PatrolPoint"))
                                Destroy(wp.gameObject);
                        }
                    }

                    _blackboard.DelValue(BlackboardAPI.Waypoints);
                    break;
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