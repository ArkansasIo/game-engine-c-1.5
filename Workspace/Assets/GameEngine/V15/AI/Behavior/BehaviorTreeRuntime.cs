using System;
using System.Collections.Generic;

namespace GameEngine.V15.AI.Behavior
{
    public enum NodeStatus { Success, Failure, Running }

    public abstract class BehaviorNode
    {
        public abstract NodeStatus Tick(float deltaSeconds);
    }

    public sealed class ActionNode : BehaviorNode
    {
        private readonly Func<float, NodeStatus> action;
        public ActionNode(Func<float, NodeStatus> action) => this.action = action;
        public override NodeStatus Tick(float deltaSeconds) => action(deltaSeconds);
    }

    public sealed class SequenceNode : BehaviorNode
    {
        private readonly IReadOnlyList<BehaviorNode> children;
        public SequenceNode(params BehaviorNode[] children) => this.children = children;
        public override NodeStatus Tick(float deltaSeconds)
        {
            foreach (var child in children)
            {
                var status = child.Tick(deltaSeconds);
                if (status != NodeStatus.Success) return status;
            }
            return NodeStatus.Success;
        }
    }

    public sealed class SelectorNode : BehaviorNode
    {
        private readonly IReadOnlyList<BehaviorNode> children;
        public SelectorNode(params BehaviorNode[] children) => this.children = children;
        public override NodeStatus Tick(float deltaSeconds)
        {
            foreach (var child in children)
            {
                var status = child.Tick(deltaSeconds);
                if (status != NodeStatus.Failure) return status;
            }
            return NodeStatus.Failure;
        }
    }
}
