using System;
using System.Collections.Generic;

using ContreJour.Content;
using ContreJour.Debug;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class LevelBuilderBase : Updatable, IDisposable
{
    public const int FOREGROUND = 10;

    public const float MAX_UPDATE_TIME = 1f / 30f;

    public const int BACKGROUND = -10;

    public string NamespacePrefix;

    protected Dictionary<string, BodyClip> clips;

    protected Hashtable createdBodies;

    protected Dictionary<string, object> createdObjects;

    protected Vector2 currentOffset;

    protected int defaultZ;

    protected Box2DConfig engineConfig;

    protected GameBase game;

    protected Body groundBody;

    protected Hashtable lastItem;

    private Vector2 levelSize;

    protected float maxWorldUpdateTime;

    private Vector2 physicsLevelSize;

    protected float physicsSpeed;

    protected List<object> processedBodies;

    protected List<object> processors;

    protected PhysicsUpdater updater;

    protected World world;

    public Box2DConfig EngineConfig
    {
        get
        {
            return engineConfig;
        }
        set
        {
            engineConfig = value;
        }
    }

    public World World => world;

    public Body GroundBody => groundBody;

    public GameBase Game => game;

    public Dictionary<string, object> CreatedObjects => createdObjects;

    public float PhysicsSpeed
    {
        get
        {
            return physicsSpeed;
        }
        set
        {
            physicsSpeed = value;
        }
    }

    public int DefaultZ
    {
        get
        {
            return defaultZ;
        }
        set
        {
            defaultZ = value;
        }
    }

    public float SizeMult => engineConfig.SizeMultiplier;

    public Vector2 LevelSize
    {
        get
        {
            return levelSize;
        }
        set
        {
            levelSize = value;
            physicsLevelSize = levelSize * SizeMult;
        }
    }

    public Vector2 PhysicsLevelSize => physicsLevelSize;

    public Node GameRoot => game.GameRoot;

    public LevelBuilderBase(GameBase _game)
    {
        //IL_0055: Unknown result type (might be due to invalid IL or missing references)
        //IL_005f: Expected O, but got Unknown
        createdObjects = new Dictionary<string, object>();
        defaultZ = 0;
        engineConfig = Box2DConfig.DefaultConfig;
        Settings.PositionIterations = engineConfig.PositionIterations;
        Settings.VelocityIterations = engineConfig.VelocityIterations;
        Settings.ContinuousPhysics = false;
        world = new World(engineConfig.Gravity);
        groundBody = BodyFactory.CreateBody(world, new Vector2(0f, 0f), 0f, (object)null);
        maxWorldUpdateTime = 1f / 30f;
        physicsSpeed = 1f;
        game = _game;
        processors = new List<object>();
        updater = new PhysicsUpdater(world);
        clips = new Dictionary<string, BodyClip>();
        AddProcessors();
    }

    public void Dispose()
    {
    }

    public Body CreateCircleRadiusPositionRotationDynamic(float radius, Vector2 position, float rotation, bool dynamic)
    {
        return world.CreateCircle(radius, position, rotation, engineConfig.Density, dynamic);
    }

    public virtual void AddProcessors()
    {
        processors.Add(new CircleProcessor(this));
        processors.Add(new PolygonProcessor(this));
        processors.Add(new BodyProcessor(this));
        processors.Add(new EditorRevoluteJointProcessor(this));
    }

    public void RegisterObject(BodyClip bodyClip, string key)
    {
        createdObjects[key] = bodyClip;
    }

    public object GetObject(string key)
    {
        if (!createdObjects.ContainsKey(key))
        {
            return null;
        }
        return createdObjects[key];
    }

    public void AddForeground(Node child)
    {
        GameRoot.AddChild(child, 10);
    }

    public void AddBackground(Node child)
    {
        GameRoot.AddChild(child, -10);
    }

    public void AddChildBefore(Node child, Node before)
    {
        GameRoot.AddChild(child, before.Layer - 1);
    }

    public void AddChildAfter(Node child, Node after)
    {
        GameRoot.AddChild(child, after.Layer);
    }

    public Vector2 ToRootChild(Vector2 source, Node child)
    {
        Vector2 source2 = child.LocalToGlobal(source);
        return GameRoot.GlobalToLocal(source2);
    }

    public void ReorderChildBefore(Node child, Node node)
    {
        GameRoot.RemoveChild(child);
        AddChildBefore(child, node);
    }

    public void ChangeChildLayer(Node child, int z)
    {
        GameRoot.ChangeChildLayer(child, z);
    }

    public void RemoveChild(Node child, bool cleanup)
    {
        GameRoot.RemoveChild(child);
    }

    public void RemoveChild(Node child)
    {
        GameRoot.RemoveChild(child);
    }

    public void Add(Node child, int z)
    {
        GameRoot.AddChild(child, z);
    }

    public Node ReplaceClipWithNode(Node clip, Node newClip)
    {
        newClip.ScaleX = clip.ScaleX;
        newClip.ScaleY = clip.ScaleY;
        newClip.Position = clip.Position;
        newClip.RotationRadians = clip.RotationRadians;
        ReplaceChildWith(clip, newClip);
        return newClip;
    }

    public Node ReplaceClipWith(Node clip, string clipName)
    {
        return ReplaceClipWithNode(clip, ClipTypesCache.CreateNewNode(clipName));
    }

    public void ReplaceChildWith(Node source, Node with)
    {
        source.Parent.AddChild(with, source.Layer);
        source.RemoveFromParent();
    }

    public Node AddChild(Node child)
    {
        return AddChild(child, 0);
    }

    public Node AddChild(Node child, int layer)
    {
        GameRoot.AddChild(child, layer);
        return child;
    }

    public BodyClip GetClip(string name)
    {
        return clips[name];
    }

    public Vector2 ToIPhoneVec(Vector2 point)
    {
        return ToVec(point);
    }

    public Vector2 ToVec(Vector2 point)
    {
        return engineConfig.ToVec(point);
    }

    public Vector2 TouchRootVec(Touch touch)
    {
        return ToVec(TouchRootPoint(touch));
    }

    public Vector2 TouchRootPoint(Touch touch)
    {
        return GameRoot.GlobalToLocal(touch.Position);
    }

    public Vector2 ToIPhonePoint(Vector2 vec)
    {
        return ToPoint(vec);
    }

    public Vector2 ToIPadPoint(Vector2 vec)
    {
        return ToPoint(vec);
    }

    public void ToPointsPoints(List<Vector2> vecs, List<Vector2> points)
    {
        for (int i = 0; i < vecs.Count; i++)
        {
            points.Add(ToPoint(vecs[i]));
        }
    }

    public Vector2 ToPoint(Vector2 vec)
    {
        return engineConfig.ToPoint(vec);
    }

    public void DestroyFixturesData(Body body)
    {
        foreach (Fixture fixture in body.FixtureList)
        {
            fixture.UserData = null;
        }
    }

    public void RemoveBody(Body body)
    {
        DestroyFixturesData(body);
        world.RemoveBody(body);
    }

    public void ProcessLevel(Level level)
    {
        Hashtable levelProperties = level.LevelProperties;
        LevelSize = new Vector2(levelProperties.GetFloat("Width"), levelProperties.GetFloat("Height"));
        foreach (Hashtable item in level.Items)
        {
            foreach (Hashtable item2 in (List<object>)item["children"])
            {
                if (!ProcessItem(item2))
                {
                    DebugUtil.Trace("Item Not Processed `" + item2.GetString("config/type") + "` ");
                }
            }
        }
    }

    public bool ProcessItem(Hashtable item)
    {
        bool result = false;
        foreach (TypeProcessorBase processor in processors)
        {
            if (processor.Match(item))
            {
                result = true;
                object obj = processor.ProcessItem(item);
                if (obj != null)
                {
                    ProcessCreatedPhysicsItem(obj, item);
                }
            }
        }
        return result;
    }

    public virtual string GetViewType(Hashtable config)
    {
        if (config.Exists("iPhoneViewType"))
        {
            return config.GetString("iPhoneViewType");
        }
        if (config.Exists("viewType"))
        {
            return config.GetString("viewType");
        }
        return null;
    }

    private string GetClipType(Hashtable config)
    {
        if (config.Exists("iPhoneClipType"))
        {
            return config.GetString("iPhoneClipType");
        }
        if (config.Exists("clipType"))
        {
            return config.GetString("clipType");
        }
        return null;
    }

    public virtual LevelBuilderBase GetBuilder()
    {
        return this;
    }

    public BodyClip CreateItem(object physics, Hashtable item)
    {
        Hashtable hashtable = item.GetHashtable("config");
        string viewType = GetViewType(hashtable);
        if (viewType != null || hashtable.Exists("createClip") || hashtable.Exists("clipType"))
        {
            Node node = null;
            if (viewType != null && !viewType.Equals("null"))
            {
                node = ClipTypesCache.CreateNewNode(viewType);
                Vector2 vector = hashtable.GetVector("scale");
                node.ScaleX = vector.X;
                node.ScaleY = vector.Y;
                if (hashtable.GetBool("flipX"))
                {
                    node.ScaleX *= -1f;
                }
                if (hashtable.GetBool("flipY"))
                {
                    node.ScaleY *= -1f;
                }
                Add(node, hashtable.Exists("z") ? hashtable.GetInt("z") : DefaultZ);
                node.Position = ToIPadPoint(item.GetVector("position"));
                node.RotationRadians = 0f - MathHelper.ToRadians(hashtable.GetFloat("rotation", 0f));
            }
            if (!hashtable.Exists("skipClip"))
            {
                string clipType = GetClipType(hashtable);
                Type type = ((clipType != null) ? Type.GetType(NamespacePrefix + clipType) : typeof(BodyClip));
                if ((object)type == null)
                {
                    DebugUtil.Trace("type not found {0}", null, clipType);
                    return null;
                }
                return (BodyClip)ReflectUtil.CreateInstance(type, this, physics, node, hashtable);
            }
        }
        return null;
    }

    public void ProcessCreatedPhysicsItem(object physics, Hashtable item)
    {
        BodyClip bodyClip = CreateItem(physics, item);
        if (bodyClip != null && item.Exists("config/id"))
        {
            string key = item.GetString("config/id");
            clips[key] = bodyClip;
        }
    }

    public float ToRotationVec(Vector2 vec)
    {
        return ToRotation(VectorUtil.Atan2(vec));
    }

    public float ToRotation(float angle)
    {
        return MathHelper.ToDegrees(0f - angle);
    }

    public override void Update(float time)
    {
        float num = Math.Min(time, maxWorldUpdateTime);
        world.Step(num * physicsSpeed);
        updater.Update(time);
    }
}
