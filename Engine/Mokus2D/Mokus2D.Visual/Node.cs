using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Mokus2D.Visual;

public class Node : DisposableBase, IUpdatable, IConfig
{
	private const int DefaultListCapacity = 64;

	public readonly Tweener Tweener;

	private readonly ConcurrentQueue<NodeAndLayer> _addLater = new ConcurrentQueue<NodeAndLayer>();

	private readonly List<Node> _cachedChildrenCopy = new List<Node>(64);

	private readonly Stack<Node> _cachedVisualStack = new Stack<Node>(64);

	private readonly ConcurrentQueue<Action<Node>> _callLater = new ConcurrentQueue<Action<Node>>();

	private readonly NodeChildren _children = new NodeChildren();

	private readonly ConcurrentQueue<Node> _removeLater = new ConcurrentQueue<Node>();

	public float ColorRatio;

	public INodeController Controller;

	public bool DrawSelf = true;

	protected IDrawer Drawer;

	public bool IgnoreParentColor;

	public bool IgnoreParentOpacity;

	public bool IgnoreParentTransformations;

	public IgnoredAnimationProperties IgnoredAnimations = IgnoredAnimationProperties.None;

	public bool InteractionsEnabled = true;

	public bool IsAnimationDiscrete;

	public string Name;

	public int OnScreenCount = 1;

	public bool ResetDefaultEffect;

	public bool Test;

	public bool UpdateChildren = true;

	public bool UpdateChildrenTransformations = true;

	public bool UpdateEnabled = true;

	public bool UpdateSelf = true;

	private IDictionary<string, string> _config;

	private GarbageTracer _drawTracer;

	private bool _firstUpdate = true;

	private int _layer;

	private bool _matrixDirty;

	private Matrix _nodeMatrix = Matrix.Identity;

	private float _opacity = 1f;

	private Node _parent;

	private Vector2 _position;

