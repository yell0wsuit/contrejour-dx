using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mokus2D.Util;
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
            _scrollWheelValue = Mouse.GetState().ScrollWheelValue;
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
            if (Mokus2DGame.Instance.AcceptsInput)
            {
                MouseState state = Mouse.GetState();
                DispatchScroll(state);
                CursorPosition = Vector2.Transform(new Vector2(state.X, state.Y), Mokus2DGame.Instance.TouchController.TransformMatrix);
                ProcessButton(state.LeftButton, ref _leftPressed, LeftButtonPress, LeftButtonRelease);
                ProcessButton(state.RightButton, ref _rightPressed, RightButtonPress, RightButtonRelease);
                ProcessButton(state.MiddleButton, ref _middlePressed, MiddleButtonPress, MiddleButtonRelease);
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

        private static void DispatchScroll(MouseState state)
        {
            if (state.ScrollWheelValue != _scrollWheelValue)
            {
                ScrollEvent.Dispatch(state.ScrollWheelValue - _scrollWheelValue);
                _scrollWheelValue = state.ScrollWheelValue;
            }
        }

        private static void ProcessButton(ButtonState buttonState, ref bool pressedValue, Action pressEvent, Action releaseEvent)
        {
            if (buttonState == ButtonState.Pressed && !pressedValue)
            {
                pressedValue = true;
                pressEvent.Dispatch();
            }
            else if (buttonState == ButtonState.Released && pressedValue)
            {
                pressedValue = false;
                releaseEvent.Dispatch();
            }
        }
    }
}
