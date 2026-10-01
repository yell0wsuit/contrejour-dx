using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using ContreJour.Content;
using ContreJour.Debug;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Mokus2D.Input;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class LevelBuilderBase : Updatable, IDisposable
    {
        public const int FOREGROUND = 10;

        public const float MaxUpdateTime = 1f / 30f;

        public const int BACKGROUND = -10;

        private readonly Dictionary<string, BodyClip> clips;
        public GameBase Game { get; }
        private Vector2 levelSize;

        protected float MaxWorldUpdateTime { get; set; }

        private Vector2 physicsLevelSize;

        public float PhysicsSpeed { get; set; }
        protected List<object> Processors { get; set; }

        protected PhysicsUpdater Updater { get; set; }

        public World World { get; }

        public Box2DConfig EngineConfig { get; set; }

        public Body GroundBody { get; }

        public Dictionary<string, object> CreatedObjects { get; }

        public int DefaultZ { get; set; }

        public float SizeMult => EngineConfig.SizeMultiplier;

        public Vector2 LevelSize
        {
            get => levelSize;
            set
            {
                levelSize = value;
                physicsLevelSize = levelSize * SizeMult;
            }
        }

        public Vector2 PhysicsLevelSize => physicsLevelSize;

        public Node GameRoot => Game.GameRoot;

        public LevelBuilderBase(GameBase game)
        {
            //IL_0055: Unknown result type (might be due to invalid IL or missing references)
            //IL_005f: Expected O, but got Unknown
            CreatedObjects = [];
            DefaultZ = 0;
            EngineConfig = Box2DConfig.DefaultConfig;
            Settings.PositionIterations = EngineConfig.PositionIterations;
            Settings.VelocityIterations = EngineConfig.VelocityIterations;
            Settings.ContinuousPhysics = false;
            World = new World(EngineConfig.Gravity);
            GroundBody = BodyFactory.CreateBody(World, new Vector2(0f, 0f), 0f, null);
            MaxWorldUpdateTime = 1f / 30f;
            PhysicsSpeed = 1f;
            Game = game;
            Processors = [];
            Updater = new PhysicsUpdater(World);
            clips = [];
            AddProcessors();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Body CreateCircleRadiusPositionRotationDynamic(float radius, Vector2 position, float rotation, bool dynamic)
        {
            return World.CreateCircle(radius, position, rotation, EngineConfig.Density, dynamic);
        }

        public virtual void AddProcessors()
        {
            Processors.Add(new CircleProcessor(this));
            Processors.Add(new PolygonProcessor(this));
            Processors.Add(new BodyProcessor(this));
            Processors.Add(new EditorRevoluteJointProcessor(this));
        }

        public void RegisterObject(BodyClip bodyClip, string key)
        {
            CreatedObjects[key] = bodyClip;
        }

        public object GetObject(string key)
        {
            return CreatedObjects.TryGetValue(key, out object value) ? value : null;
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

        public void RemoveChild(Node child)
        {
            GameRoot.RemoveChild(child);
        }

        public void Add(Node child, int z)
        {
            GameRoot.AddChild(child, z);
        }

        public static Node ReplaceClipWithNode(Node clip, Node newClip)
        {
            newClip.ScaleX = clip.ScaleX;
            newClip.ScaleY = clip.ScaleY;
            newClip.Position = clip.Position;
            newClip.RotationRadians = clip.RotationRadians;
            ReplaceChildWith(clip, newClip);
            return newClip;
        }

        public static Node ReplaceClipWith(Node clip, string clipName)
        {
            return ReplaceClipWithNode(clip, ClipCatalog.Create(clipName));
        }

        public static void ReplaceChildWith(Node source, Node with)
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
            return EngineConfig.ToVec(point);
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
            return EngineConfig.ToPoint(vec);
        }

        public static void DestroyFixturesData(Body body)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                fixture.UserData = null;
            }
        }

        public void RemoveBody(Body body)
        {
            DestroyFixturesData(body);
            World.RemoveBody(body);
        }

        public void ProcessLevel(Level level)
        {
            Hashtable levelProperties = level.LevelProperties;
            LevelSize = new Vector2(levelProperties.GetFloat("Width"), levelProperties.GetFloat("Height"));
            foreach (Hashtable item in level.Items.Cast<Hashtable>())
            {
                foreach (Hashtable item2 in ((List<object>)item["children"]).Cast<Hashtable>())
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
            foreach (TypeProcessorBase processor in Processors.Cast<TypeProcessorBase>())
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
            return config.Exists("iPhoneViewType")
                ? config.GetString("iPhoneViewType")
                : config.Exists("viewType") ? config.GetString("viewType") : null;
        }

        private static string GetClipType(Hashtable config)
        {
            return config.Exists("iPhoneClipType")
                ? config.GetString("iPhoneClipType")
                : config.Exists("clipType") ? config.GetString("clipType") : null;
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
                if (viewType is not null and not "null")
                {
                    node = ClipCatalog.Create(viewType);
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
                    node.RotationRadians = 0f - float.DegreesToRadians(hashtable.GetFloat("rotation", 0f));
                }
                if (!hashtable.Exists("skipClip"))
                {
                    string clipType = GetClipType(hashtable);
                    BodyClip bodyClip = BodyClipFactory.Create(clipType, this, physics, node, hashtable);
                    if (bodyClip is null)
                    {
                        DebugUtil.Trace("type not found {0}", null, clipType);
                    }
                    return bodyClip;
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

        public static float ToRotationVec(Vector2 vec)
        {
            return ToRotation(VectorUtil.Atan2(vec));
        }

        public static float ToRotation(float angle)
        {
            return float.RadiansToDegrees(0f - angle);
        }

        public override void Update(float time)
        {
            float num = Math.Min(time, MaxWorldUpdateTime);
            World.Step(num * PhysicsSpeed);
            Updater.Update(time);
        }
    }
}
