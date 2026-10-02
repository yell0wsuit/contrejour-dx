using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Integration.Farseer.Construction.Processors;
using Mokus2D.Integration.Farseer.Physics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Mokus2D.Integration.Farseer.Construction
{
    public class PhysicsConstructor : PhysicsTransform
    {
        private static readonly Category[] Categories =
        [
            Category.None,
            Category.Cat1,
            Category.Cat2,
            Category.Cat3,
            Category.Cat4,
            Category.Cat5,
            Category.Cat6,
            Category.Cat7,
            Category.Cat8,
            Category.Cat9,
            Category.Cat10,
            Category.Cat11,
            Category.Cat12,
            Category.Cat13,
            Category.Cat14,
            Category.Cat15,
            Category.Cat16,
            Category.Cat17,
            Category.Cat18,
            Category.Cat19,
            Category.Cat20,
            Category.Cat21,
            Category.Cat22,
            Category.Cat23,
            Category.Cat24,
            Category.Cat25,
            Category.Cat26,
            Category.Cat27,
            Category.Cat28,
            Category.Cat29,
            Category.Cat30,
            Category.Cat31
        ];

        private readonly float Density = 1f;

        public IPhysicsConfigProcessor ConfigProcessor { get; set; }
        private readonly World World;

        private readonly Dictionary<string, ShapeProcessor> _processors = [];

        private readonly Dictionary<string, JointProcessor> _jointProcessors = [];

        private Body _bodyToAttach;

        private readonly Dictionary<string, Body> _createdBodies = [];

        public Node PhysicsContainer { get; }

        public PhysicsConstructor(World world, Node physicsContainer, float physicsToPixels)
            : base(physicsToPixels)
        {
            World = world;
            PhysicsContainer = physicsContainer;
            _processors["square"] = new SquareProcessor(this);
            _processors["triangle"] = new TriangleProcessor(this);
            _processors["circle"] = new CircleProcessor(this);
            _jointProcessors["revoluteJoint"] = new RevoluteJointProcessor(this);
            _jointProcessors["ropeJoint"] = new RopeJointProcessor(this);
        }

        public Vector2 ToPhysics(Node item)
        {
            return ToPhysics(Vector2.Zero, item);
        }

        public Body GetCreatedBody(string name)
        {
            return _createdBodies[name];
        }

        public Vector2 ToPhysics(Vector2 position, Node item, Vector2 positionOffset = default)
        {
            Vector2 pixels = item.LocalToNode(position, PhysicsContainer) + positionOffset;
            pixels = ToPhysics(pixels);
            return _bodyToAttach != null ? _bodyToAttach.GetLocalPoint(pixels) : pixels;
        }

        public void CreatePhysics(Node parent, List<Body> result = null, List<Joint> joints = null)
        {
            CreateBodies(parent, result);
            CreateJoints(parent, joints);
        }

        private void CreateBodies(Node parent, List<Body> result)
        {
            Body body = new(World);
            foreach (Node child in parent.Children)
            {
                InitializeBody(child, body);
                AttachShapes(body, child);
                if (body.FixtureList.Count != 0)
                {
                    IDictionary<string, string> config = child.Config;
                    if (config != null)
                    {
                        ProcessConfig(config, body);
                    }
                    result?.Add(body);
                    body = new Body(World);
                }
            }
            if (body.FixtureList.Count == 0)
            {
                World.RemoveBody(body);
            }
        }

        private void CreateJoints(Node parent, List<Joint> result = null)
        {
            foreach (Node child in parent.Children)
            {
                if (child.Config != null)
                {
                    string text = child.Config.GetString("joint");
                    if (text != null)
                    {
                        JointProcessor jointProcessor = _jointProcessors[text];
                        Joint joint = jointProcessor.Process(child);
                        World.AddJoint(joint);
                        result?.Add(joint);
                    }
                }
            }
        }

        private void InitializeBody(Node child, Body body)
        {
            Vector2 pixels = child.ZeroToNode(PhysicsContainer);
            body.Position = ToPhysics(pixels);
            body.Rotation = child.GetRootRotationRadians();
            if (child.Config != null)
            {
                if (child.Config.GetBool("dynamic"))
                {
                    body.BodyType = BodyType.Dynamic;
                }
                else if (child.Config.GetBool("kinematic"))
                {
                    body.BodyType = BodyType.Kinematic;
                }
            }
            if (child.Name != null)
            {
                _createdBodies[child.Name] = body;
            }
        }

        private void ProcessConfig(IDictionary<string, string> config, Body body)
        {
            body.IsSensor = config.GetBool("sensor");
            SetCollisionProperties(config, body);
            body.UserData = config;
            ConfigProcessor?.ProcessConfig(config, body);
        }

        private static void SetCollisionProperties(IDictionary<string, string> config, Body body)
        {
            string text = config.GetString("collisionCategories");
            if (!string.IsNullOrEmpty(text))
            {
                Category category = Category.None;
                string[] array = text.Split([',']);
                string[] array2 = array;
                foreach (string s in array2)
                {
                    category |= Categories[Convert.ToInt32(s, CultureInfo.InvariantCulture)];
                }
                body.CollisionCategories = category;
            }
        }

        public void AttachShapes(Body body, Node parent)
        {
            _bodyToAttach = body;
            List<ShapeAndConfig> shapes = CreateShapes(parent);
            AddShapes(body, shapes);
            _bodyToAttach = null;
        }

        private static void AddShapes(Body body, List<ShapeAndConfig> shapes)
        {
            foreach (ShapeAndConfig shape in shapes)
            {
                Fixture fixture = body.CreateFixture(shape.Shape);
                if (shape.Config != null)
                {
                    ApplyFixtureConfig(fixture, shape.Config);
                }
            }
        }

        public static void ApplyFixtureConfig(Fixture fixture, IDictionary<string, string> config)
        {
            fixture.UserData = config;
            if (config.GetBool("sensor"))
            {
                fixture.IsSensor = true;
            }
        }

        public List<ShapeAndConfig> CreateShapes(Node parent, Vector2 positionOffset = default)
        {
            List<ShapeAndConfig> result = [];
            CreateShape(parent, result, positionOffset);
            foreach (Node child in parent.Children)
            {
                CreateShape(child, result, positionOffset);
            }
            return result;
        }

        private void CreateShape(Node child, List<ShapeAndConfig> result, Vector2 positionOffset = default)
        {
            string name = child.GetType().Name;
            IDictionary<string, string> config = child.Config;
            Shape shape = ProcessByType(child, positionOffset, name);
            if (shape == null && config != null && config.TryGetValue("shape", out string shapeName))
            {
                shape = ProcessByType(child, positionOffset, shapeName);
            }
            if (shape != null)
            {
                result.Add(new ShapeAndConfig(shape, config));
            }
        }

        private Shape ProcessByType(Node child, Vector2 positionOffset, string type)
        {
            if (_processors.TryGetValue(type, out ShapeProcessor processor))
            {
                Shape result = processor.Process(child, positionOffset);
                child.Visible = false;
                return result;
            }
            return null;
        }

        public float GetDensity()
        {
            return Density;
        }
    }
}
