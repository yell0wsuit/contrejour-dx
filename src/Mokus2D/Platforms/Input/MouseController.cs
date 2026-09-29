using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.Platforms.Input
{
    public static class MouseController
    {
        private static bool _leftPressed;

        private static bool _rightPressed;

        private static bool _middlePressed;

        private static int _scrollWheelValue;

        private static bool _scrollBaselineTaken;

        private static readonly Dictionary<IMouseOverNode, LinkedListNode<IMouseOverNode>> MouseEventNodes;

        private static readonly LinkedList<IMouseOverNode> MouseOverNodes;

        private static readonly LinkedList<IMouseOverNode> MouseOutNodes;

        private static readonly Action<IMouseOverNode> MouseOverAction;

        private static readonly Action<IMouseOverNode> MouseOutAction;

        public static Vector2 CursorPosition { get; private set; }

        public static event Action LeftButtonPress;

        public static event Action LeftButtonRelease;

        public static event Action MiddleButtonPress;

        public static event Action MiddleButtonRelease;

        public static event Action RightButtonPress;

        public static event Action RightButtonRelease;

        public static event Action<int> ScrollEvent;

        static MouseController()
        {
            MouseEventNodes = [];
            MouseOverNodes = new LinkedList<IMouseOverNode>();
            MouseOutNodes = new LinkedList<IMouseOverNode>();
            MouseOverAction = delegate (IMouseOverNode n)
            {
                n.MouseOver();
            };
            MouseOutAction = delegate (IMouseOverNode n)
            {
                n.MouseOut();
            };
        }

        public static void AddMouseOverNode(IMouseOverNode node)
        {
            LinkedListNode<IMouseOverNode> value = MouseOverNodes.AddLast(node);
            MouseEventNodes.Add(node, value);
        }

        public static void RemoveMouseOverNode(IMouseOverNode node)
        {
            LinkedListNode<IMouseOverNode> linkedListNode = MouseEventNodes[node];
            _ = MouseEventNodes.Remove(node);
            linkedListNode.List.Remove(linkedListNode);
        }

        public static void Update()
        {
            if (!_scrollBaselineTaken)
            {
                // Taken on the first frame rather than in the static constructor, which can run
                // before the host's input source is available.
                _scrollWheelValue = Mokus2DGame.Input.GetMouse().ScrollWheelValue;
                _scrollBaselineTaken = true;
            }
            if (Mokus2DGame.Instance.AcceptsInput)
            {
                MouseSnapshot mouse = Mokus2DGame.Input.GetMouse();
                DispatchScroll(mouse);
                CursorPosition = XnaMath.Transform(mouse.Position, Mokus2DGame.Instance.TouchController.TransformMatrix);
                ProcessButton(mouse.Left, ref _leftPressed, LeftButtonPress, LeftButtonRelease);
                ProcessButton(mouse.Right, ref _rightPressed, RightButtonPress, RightButtonRelease);
                ProcessButton(mouse.Middle, ref _middlePressed, MiddleButtonPress, MiddleButtonRelease);
                ProcessMouseOver();
            }
        }

        private static void ProcessMouseOver()
        {
            ProcessList(MouseOverNodes, MouseOutNodes, shouldContainMouse: true, MouseOverAction);
            ProcessList(MouseOutNodes, MouseOverNodes, shouldContainMouse: false, MouseOutAction);
        }

        private static void ProcessList(LinkedList<IMouseOverNode> sourceList, LinkedList<IMouseOverNode> targetList, bool shouldContainMouse, Action<IMouseOverNode> action)
        {
            LinkedListNode<IMouseOverNode> linkedListNode = sourceList.First;
            while (linkedListNode != null)
            {
                LinkedListNode<IMouseOverNode> next = linkedListNode.Next;
                Node node = (Node)linkedListNode.Value;
                if (node.RootInteractionsEnabled && node.RootVisible && linkedListNode.Value.ContainsGlobalPosition(CursorPosition) == shouldContainMouse)
                {
                    sourceList.Remove(linkedListNode);
                    targetList.AddLast(linkedListNode);
                    action(linkedListNode.Value);
                }
                linkedListNode = next;
            }
        }

        private static void DispatchScroll(MouseSnapshot mouse)
        {
            if (mouse.ScrollWheelValue != _scrollWheelValue)
            {
                ScrollEvent.Dispatch(mouse.ScrollWheelValue - _scrollWheelValue);
                _scrollWheelValue = mouse.ScrollWheelValue;
            }
        }

        private static void ProcessButton(bool isDown, ref bool pressedValue, Action pressEvent, Action releaseEvent)
        {
            if (isDown && !pressedValue)
            {
                pressedValue = true;
                pressEvent.Dispatch();
            }
            else if (!isDown && pressedValue)
            {
                pressedValue = false;
                releaseEvent.Dispatch();
            }
        }
    }
}
