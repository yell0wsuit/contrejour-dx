using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class SnotBodyClipBase : ContreJourBodyClip
    {
        protected MonsterEye Eye { get; set; }


        protected SnotSprite ClipContent { get; set; }

        protected Node BaseClip { get; set; }

        protected Node BaseEndClip { get; set; }

        protected Node Container { get; set; }

        protected float EndWidthPixels { get; set; }

        protected float CenterWidth { get; set; }

        protected float EndWidth { get; set; }

        protected float StartWidthPixels { get; set; }

        protected float StartWidth { get; set; }

        public SnotData Physics { get; }

        public virtual Vector2 StartPosition => Physics.GetWorldStartPoint();

        public virtual Body EyeBody => Physics.EyeBody;

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public SnotBodyClipBase(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
            : base(builder, body.EndBody, null, config)
        {
            Physics = body;
            Container = new Node();
            Physics.Snot = this;
            InitSizes();
            ClipContent = CreateClip();
            BaseClip = ClipCatalog.Create(BaseClipName());
            BaseClip.Position = Builder.ToIPadPoint(Physics.GetWorldStartPoint());
            BaseEndClip = ClipCatalog.Create(BaseEndClipName());
            Physics.EndBody.ApplyLinearImpulse(new Vector2(Maths.Random(), Maths.Random()) * Physics.EndBody.Mass);
            Eye = CreateEye();
            AddClipsToStage();
        }

        public virtual void InitSizes()
        {
            EndWidthPixels = 10f;
            CenterWidth = 6f * Builder.EngineConfig.SizeMultiplier;
            EndWidth = EndWidthPixels * Builder.EngineConfig.SizeMultiplier;
            StartWidthPixels = 28f;
            StartWidth = StartWidthPixels * Builder.EngineConfig.SizeMultiplier;
        }

        public virtual int Layer()
        {
            return 0;
        }

        public virtual string BaseEndClipName()
        {
            return "McSnotEnd";
        }

        public virtual string BaseClipName()
        {
            return "McSnotStart";
        }

        public virtual void AddClipsToStage()
        {
            Container.AddChild(ClipContent);
            Container.AddChild(BaseEndClip);
            Container.AddChild(BaseClip);
            if (Eye != null)
            {
                Container.AddChild(Eye);
            }
            Builder.Add(Container, Layer());
        }

        protected virtual MonsterEye CreateEye()
        {
            return new MonsterEye((ContreJourGame)Builder.Game, visible: false, Physics.EyeBody.Position);
        }

        public virtual SnotSprite CreateClip()
        {
            return new SnotSprite(this, StartWidth, CenterWidth, EndWidth);
        }

        public virtual Vector2 EndPosition()
        {
            return Physics.EndBody.Position;
        }

        public override void Update(float time)
        {
            base.Update(time);
            BaseClip.Position = Builder.ToIPadPoint(Physics.GetWorldStartPoint());
            BaseEndClip.Position = Builder.ToIPadPoint(EndPosition());
            if (Eye != null && Eye.HasToUpdate)
            {
                Eye.Position = Builder.ToIPadPoint(EyeBody.Position);
                Eye.UpdateNode(time);
            }
        }
    }
}