	private RootNode _root;

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
		get
		{
			return Position.X;
		}
		set
		{
			Position = new Vector2(value, Position.Y);
		}
	}

	public float Y
	{
		get
		{
			return Position.Y;
		}
		set
		{
			Position = new Vector2(Position.X, value);
		}
	}

	public float RotationDegrees
	{
		get
		{
			return MathHelper.ToDegrees(RotationRadians);
		}
		set
		{
			RotationRadians = MathHelper.ToRadians(value);
		}
	}

	public bool VisibleAndUpdating
	{
		set
		{
			Visible = (UpdateChildren = (UpdateEnabled = value));
		}
	}

	public virtual Color Color { get; set; }

	public int OpacityByte
	{
		get
		{
			return (int)(OpacityFloat * 255f);
		}
		set
		{
			OpacityFloat = (float)value / 255f;
		}
	}

	public virtual float OpacityFloat
	{
		get
		{
			return _opacity;
		}
		set
		{
			_opacity = value.Clamp(0f, 1f);
		}
	}

	public float Scale
	{
		get
		{
			if (ScaleVec.X == ScaleVec.Y)
			{
				return ScaleVec.X;
			}
			throw new InvalidOperationException("ScaleVec.X differs from ScaleVec.Y");
		}
		set
		{
			ScaleVec = new Vector2(value, value);
		}
	}

	public float ScaleX
	{
		get
		{
			return ScaleVec.X;
		}
		set
		{
			ScaleVec = new Vector2(value, ScaleVec.Y);
		}
	}

	public float ScaleY
	{
		get
		{
			return ScaleVec.Y;
		}
		set
		{
			ScaleVec = new Vector2(ScaleVec.X, value);
		}
	}

	public virtual bool Visible
	{
		get
		{
			return _visible;
		}
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
		set
		{
			Position = new Vector2(value);
		}
	}

	public virtual Vector2 Position
	{
		get
		{
			return _position;
		}
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
		get
		{
			return _scaleVec;
		}
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
		get
		{
			return _rotationRadians;
		}
		set
		{
			if (_rotationRadians != value)
			{
				_rotationRadians = value;
				_matrixDirty = true;
			}
		}
	}

	public NodeChildren Children => _children;

	public int Layer => _layer;

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

	public bool IsVisibleAndOnScreen
	{
		get
		{
			if (Visible)
			{
				return OnScreenCount > 0;
			}
			return false;
		}
	}

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
		get
		{
			return _parent;
		}
		protected set
		{
			_parent = value;
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
		get
		{
			return _root;
		}
		internal set
		{
			if (_root == value)
			{
				return;
			}
			bool flag = _root == null && value != null;
			bool flag2 = _root != null && value == null;
			_root = value;
			if (flag)
			{
				OnAddedToStage();
				if (Controller != null)
				{
					Controller.OnAddedToStage();
				}
				this.AddedToStageEvent.Dispatch();
			}
			if (flag2)
			{
				OnRemovedFromStage();
				if (Controller != null)
				{
					Controller.OnRemovedFromStage();
				}
			}
		}
	}

	public virtual bool IsRoot => Root == this;

	public IDictionary<string, string> Config => _config;

	public event Action TransformationsRefreshedEvent;

	public event Action AddedToStageEvent;

	[Conditional("DEBUG")]
	private void ThrowIfNotInMainThread()
	{
	}

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
			if (Controller != null)
			{
				Controller.FirstUpdate();
			}
			_firstUpdate = false;
		}
	}

	public void CreateConfig()
	{
		if (_config == null)
		{
			_config = new Dictionary<string, string>();
		}
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
			if (_config == null)
			{
				_config = new DoubleSourceDictionary<string, string>(new Dictionary<string, string>());
			}
			((DoubleSourceDictionary<string, string>)_config).SetSecondSource(config);
		}
		if (Config != null && Config.GetBool("test"))
		{
			Test = true;
		}
	}

	protected void SetMainConfig(IDictionary<string, string> config)
	{
		if (_config == null)
		{
			if (config != null)
			{
				_config = new DoubleSourceDictionary<string, string>(config);
			}
			return;
		}
		if (_config is DoubleSourceDictionary<string, string>)
		{
			DoubleSourceDictionary<string, string> doubleSourceDictionary = (DoubleSourceDictionary<string, string>)_config;
			if (config == null)
			{
				_config = doubleSourceDictionary.SecondSource;
			}
			else
			{
				doubleSourceDictionary.ResetMainSource(config);
			}
			return;
		}
		throw new Exception("Config reset not allowed");
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
		return _children.IndexOf(child);
	}

	public virtual void ChangeChildLayer(Node node, int nodeLayer)
	{
		if (nodeLayer != node._layer)
		{
			node._layer = nodeLayer;
			RemoveFromChildren(node);
			_children.Add(node);
		}
	}

	public void AddChild(Node node)
	{
		AddChild(node, 0);
	}

	public virtual void AddChild(Node node, int nodeLayer)
	{
		CheckIfCanAdd(node);
		node._layer = nodeLayer;
		SetThisAsParentTo(node);
		_children.Add(node);
	}

	private void CheckIfCanAdd(Node node)
	{
		if (node == this)
		{
			throw new InvalidOperationException("Cannot add node to itself");
		}
		if (node.Parent != null)
		{
			throw new InvalidOperationException(string.Format("node already added to display list node:{0} current parent:{1} new parent: {2}", new object[3] { node, node.Parent, this }));
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
		node.SetRootAndDrawer(_root, Drawer);
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
			throw new Exception("node already added to another parent");
		}
		if (!_children.Empty())
		{
			if (index == 0)
			{
				node._layer = Math.Min(_children[0].Layer, node._layer);
			}
			else if (index == _children.Count)
			{
				node._layer = Math.Max(_children.Last().Layer, node.Layer);
			}
			else
			{
				node._layer = node._layer.Clamp(_children[index - 1]._layer, _children[index]._layer);
			}
		}
		SetThisAsParentTo(node);
		_children.Insert(index, node);
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
		if (Parent != null)
		{
			Parent.RemoveChild(this);
		}
	}

	public virtual void RemoveFromParentLater()
	{
		if (Parent != null)
		{
			Parent.RemoveChildLater(this);
		}
	}

	public virtual void RemoveAllChildren()
	{
		while (!Children.Empty())
		{
			RemoveChild(Children.Last());
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
		if (!_children.Remove(node))
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
				if (Controller != null)
				{
					Controller.Update(time);
				}
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
		_cachedChildrenCopy.AddItemsNoGarbage(Children);
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
		Node result;
		while (_removeLater.TryDequeue(out result))
		{
			RemoveChild(result);
		}
		NodeAndLayer result2;
		while (_addLater.TryDequeue(out result2))
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
		if (_callLater.TryDequeue(out var result))
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
		DrawChildrenPart(index, positiveLayers: true);
	}

	protected int DrawChildrenPart(int index, bool positiveLayers)
	{
		bool flag;
		do
		{
			flag = index < _children.Count;
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
			if (CompositeState == null)
			{
				CompositeState = new VisualState(parentState);
			}
			CompositeState.Refresh(parentState, ref _nodeMatrix, OpacityFloat, Color, ColorRatio, IgnoreParentOpacity, IgnoreParentColor, IgnoreParentTransformations);
			_transformationDirty = false;
			this.TransformationsRefreshedEvent.Dispatch();
		}
		else
		{
			CompositeState.RefreshValues(parentState, OpacityFloat, Color, ColorRatio, IgnoreParentOpacity, IgnoreParentColor);
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (Tweener != null)
		{
			Tweener.Dispose();
		}
		if (Children == null)
		{
			return;
		}
		foreach (Node child in Children)
		{
			child.Dispose();
		}
	}

	[Conditional("DEBUG")]
	private void CreateGarbageTracer()
	{
		_drawTracer = new GarbageTracer(string.Concat(GetType(), ".Draw"));
	}

	public override string ToString()
	{
		return string.Format("Node type:{0} name:{1}", new object[2]
		{
			GetType(),
			Name
		});
	}
}
