using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xna.Framework;

using Mokus2D.Behaviour;
using Mokus2D.Collections;
using Mokus2D.Effects.Tweening;
using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Util.Resources;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Exceptions;
using Mokus2D.Visual.GameDebug;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Threading;

namespace Mokus2D.Visual
{
    public class Node : DisposableBase, IUpdatable, IConfig
    {
        public Tweener Tweener { get; }

        private readonly ConcurrentQueue<NodeAndLayer> _addLater = new();

        private readonly List<Node> _cachedChildrenCopy = new(64);

        private readonly Stack<Node> _cachedVisualStack = new(64);

        private readonly ConcurrentQueue<Action<Node>> _callLater = new();
        private readonly ConcurrentQueue<Node> _removeLater = new();

        public float ColorRatio { get; set; }

        public INodeController Controller { get; set; }

        public bool DrawSelf { get; set; } = true;

        protected IDrawer Drawer { get; set; }

        public bool IgnoreParentColor { get; set; }

        public bool IgnoreParentOpacity { get; set; }

        public bool IgnoreParentTransformations { get; set; }

        private IgnoredAnimationProperties ignoredAnimations = IgnoredAnimationProperties.None;

        public ref IgnoredAnimationProperties IgnoredAnimations => ref ignoredAnimations;

        public bool InteractionsEnabled { get; set; } = true;

        public bool IsAnimationDiscrete { get; set; }

        public string Name { get; set; }

        public int OnScreenCount { get; set; } = 1;

        public bool ResetDefaultEffect { get; set; }

        public bool Test { get; set; }

        public bool UpdateChildren { get; set; } = true;

        public bool UpdateChildrenTransformations { get; set; } = true;

        public bool UpdateEnabled { get; set; } = true;

        private readonly bool UpdateSelf = true;
        private bool _firstUpdate = true;
        private bool _matrixDirty;

        private Matrix _nodeMatrix = Matrix.Identity;

        private float _opacity = 1f;
        private Vector2 _position;
        private float _rotationRadians;

        private Vector2 _scaleVec = Vector2.One;

        private bool _transformationDirty = true;

        private bool _visible = true;

        public VisualState CompositeState { get; protected set; }

        public ISpriteBatchEffect Effect { get; set; }

        public bool RefreshEffect => Effect != null;

        public bool HasConfig => Config != null;

        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        public float RotationDegrees
        {
            get => MathHelper.ToDegrees(RotationRadians);
            set => RotationRadians = MathHelper.ToRadians(value);
        }

        public bool VisibleAndUpdating
        {
            set => Visible = UpdateChildren = UpdateEnabled = value;
        }

        public virtual Color Color { get; set; }

        public int OpacityByte
        {
            get => (int)(OpacityFloat * 255f);
            set => OpacityFloat = value / 255f;
        }

        public virtual float OpacityFloat
        {
            get => _opacity;
            set => _opacity = value.Clamp(0f, 1f);
        }

        public float Scale
        {
            get => ScaleVec.X == ScaleVec.Y ? ScaleVec.X : throw new InvalidOperationException("ScaleVec.X differs from ScaleVec.Y");

            set => ScaleVec = new Vector2(value, value);
        }

        public float ScaleX
        {
            get => ScaleVec.X;
            set => ScaleVec = new Vector2(value, ScaleVec.Y);
        }

        public float ScaleY
        {
            get => ScaleVec.Y;
            set => ScaleVec = new Vector2(ScaleVec.X, value);
        }

        public virtual bool Visible
        {
            get => _visible;
            set
            {
                if (Visible != value)
                {
                    _visible = value;
                    SetTransformationDirty();
                }
            }
        }

        public float XY
        {
            set => Position = new Vector2(value);
        }

