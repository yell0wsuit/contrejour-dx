using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public static class BodyClipFactory
    {
        public static BodyClip Create(string clipType, LevelBuilderBase builder, object physics, Node clip, Hashtable config)
        {
            return clipType switch
            {
                null => new BodyClip(builder, physics, clip, config),
                "AlphaForeground" => new AlphaForeground((ContreJourLevelBuilder)builder, physics, clip, config),
                "BackSnotBodyClip" => new BackSnotBodyClip(builder, (SnotData)physics, clip, config),
                "BridgeHideHint" => new BridgeHideHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "BridgeHint" => new BridgeHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "DestroyOnHitClip" => new DestroyOnHitClip(builder, physics, clip, config),
                "DragableBodyClip" => new DragableBodyClip((ContreJourLevelBuilder)builder, physics, clip, config),
                "EndHeroBodyClip" => new EndHeroBodyClip(builder, physics, (Sprite)clip, config),
                "EndLastLevelBodyClip" => new EndLastLevelBodyClip(builder, physics, clip, config),
                "EndLevelBodyClip" => new EndLevelBodyClip(builder, physics, clip, config),
                "EndRoseBodyClip" => new EndRoseBodyClip(builder, physics, (Sprite)clip, config),
                "EnergyBodyClip" => new EnergyBodyClip(builder, physics, clip, config),
                "FadeHint" => new FadeHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "FlyBodyClip" => new FlyBodyClip(builder, physics, clip, config),
                "HeroBodyClip" => new HeroBodyClip(builder, physics, clip, config),
                "JoinableSpringBodyClip" => new JoinableSpringBodyClip(builder, physics, clip, config),
                "KaktusBodyClip" => new KaktusBodyClip((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "KaktusForeground" => new KaktusForeground((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "LeafBodyClip" => new LeafBodyClip((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "LianaBodyClip" => new LianaBodyClip(builder, (LianaData)physics, clip, config),
                "LightsHint" => new LightsHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "MovableSpringBodyClip" => new MovableSpringBodyClip(builder, physics, clip, config),
                "MoveCharHint" => new MoveCharHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "MoveHint" => new MoveHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "MultitouchHint" => new MultitouchHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "PlasticineBodyClip" => new PlasticineBodyClip(builder, (List<Vector2>)physics, clip, config),
                "Portal2Hint" => new Portal2Hint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "PortalHint" => new PortalHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "RelsHint" => new RelsHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "RoseBodyClip" => new RoseBodyClip(builder, physics, clip, config),
                "RotatableSpringBodyClip" => new RotatableSpringBodyClip(builder, physics, clip, config),
                "RotatableSpringHint" => new RotatableSpringHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "RotatorBodyClip" => new RotatorBodyClip((ContreJourLevelBuilder)builder, physics, clip, config),
                "RoundDragBodyClip" => new RoundDragBodyClip((ContreJourLevelBuilder)builder, physics, clip, config),
                "SimpleSpikesBodyClip" => new SimpleSpikesBodyClip(builder, physics, clip, config),
                "SlingshotHint" => new SlingshotHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "SnotBodyClip" => new SnotBodyClip(builder, (SnotData)physics, clip, config),
                "SnotLinkHint" => new SnotLinkHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "SnotPoint" => physics is Vector2 position
                    ? new SnotPoint(builder, position, clip, config)
                    : new SnotPoint(builder, physics, clip, config),
                "SnotReleaseHint" => new SnotReleaseHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "SpikesFlowerBodyClip" => new SpikesFlowerBodyClip((ContreJourLevelBuilder)builder, physics, clip, config),
                "SpringBodyClip" => new SpringBodyClip(builder, physics, clip, config),
                "SpringBridgeHint" => new SpringBridgeHint((ContreJourLevelBuilder)builder, physics, (Sprite)clip, config),
                "SpringSuckerBodyClip" => new SpringSuckerBodyClip(builder, physics, clip, config),
                "StickyBodyClip" => new StickyBodyClip(builder, physics, clip, config),
                "StoneBodyClip" => new StoneBodyClip(builder, physics, clip, config),
                "StrongSnotBodyClip" => new StrongSnotBodyClip(builder, (SnotData)physics, clip, config),
                "SuckerBodyClip" => new SuckerBodyClip(builder, physics, clip, config),
                "TeleportBodyClip" => new TeleportBodyClip(builder, physics, clip, config),
                "TrampolineBodyClip" => new TrampolineBodyClip(builder, (SnotData)physics, clip, config),
                _ => null,
            };
        }
    }
}