        public virtual Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _matrixDirty = true;
                }
            }
        }

        public virtual Vector2 ScaleVec
        {
            get => _scaleVec;
            set
            {
                if (_scaleVec != value)
                {
                    _scaleVec = value;
                    _matrixDirty = true;
                }
            }
        }

        public virtual float RotationRadians
        {
            get => _rotationRadians;
            set
            {
                if (_rotationRadians != value)
                {
                    _rotationRadians = value;
                    _matrixDirty = true;
                }
            }
        }

        public NodeChildren Children { get; } = [];

        public int Layer { get; private set; }

        public bool RootVisible
        {
            get
            {
                if (!OnDisplayList)
                {
                    return false;
                }
                Node node;
                for (node = this; node != Root; node = node.Parent)
                {
                    if (node == null || !node.IsVisibleAndOnScreen)
                    {
                        return false;
                    }
                }
                return node.IsVisibleAndOnScreen;
            }
        }

        public bool IsVisibleAndOnScreen => Visible && OnScreenCount > 0;

        public bool RootInteractionsEnabled
        {
            get
            {
                if (!OnDisplayList)
                {
                    return false;
                }
                for (Node node = this; node != null; node = node.Parent)
                {
                    if (!node.IsVisibleAndOnScreen || !node.InteractionsEnabled)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool OnDisplayList => Root != null;

        public Node Parent
        {
            get; protected set
            {
                field = value;
                SetTransformationDirty();
            }
        }

        public Matrix NodeMatrix
        {
            get
            {
                RefreshMatrix();
                return _nodeMatrix;
            }
        }

        public RootNode Root
        {
            get; internal set
            {
                if (field == value)
                {
                    return;
                }
                bool flag = field == null && value != null;
                bool flag2 = field != null && value == null;
                field = value;
                if (flag)
                {
                    OnAddedToStage();
                    Controller?.OnAddedToStage();
                    AddedToStageEvent.Dispatch();
                }
                if (flag2)
                {
                    OnRemovedFromStage();
                    Controller?.OnRemovedFromStage();
                }
            }
        }

        public virtual bool IsRoot => Root == this;

        public IDictionary<string, string> Config { get; private set; }

        public event Action TransformationsRefreshedEvent;

        public event Action AddedToStageEvent;

        public Node()
        {
            Color = Color.White;
            Tweener = new Tweener(this);
        }

        public void SetColorAndRatio(Color color, float colorRatio = 1f)
        {
            Color = color;
            ColorRatio = colorRatio;
        }

        public virtual void Update(float time)
        {
            if (_firstUpdate)
            {
                FirstUpdate();
                Controller?.FirstUpdate();
                _firstUpdate = false;
            }
        }

        public void CreateConfig()
        {
            Config ??= new Dictionary<string, string>();
        }

        public virtual void RefreshProperties()
        {
            Position = Vector2.Zero;
            RotationRadians = 0f;
            OpacityFloat = 1f;
            Scale = 1f;
            Color = Color.White;
            ColorRatio = 0f;
        }

        public virtual void OnParentInitialized()
        {
        }

        internal virtual void SetInstanceConfig(IDictionary<string, string> config)
        {
            if (config != null)
            {
                Config ??= new DoubleSourceDictionary<string, string>(new Dictionary<string, string>());
                ((DoubleSourceDictionary<string, string>)Config).SetSecondSource(config);
            }
            if (Config != null && Config.GetBool("test"))
            {
                Test = true;
            }
        }

        protected void SetMainConfig(IDictionary<string, string> config)
        {
            if (Config == null)
            {
                if (config != null)
                {
                    Config = new DoubleSourceDictionary<string, string>(config);
                }
                return;
            }
            if (Config is DoubleSourceDictionary<string, string> doubleSourceDictionary)
            {
                if (config == null)
                {
                    Config = doubleSourceDictionary.SecondSource;
                }
                else
                {
                    doubleSourceDictionary.ResetMainSource(config);
                }
                return;
            }
            throw new InvalidOperationException("Config reset not allowed");
        }

        protected virtual void FirstUpdate()
        {
        }

        public virtual void ReplaceChild(Node child, Node replacement)
        {
            if (child.Parent != this)
            {
                throw new NodeException("Child has another parent");
            }
            replacement.ApplyTransformations(child);
            AddChildAfter(replacement, child);
            RemoveChild(child);
        }

        public void ApplyTransformations(Node source)
        {
            ScaleVec = source.ScaleVec;
            RotationDegrees = source.RotationDegrees;
            Position = source.Position;
            OpacityFloat = source.OpacityFloat;
            Color = source.Color;
            ColorRatio = source.ColorRatio;
        }

        protected virtual void OnRemovedFromStage()
        {
        }

        protected virtual void OnAddedToStage()
        {
        }

        public virtual void Draw(VisualState state)
        {
        }

        public virtual int GetChildIndex(Node child)
        {
            return Children.IndexOf(child);
        }

        public virtual void ChangeChildLayer(Node node, int nodeLayer)
        {
            if (nodeLayer != node.Layer)
            {
                node.Layer = nodeLayer;
                RemoveFromChildren(node);
                Children.Add(node);
            }
        }

        public void AddChild(Node node)
        {
            AddChild(node, 0);
        }

        public virtual void AddChild(Node node, int nodeLayer)
        {
            CheckIfCanAdd(node);
            node.Layer = nodeLayer;
            SetThisAsParentTo(node);
            Children.Add(node);
        }

        private void CheckIfCanAdd(Node node)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Cannot add node to itself");
            }
            if (node.Parent != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "node already added to display list node:{0} current parent:{1} new parent: {2}", node, node.Parent, this));
            }
        }

        public void CallLater(Action<Node> action)
        {
            _callLater.Enqueue(action);
        }

        public void AddChildLater(Node node, bool removeFromPreviousParent = false)
        {
            AddChildLater(node, 0, removeFromPreviousParent);
        }

        public void AddChildLater(Node node, int nodeLayer, bool removeFromPreviousParent = false)
        {
            _addLater.Enqueue(new NodeAndLayer(node, nodeLayer, removeFromPreviousParent));
        }

        public void RemoveChildLater(Node node)
        {
            _removeLater.Enqueue(node);
        }

        private void SetThisAsParentTo(Node node)
        {
            node.Parent = this;
            node.SetRootAndDrawer(Root, Drawer);
        }

        public void AddChildBefore(Node node, Node before)
        {
            AddChildAt(node, GetChildIndex(before));
        }

        public void AddChildAfter(Node node, Node after)
        {
            AddChildAt(node, GetChildIndex(after) + 1);
        }

        public virtual void AddChildAt(Node node, int index)
        {
            if (node.Parent != null)
            {
                throw new InvalidOperationException("node already added to another parent");
            }
            if (Children.Count != 0)
            {
                node.Layer = index == 0
                    ? Math.Min(Children[0].Layer, node.Layer)
                    : index == Children.Count
                        ? Math.Max(Children[^1].Layer, node.Layer)
                        : node.Layer.Clamp(Children[index - 1].Layer, Children[index].Layer);
            }
            SetThisAsParentTo(node);
            Children.Insert(index, node);
        }

        public Vector2 ZeroToGlobal(bool refreshTransformations = true)
        {
            return LocalToGlobal(Vector2.Zero, refreshTransformations);
        }

        public Vector2 ZeroToNode(Node node, bool refreshTransformations = true)
        {
            return LocalToNode(Vector2.Zero, node, refreshTransformations);
        }

        public Vector2 LocalToNode(Vector2 source, Node node, bool refreshTransformations = true)
        {
            Vector2 source2 = LocalToGlobal(source, refreshTransformations);
            return node.GlobalToLocal(source2, refreshTransformations);
        }

        public virtual Vector2 GlobalToLocal(Vector2 source, bool refreshTransformations = true)
        {
            if (!OnDisplayList)
            {
                throw new InvalidOperationException("Node is not on display list");
            }
            if (refreshTransformations)
            {
                RefreshParentTransformations();
            }
            Matrix matrix = Matrix.Invert(CompositeState.Matrix);
            return source.Transform(ref matrix);
        }

        public Vector2 LocalToGlobal(Vector2 source, bool refreshTransformations = true)
        {
            if (refreshTransformations || CompositeState == null)
            {
                RefreshParentTransformations();
            }
            return Vector2.Transform(source, CompositeState.Matrix);
        }

        private void RefreshParentTransformations()
        {
            _cachedVisualStack.Clear();
            Node node = this;
            while (!node.IsRoot)
            {
                _cachedVisualStack.Push(node);
                node = node.Parent;
            }
            node.RefreshTransformations(((RootNode)node).RootState);
            VisualState compositeState = node.CompositeState;
            while (_cachedVisualStack.Count != 0)
            {
                node = _cachedVisualStack.Pop();
                node.RefreshTransformations(compositeState);
                compositeState = node.CompositeState;
            }
        }

        internal virtual void SetRootAndDrawer(RootNode root, IDrawer drawer)
        {
            Drawer = drawer;
            Root = root;
            foreach (Node child in Children)
            {
                child.SetRootAndDrawer(root, drawer);
            }
        }

        public virtual void RemoveFromParent()
        {
            Parent?.RemoveChild(this);
        }

        public virtual void RemoveFromParentLater()
        {
            Parent?.RemoveChildLater(this);
        }

        public virtual void RemoveAllChildren()
        {
            while (Children.Count != 0)
            {
                RemoveChild(Children[^1]);
            }
        }

        public virtual void RemoveChild(Node node)
        {
            node.Parent = null;
            node.SetRootAndDrawer(null, null);
            RemoveFromChildren(node);
        }

        private void RemoveFromChildren(Node node)
        {
            if (!Children.Remove(node))
            {
                throw new InvalidOperationException("There is no such child in collection");
            }
        }

        internal void TryUpdateNode(float time)
        {
            if (UpdateEnabled && OnScreenCount > 0)
            {
                UpdateNode(time);
            }
        }

        public virtual void UpdateNode(float time)
        {
            Mokus2DGame.Instance.PerformanceCounter.IncreaseUpdates();
            Tweener.Update(time);
            ExecuteCallLaters();
            if (UpdateSelf)
            {
                using (new GarbageTracer(GetType().Name))
                {
                    Update(time);
                    Controller?.Update(time);
                }
            }
            if (UpdateChildren)
            {
                DoUpdateChildren(time);
            }
        }

        protected virtual void DoUpdateChildren(float time)
        {
            _cachedChildrenCopy.Clear();
            _cachedChildrenCopy.AddRange(Children);
            foreach (Node item in _cachedChildrenCopy)
            {
                item.TryUpdateNode(time);
            }
            _cachedChildrenCopy.Clear();
        }

        private void RefreshMatrix()
        {
            if (_matrixDirty)
            {
                _nodeMatrix = MatrixUtil.CalculateTransformMatrix(Position, RotationRadians, ScaleVec);
                _matrixDirty = false;
                SetTransformationDirty();
            }
        }

        protected void SetTransformationDirty()
        {
            _transformationDirty = true;
        }

        internal void DrawNode()
        {
            if (RefreshEffect)
            {
                Drawer.StartEffect(Effect);
            }
            DrawWithChildren();
            if (ResetDefaultEffect)
            {
                Drawer.StartEffect(null);
            }
            CompositeState.TransformationDirty = false;
        }

        private void RefreshChildren()
        {
            while (_removeLater.TryDequeue(out Node result))
            {
                RemoveChild(result);
            }
            while (_addLater.TryDequeue(out NodeAndLayer result2))
            {
                if (result2.RemoveFromPreviousParent)
                {
                    result2.Node.RemoveFromParent();
                }
                AddChild(result2.Node, result2.Layer);
            }
        }

        private void ExecuteCallLaters()
        {
            if (_callLater.TryDequeue(out Action<Node> result))
            {
                result(this);
            }
        }

        protected virtual void DrawWithChildren()
        {
            int index = DrawChildrenPart(0, positiveLayers: false);
            if (DrawSelf)
            {
                Draw(CompositeState);
                Drawer.IncreaseNodesDrawnCount();
            }
            _ = DrawChildrenPart(index, positiveLayers: true);
        }

        protected int DrawChildrenPart(int index, bool positiveLayers)
        {
            bool flag;
            do
            {
                flag = index < Children.Count;
                if (!flag)
                {
                    continue;
                }
                Node node = Children[index];
                flag = positiveLayers || node.Layer < 0;
                if (flag)
                {
                    if (node.Visible && node.OnScreenCount > 0 && node.OpacityFloat > 0f && node.CompositeState != null)
                    {
                        node.DrawNode();
                    }
                    index++;
                }
            }
            while (flag);
            return index;
        }

        internal void RefreshVisualState(VisualState parentState)
        {
            RefreshChildren();
            RefreshTransformations(parentState);
        }

        protected virtual void RefreshTransformations(VisualState parentState)
        {
            RefreshMatrix();
            if (parentState.TransformationDirty || _transformationDirty)
            {
                CompositeState ??= new VisualState(parentState);
                CompositeState.Refresh(parentState, ref _nodeMatrix, OpacityFloat, Color, ColorRatio, IgnoreParentOpacity, IgnoreParentColor, IgnoreParentTransformations);
                _transformationDirty = false;
                TransformationsRefreshedEvent.Dispatch();
            }
            else
            {
                CompositeState.RefreshValues(parentState, OpacityFloat, Color, ColorRatio, IgnoreParentOpacity, IgnoreParentColor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Tweener?.Dispose();
            if (Children == null)
            {
                return;
            }
            foreach (Node child in Children)
            {
                child.Dispose();
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Node type:{0} name:{1}", GetType(), Name);
        }
    }
}
