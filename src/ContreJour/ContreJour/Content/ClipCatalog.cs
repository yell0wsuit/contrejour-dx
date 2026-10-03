using System;
using System.Collections.Generic;
using System.Diagnostics;

using ContreJour.Clips;

using Mokus2D.Visual;

namespace ContreJour.Content
{
    // Creates the node a level or the game names by its bare clip name ("McDust"), as ClipTypesCache did by
    // reflection: the folders are searched in FolderNames order and the first that has the name wins, which
    // settles the names several folders share. Clips in other folders (loading, menu, menu2) were never found
    // by name and still are not.
    public static class ClipCatalog
    {
        private static readonly List<string> SkipList = ["McEndLevelView", "McRoseView", "McTeleportView", "McSpringView"];

        private static readonly string[] FolderNames =
        [
            "level1",
            "spikes",
            "fakeHero",
            "chapter1",
            "chapter2",
            "chapter3",
            "chapter4",
            "chapter5",
            "chapter5Backgrounds",
            "chapter4Backgrounds",
            "chapter3More",
            "chapter6",
            "newFriend",
            "common",
            "common2",
            "lights",
            "planets",
            "finalLevel",
            "menuBackgrounds"
        ];

        private static readonly Dictionary<string, Func<Node>> Cache = [];

        public static Node Create(string name)
        {
            if (!Cache.TryGetValue(name, out Func<Node> create))
            {
                ClipEntry entry = Find(name);
                if (entry is null && !SkipList.Contains(name))
                {
                    Trace.TraceInformation("Cannot find " + name);
                }
                create = entry is null ? static () => new Node() : entry.Create;
                Cache[name] = create;
            }
            return create();
        }

        public static ClipEntry Find(string name)
        {
            foreach (string folder in FolderNames)
            {
                ClipEntry entry = FindIn(folder, name);
                if (entry is not null)
                {
                    return entry;
                }
            }
            return null;
        }

        private static ClipEntry FindIn(string folder, string name)
        {
            return folder switch
            {
                "level1" => Level1(name),
                "spikes" => Spikes(name),
                "fakeHero" => FakeHero(name),
                "chapter1" => Chapter1(name),
                "chapter2" => Chapter2(name),
                "chapter3" => Chapter3(name),
                "chapter4" => Chapter4(name),
                "chapter5" => Chapter5(name),
                "chapter5Backgrounds" => Chapter5Backgrounds(name),
                "chapter4Backgrounds" => Chapter4Backgrounds(name),
                "chapter3More" => Chapter3More(name),
                "chapter6" => Chapter6(name),
                "newFriend" => NewFriend(name),
                "common" => Common(name),
                "common2" => Common2(name),
                "lights" => Lights(name),
                "planets" => Planets(name),
                "finalLevel" => FinalLevel(name),
                "menuBackgrounds" => MenuBackgrounds(name),
                _ => null,
            };
        }

        private static ClipEntry Chapter6(string name)
        {
            return name switch
            {
                "McBackgroundContent2_6" => new("chapter6/McBackgroundContent2_6", ClipKind.Sprite),
                "McBackgroundContent4_6" => new("chapter6/McBackgroundContent4_6", ClipKind.Sprite),
                "McBackgroundContent5_6" => new("chapter6/McBackgroundContent5_6", ClipKind.Sprite),
                "McBackgroundContent6_1" => new("chapter6/McBackgroundContent6_1", ClipKind.Sprite),
                "McBackgroundContent6_2" => new("chapter6/McBackgroundContent6_2", ClipKind.Sprite),
                "McBackgroundContent6_3" => new("chapter6/McBackgroundContent6_3", ClipKind.Sprite),
                "McBackgroundContent6_4" => new("chapter6/McBackgroundContent6_4", ClipKind.Sprite),
                "McBackgroundContent6_5" => new("chapter6/McBackgroundContent6_5", ClipKind.Sprite),
                "McChapter6Background" => new("chapter6/McChapter6Background", ClipKind.Sprite),
                "McChapter6Foreground" => new("chapter6/McChapter6Foreground", ClipKind.Sprite),
                "McCircleSpikesView_6" => new("chapter6/McCircleSpikesView_6", ClipKind.MovieClip),
                "McEggBridgeView_6" => new("chapter6/McEggBridgeView_6", ClipKind.Sprite),
                "McEggView0_6" => new("chapter6/McEggView0_6", ClipKind.Sprite),
                "McEggView1_6" => new("chapter6/McEggView1_6", ClipKind.Sprite),
                "McEggView2_6" => new("chapter6/McEggView2_6", ClipKind.Sprite),
                "McEggView3_6" => new("chapter6/McEggView3_6", ClipKind.Sprite),
                "McEyeBallHit_6" => new("chapter6/McEyeBallHit_6", ClipKind.MovieClip),
                "McEyeBallMonster_6" => new("chapter6/McEyeBallMonster_6", ClipKind.Sprite),
                "McEyeBall_6" => new("chapter6/McEyeBall_6", ClipKind.Sprite),
                "McFlowerHead_6" => new("chapter6/McFlowerHead_6", ClipKind.Sprite),
                "McGrass_6" => new("chapter6/McGrass_6", ClipKind.MovieClip),
                "McGreenPlanetFly" => new("chapter6/McGreenPlanetFly", ClipKind.Sprite),
                "McGroundPart_6" => new("chapter6/McGroundPart_6", ClipKind.Sprite),
                "McHeroShadow_6" => new("chapter6/McHeroShadow_6", ClipKind.Sprite),
                "McHeroView_6" => new("chapter6/McHeroView_6", ClipKind.Sprite),
                "McMoveHintView" => new("chapter6/McMoveHintView", ClipKind.Sprite),
                "McRotatableSpring_6" => new("chapter6/McRotatableSpring_6", ClipKind.MovieClip),
                "McSnotEnd_6" => new("chapter6/McSnotEnd_6", ClipKind.Sprite),
                "McSnotPoint" => new("chapter6/McSnotPoint", ClipKind.Sprite),
                "McSnotStart_6" => new("chapter6/McSnotStart_6", ClipKind.Sprite),
                "McSpikesView_6" => new("chapter6/McSpikesView_6", ClipKind.MovieClip),
                "McSpringView_6" => new("chapter6/McSpringView_6", ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry NewFriend(string name)
        {
            return name switch
            {
                "McAmieHintView" => new("newFriend/McAmieHintView", ClipKind.Sprite),
                "McBackgroundContent4_7" => new("newFriend/McBackgroundContent4_7", ClipKind.Sprite),
                "McBackgroundContent7_1" => new("newFriend/McBackgroundContent7_1", ClipKind.Sprite),
                "McBackgroundContent7_2" => new("newFriend/McBackgroundContent7_2", ClipKind.Sprite),
                "McBackgroundContent7_3" => new("newFriend/McBackgroundContent7_3", ClipKind.Sprite),
                "McBackgroundContent7_4" => new("newFriend/McBackgroundContent7_4", ClipKind.Sprite),
                "McBaloonGrassAll" => new("newFriend/McBaloonGrassAll", ClipKind.MovieClip),
                "McBaloonLegs" => new("newFriend/McBaloonLegs", ClipKind.Sprite),
                "McBaloonTailEnd" => new("newFriend/McBaloonTailEnd", ClipKind.Sprite),
                "McChapter5MenuBackground" => new("newFriend/McChapter5MenuBackground", ClipKind.Sprite),
                "McChapterVBlur" => new("newFriend/McChapterVBlur", ClipKind.Sprite),
                "McCircleSpikesView_7" => new("newFriend/McCircleSpikesView_7", ClipKind.MovieClip),
                "McDragView_7" => new("newFriend/McDragView_7", ClipKind.Sprite),
                "McDragSquareView_7" => new("newFriend/McDragView_7", ClipKind.Sprite),
                "McEggBridgeView_7" => new("newFriend/McEggBridgeView_7", ClipKind.Sprite),
                "McEggView0_7" => new("newFriend/McEggView0_7", ClipKind.Sprite),
                "McEggView1_7" => new("newFriend/McEggView1_7", ClipKind.Sprite),
                "McEggView2_7" => new("newFriend/McEggView2_7", ClipKind.Sprite),
                "McEggView3_7" => new("newFriend/McEggView3_7", ClipKind.Sprite),
                "McFakeHeroBackground" => new("newFriend/McFakeHeroBackground", ClipKind.Sprite),
                "McFakeHeroBackgroundBlack" => new("newFriend/McFakeHeroBackgroundBlack", ClipKind.Sprite),
                "McFakeHeroBackgroundWhite" => new("newFriend/McFakeHeroBackgroundWhite", ClipKind.Sprite),
                "McFakeHeroEye" => new("newFriend/McFakeHeroEye", ClipKind.Sprite),
                "McFakeHeroEyeBall" => new("newFriend/McFakeHeroEyeBall", ClipKind.Sprite),
                "McFakeHeroEyeBallBlack" => new("newFriend/McFakeHeroEyeBallBlack", ClipKind.Sprite),
                "McFakeHeroEyeBallWhite" => new("newFriend/McFakeHeroEyeBallWhite", ClipKind.Sprite),
                "McFakeHeroEyeBlack" => new("newFriend/McFakeHeroEyeBlack", ClipKind.Sprite),
                "McFakeHeroEyeBlink" => new("newFriend/McFakeHeroEyeBlink", ClipKind.MovieClip),
                "McFakeHeroEyeBlinkBlack" => new("newFriend/McFakeHeroEyeBlinkBlack", ClipKind.MovieClip),
                "McFakeHeroEyeOpen" => new("newFriend/McFakeHeroEyeOpen", ClipKind.MovieClip),
                "McFakeHeroEyeSmile" => new("newFriend/McFakeHeroEyeSmile", ClipKind.MovieClip),
                "McFakeHeroEyeSmileBlack" => new("newFriend/McFakeHeroEyeSmileBlack", ClipKind.MovieClip),
                "McFakeHeroHotspot" => new("newFriend/McFakeHeroHotspot", ClipKind.Sprite),
                "McFakeHeroHotspotBlack" => new("newFriend/McFakeHeroHotspotBlack", ClipKind.Sprite),
                "McFakeHeroHotspotWhite" => new("newFriend/McFakeHeroHotspotWhite", ClipKind.Sprite),
                "McFakeHeroShadow" => new("newFriend/McFakeHeroShadow", ClipKind.Sprite),
                "McFakeHeroShadowBlack" => new("newFriend/McFakeHeroShadowBlack", ClipKind.Sprite),
                "McFakeHeroShadowWhite" => new("newFriend/McFakeHeroShadowWhite", ClipKind.Sprite),
                "McGrass_7" => new("newFriend/McGrass_7", ClipKind.MovieClip),
                "McGroundPart_7" => new("newFriend/McGroundPart_7", ClipKind.Sprite),
                "McParticle_7" => new("newFriend/McParticle_7", ClipKind.Sprite),
                "McPlanetVBackground" => new("newFriend/McPlanetVBackground", ClipKind.Sprite),
                "McPlanetVBackground1" => new("newFriend/McPlanetVBackground1", ClipKind.Sprite),
                "McPlanetVBackground2" => new("newFriend/McPlanetVBackground2", ClipKind.Sprite),
                "McPlanetVLocked" => new("newFriend/McPlanetVLocked", ClipKind.Sprite),
                "McRelsHintView_7" => new("newFriend/McRelsHintView_7", ClipKind.Sprite),
                "McRotatorBase" => new("newFriend/McRotatorBase", ClipKind.Sprite),
                "McRoundDragFrameView_7" => new("newFriend/McRoundDragFrameView_7", ClipKind.Sprite),
                "McRoundDragView_7" => new("newFriend/McRoundDragView_7", ClipKind.Sprite),
                "McRoundDragSquareView_7" => new("newFriend/McRoundDragView_7", ClipKind.Sprite),
                "McSnotEndHighlite_7" => new("newFriend/McSnotEndHighlite_7", ClipKind.Sprite),
                "McSpringShadow_7" => new("newFriend/McSpringShadow_7", ClipKind.Sprite),
                "McSpringView_7" => new("newFriend/McSpringView_7", ClipKind.MovieClip),
                "McStrongSnotEnd_7" => new("newFriend/McStrongSnotEnd_7", ClipKind.Sprite),
                "strongSnotTexture_7" => new("newFriend/strongSnotTexture_7", ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry Level1(string name)
        {
            return name switch
            {
                "LystokAnimation" => new(ClipIds.Level1.LystokAnimation, ClipKind.Composite, static () => new Clips.level1.LystokAnimation()),
                "McBackground0Front" => new(ClipIds.Level1.McBackground0Front, ClipKind.Sprite),
                "McBackground1Front" => new(ClipIds.Level1.McBackground1Front, ClipKind.Sprite),
                "McBackground2" => new(ClipIds.Level1.McBackground2, ClipKind.Composite, static () => new Clips.level1.McBackground2()),
                "McBackground2Back" => new(ClipIds.Level1.McBackground2Back, ClipKind.Sprite),
                "McBlackSkyBackground" => new(ClipIds.Level1.McBlackSkyBackground, ClipKind.Sprite),
                "McIntroLogo" => new(ClipIds.Level1.McIntroLogo, ClipKind.Sprite),
                "McLystok1" => new(ClipIds.Level1.McLystok1, ClipKind.Composite, static () => new Clips.level1.McLystok1()),
                "McLystok1Content" => new(ClipIds.Level1.McLystok1Content, ClipKind.Sprite),
                "McLystok2" => new(ClipIds.Level1.McLystok2, ClipKind.Composite, static () => new Clips.level1.McLystok2()),
                "McLystok2Content" => new(ClipIds.Level1.McLystok2Content, ClipKind.Sprite),
                "McLystokMain" => new(ClipIds.Level1.McLystokMain, ClipKind.Composite, static () => new Clips.level1.McLystokMain()),
                "McLystokMainContent" => new(ClipIds.Level1.McLystokMainContent, ClipKind.Sprite),
                "McPuddle" => new(ClipIds.Level1.McPuddle, ClipKind.Composite, static () => new Clips.level1.McPuddle()),
                "McPuddleContent" => new(ClipIds.Level1.McPuddleContent, ClipKind.Sprite),
                "McRoseHeadBack" => new(ClipIds.Level1.McRoseHeadBack, ClipKind.Sprite),
                "McRoseHeadBase" => new(ClipIds.Level1.McRoseHeadBase, ClipKind.Sprite),
                "McRoseHeadBase2" => new(ClipIds.Level1.McRoseHeadBase2, ClipKind.Sprite),
                "McRoseHeadDown" => new(ClipIds.Level1.McRoseHeadDown, ClipKind.Composite, static () => new Clips.level1.McRoseHeadDown()),
                "McRoseHeadFront" => new(ClipIds.Level1.McRoseHeadFront, ClipKind.Composite, static () => new Clips.level1.McRoseHeadFront()),
                "McRoseHeadLight" => new(ClipIds.Level1.McRoseHeadLight, ClipKind.Sprite),
                "McRoseHeadLightDown" => new(ClipIds.Level1.McRoseHeadLightDown, ClipKind.Composite, static () => new Clips.level1.McRoseHeadLightDown()),
                "McRoseLightBlue" => new(ClipIds.Level1.McRoseLightBlue, ClipKind.Sprite),
                "McRoseLightBlue2" => new(ClipIds.Level1.McRoseLightBlue2, ClipKind.Sprite),
                "McStebloAnimation" => new(ClipIds.Level1.McStebloAnimation, ClipKind.MovieClip),
                "McSunBackground" => new(ClipIds.Level1.McSunBackground, ClipKind.Sprite),
                "McSunLight" => new(ClipIds.Level1.McSunLight, ClipKind.Sprite),
                "McTear" => new(ClipIds.Level1.McTear, ClipKind.MovieClip),
                "McTestAnimation" => new(ClipIds.Level1.McTestAnimation, ClipKind.Composite, static () => new Clips.level1.McTestAnimation()),
                "McTestAnimation2" => new(ClipIds.Level1.McTestAnimation2, ClipKind.Composite, static () => new Clips.level1.McTestAnimation2()),
                "PelustokNoLight" => new(ClipIds.Level1.PelustokNoLight, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Spikes(string name)
        {
            return name switch
            {
                "McCircleSpikesView" => new(ClipIds.Spikes.McCircleSpikesView, ClipKind.MovieClip),
                "McSimpleSpikesView" => new(ClipIds.Spikes.McSimpleSpikesView, ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry FakeHero(string name)
        {
            return name switch
            {
                "McEyeBall" => new(ClipIds.FakeHero.McEyeBall, ClipKind.Sprite),
                "McEyeSpotlight" => new(ClipIds.FakeHero.McEyeSpotlight, ClipKind.Sprite),
                "McFakeHeroBackground" => new(ClipIds.FakeHero.McFakeHeroBackground, ClipKind.Sprite),
                "McFakeHeroBackgroundBlack" => new(ClipIds.FakeHero.McFakeHeroBackgroundBlack, ClipKind.Sprite),
                "McFakeHeroBackgroundWhite" => new(ClipIds.FakeHero.McFakeHeroBackgroundWhite, ClipKind.Sprite),
                "McFakeHeroBackground_6" => new(ClipIds.FakeHero.McFakeHeroBackground_6, ClipKind.Sprite),
                "McFakeHeroEye" => new(ClipIds.FakeHero.McFakeHeroEye, ClipKind.Sprite),
                "McFakeHeroEyeBall" => new(ClipIds.FakeHero.McFakeHeroEyeBall, ClipKind.Sprite),
                "McFakeHeroEyeBallBlack" => new(ClipIds.FakeHero.McFakeHeroEyeBallBlack, ClipKind.Sprite),
                "McFakeHeroEyeBallWhite" => new(ClipIds.FakeHero.McFakeHeroEyeBallWhite, ClipKind.Sprite),
                "McFakeHeroEyeBall_6" => new(ClipIds.FakeHero.McFakeHeroEyeBall_6, ClipKind.Sprite),
                "McFakeHeroEyeBlack" => new(ClipIds.FakeHero.McFakeHeroEyeBlack, ClipKind.Sprite),
                "McFakeHeroEyeBlink" => new(ClipIds.FakeHero.McFakeHeroEyeBlink, ClipKind.MovieClip),
                "McFakeHeroEyeBlinkBlack" => new(ClipIds.FakeHero.McFakeHeroEyeBlinkBlack, ClipKind.MovieClip),
                "McFakeHeroEyeOpen" => new(ClipIds.FakeHero.McFakeHeroEyeOpen, ClipKind.MovieClip),
                "McFakeHeroEyeSmile" => new(ClipIds.FakeHero.McFakeHeroEyeSmile, ClipKind.MovieClip),
                "McFakeHeroEyeSmileBlack" => new(ClipIds.FakeHero.McFakeHeroEyeSmileBlack, ClipKind.MovieClip),
                "McFakeHeroHotspot" => new(ClipIds.FakeHero.McFakeHeroHotspot, ClipKind.Sprite),
                "McFakeHeroHotspotBlack" => new(ClipIds.FakeHero.McFakeHeroHotspotBlack, ClipKind.Sprite),
                "McFakeHeroHotspotWhite" => new(ClipIds.FakeHero.McFakeHeroHotspotWhite, ClipKind.Sprite),
                "McFakeHeroHotspot_6" => new(ClipIds.FakeHero.McFakeHeroHotspot_6, ClipKind.Sprite),
                "McFakeHeroShadow" => new(ClipIds.FakeHero.McFakeHeroShadow, ClipKind.Sprite),
                "McFakeHeroShadowBlack" => new(ClipIds.FakeHero.McFakeHeroShadowBlack, ClipKind.Sprite),
                "McFakeHeroShadowWhite" => new(ClipIds.FakeHero.McFakeHeroShadowWhite, ClipKind.Sprite),
                "McFakeHeroShadow_6" => new(ClipIds.FakeHero.McFakeHeroShadow_6, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter1(string name)
        {
            return name switch
            {
                "McBackSnotBase" => new(ClipIds.Chapter1.McBackSnotBase, ClipKind.Sprite),
                "McBackSnotEye" => new(ClipIds.Chapter1.McBackSnotEye, ClipKind.Sprite),
                "McBackSnotEyeBall" => new(ClipIds.Chapter1.McBackSnotEyeBall, ClipKind.Sprite),
                "McBackSnotEyeBlink" => new(ClipIds.Chapter1.McBackSnotEyeBlink, ClipKind.MovieClip),
                "McBackSnotEyeBlinkOneTime" => new(ClipIds.Chapter1.McBackSnotEyeBlinkOneTime, ClipKind.MovieClip),
                "McBackground5Content" => new(ClipIds.Chapter1.McBackground5Content, ClipKind.Sprite),
                "McBackgroundCrabContent" => new(ClipIds.Chapter1.McBackgroundCrabContent, ClipKind.Sprite),
                "McBackgroundSkyContent" => new(ClipIds.Chapter1.McBackgroundSkyContent, ClipKind.Sprite),
                "McFlowerHead" => new(ClipIds.Chapter1.McFlowerHead, ClipKind.Sprite),
                "McGroundHintView" => new(ClipIds.Chapter1.McGroundHintView, ClipKind.Sprite),
                "McLightsHintView" => new(ClipIds.Chapter1.McLightsHintView, ClipKind.Sprite),
                "McMoreTentaclesHintView" => new(ClipIds.Chapter1.McMoreTentaclesHintView, ClipKind.Sprite),
                "McMultitouch2HintView" => new(ClipIds.Chapter1.McMultitouch2HintView, ClipKind.Sprite),
                "McMultitouchHintView" => new(ClipIds.Chapter1.McMultitouchHintView, ClipKind.Sprite),
                "McPinchHintView" => new(ClipIds.Chapter1.McPinchHintView, ClipKind.Sprite),
                "McRetryHintView" => new(ClipIds.Chapter1.McRetryHintView, ClipKind.Sprite),
                "McSkipHintView" => new(ClipIds.Chapter1.McSkipHintView, ClipKind.Sprite),
                "McSlingshotHintView" => new(ClipIds.Chapter1.McSlingshotHintView, ClipKind.Sprite),
                "McSnotHintView" => new(ClipIds.Chapter1.McSnotHintView, ClipKind.Sprite),
                "McSnotReleaseHintView" => new(ClipIds.Chapter1.McSnotReleaseHintView, ClipKind.Sprite),
                "McStoneView0" => new(ClipIds.Chapter1.McStoneView0, ClipKind.Sprite),
                "McStoneView1" => new(ClipIds.Chapter1.McStoneView1, ClipKind.Sprite),
                "McStoneView2" => new(ClipIds.Chapter1.McStoneView2, ClipKind.Sprite),
                "McStoneView3" => new(ClipIds.Chapter1.McStoneView3, ClipKind.Sprite),
                "McStoneView4" => new(ClipIds.Chapter1.McStoneView4, ClipKind.Sprite),
                "McStoneView5" => new(ClipIds.Chapter1.McStoneView5, ClipKind.Sprite),
                "McStoneView6" => new(ClipIds.Chapter1.McStoneView6, ClipKind.Sprite),
                "McStripesHintView" => new(ClipIds.Chapter1.McStripesHintView, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter2(string name)
        {
            return name switch
            {
                "McBackgroundContent2_1" => new(ClipIds.Chapter2.McBackgroundContent2_1, ClipKind.Sprite),
                "McBackgroundContent2_2" => new(ClipIds.Chapter2.McBackgroundContent2_2, ClipKind.Sprite),
                "McBackgroundContent2_3" => new(ClipIds.Chapter2.McBackgroundContent2_3, ClipKind.Sprite),
                "McBackgroundContent2_4" => new(ClipIds.Chapter2.McBackgroundContent2_4, ClipKind.Sprite),
                "McBackgroundContent2_6" => new(ClipIds.Chapter2.McBackgroundContent2_6, ClipKind.Sprite),
                "McBackgroundStoneView1Black" => new(ClipIds.Chapter2.McBackgroundStoneView1Black, ClipKind.Sprite),
                "McBackgroundStoneView2Black" => new(ClipIds.Chapter2.McBackgroundStoneView2Black, ClipKind.Sprite),
                "McBlackGround0" => new(ClipIds.Chapter2.McBlackGround0, ClipKind.Sprite),
                "McBlackGround1" => new(ClipIds.Chapter2.McBlackGround1, ClipKind.Sprite),
                "McCircleSpikesView_1" => new(ClipIds.Chapter2.McCircleSpikesView_1, ClipKind.MovieClip),
                "McEyeBallBlack" => new(ClipIds.Chapter2.McEyeBallBlack, ClipKind.Sprite),
                "McEyeBallHitBlack" => new(ClipIds.Chapter2.McEyeBallHitBlack, ClipKind.Composite, static () => new Clips.chapter2.McEyeBallHitBlack()),
                "McEyeBallMonsterBlack" => new(ClipIds.Chapter2.McEyeBallMonsterBlack, ClipKind.Sprite),
                "McHeroBlackView" => new(ClipIds.Chapter2.McHeroBlackView, ClipKind.Sprite),
                "McHeroSmokeBlack" => new(ClipIds.Chapter2.McHeroSmokeBlack, ClipKind.Sprite),
                "McKaktusView0Black" => new(ClipIds.Chapter2.McKaktusView0Black, ClipKind.Sprite),
                "McKaktusView3Black" => new(ClipIds.Chapter2.McKaktusView3Black, ClipKind.Sprite),
                "McPortal2HintView" => new(ClipIds.Chapter2.McPortal2HintView, ClipKind.Sprite),
                "McPortalHintView" => new(ClipIds.Chapter2.McPortalHintView, ClipKind.Sprite),
                "McSnotEndBlack" => new(ClipIds.Chapter2.McSnotEndBlack, ClipKind.Sprite),
                "McSnotStartBlack" => new(ClipIds.Chapter2.McSnotStartBlack, ClipKind.Sprite),
                "McSpringViewBlack" => new(ClipIds.Chapter2.McSpringViewBlack, ClipKind.MovieClip),
                "McStoneLongBlackView" => new(ClipIds.Chapter2.McStoneLongBlackView, ClipKind.Sprite),
                "McStoneView7" => new(ClipIds.Chapter2.McStoneView7, ClipKind.Sprite),
                "McStoneView8" => new(ClipIds.Chapter2.McStoneView8, ClipKind.Sprite),
                "McStrongSnotEndBlack" => new(ClipIds.Chapter2.McStrongSnotEndBlack, ClipKind.Sprite),
                "McTailTexture" => new(ClipIds.Chapter2.McTailTexture, ClipKind.Sprite),
                "McTrampolineEndBlack" => new(ClipIds.Chapter2.McTrampolineEndBlack, ClipKind.Sprite),
                "McTrampolinePathBlack" => new(ClipIds.Chapter2.McTrampolinePathBlack, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter3(string name)
        {
            return name switch
            {
                "McBackground3_0Content" => new(ClipIds.Chapter3.McBackground3_0Content, ClipKind.Sprite),
                "McBackground3_0Foreground" => new(ClipIds.Chapter3.McBackground3_0Foreground, ClipKind.Sprite),
                "McBackground3_1Content" => new(ClipIds.Chapter3.McBackground3_1Content, ClipKind.Sprite),
                "McBackground3_1Foreground" => new(ClipIds.Chapter3.McBackground3_1Foreground, ClipKind.Sprite),
                "McBackground3_2Content" => new(ClipIds.Chapter3.McBackground3_2Content, ClipKind.Sprite),
                "McBackground3_2Foreground" => new(ClipIds.Chapter3.McBackground3_2Foreground, ClipKind.Sprite),
                "McBackground3_4Content" => new(ClipIds.Chapter3.McBackground3_4Content, ClipKind.Sprite),
                "McBackground3_4Foreground" => new(ClipIds.Chapter3.McBackground3_4Foreground, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter4(string name)
        {
            return name switch
            {
                "McCircleSpikesViewWhite" => new(ClipIds.Chapter4.McCircleSpikesViewWhite, ClipKind.MovieClip),
                "McDragViewMiddleWhite" => new(ClipIds.Chapter4.McDragViewMiddleWhite, ClipKind.Sprite),
                "McDragViewWhite" => new(ClipIds.Chapter4.McDragViewWhite, ClipKind.Sprite),
                "McDustWhite" => new(ClipIds.Chapter4.McDustWhite, ClipKind.Sprite),
                "McEggView0" => new(ClipIds.Chapter4.McEggView0, ClipKind.Sprite),
                "McEggView1" => new(ClipIds.Chapter4.McEggView1, ClipKind.Sprite),
                "McEggView2" => new(ClipIds.Chapter4.McEggView2, ClipKind.Sprite),
                "McEggView3" => new(ClipIds.Chapter4.McEggView3, ClipKind.Sprite),
                "McEyeBallHitWhite" => new(ClipIds.Chapter4.McEyeBallHitWhite, ClipKind.Composite, static () => new Clips.chapter4.McEyeBallHitWhite()),
                "McEyeBallMonsterWhite" => new(ClipIds.Chapter4.McEyeBallMonsterWhite, ClipKind.Sprite),
                "McEyeBallWhite" => new(ClipIds.Chapter4.McEyeBallWhite, ClipKind.Sprite),
                "McFlowerHeadWhite" => new(ClipIds.Chapter4.McFlowerHeadWhite, ClipKind.Sprite),
                "McGroundCircle0" => new(ClipIds.Chapter4.McGroundCircle0, ClipKind.Sprite),
                "McGroundCircle1" => new(ClipIds.Chapter4.McGroundCircle1, ClipKind.Sprite),
                "McGroundPartWhite" => new(ClipIds.Chapter4.McGroundPartWhite, ClipKind.Sprite),
                "McHeroShadowWhite" => new(ClipIds.Chapter4.McHeroShadowWhite, ClipKind.Sprite),
                "McHeroWhiteView" => new(ClipIds.Chapter4.McHeroWhiteView, ClipKind.Sprite),
                "McKaktusView0" => new(ClipIds.Chapter4.McKaktusView0, ClipKind.Sprite),
                "McKaktusView1" => new(ClipIds.Chapter4.McKaktusView1, ClipKind.Sprite),
                "McKaktusView2" => new(ClipIds.Chapter4.McKaktusView2, ClipKind.Sprite),
                "McKaktusView4" => new(ClipIds.Chapter4.McKaktusView4, ClipKind.Sprite),
                "McRoundDragSquareView" => new(ClipIds.Chapter4.McRoundDragSquareView, ClipKind.Sprite),
                "McRoundDragViewWhite" => new(ClipIds.Chapter4.McRoundDragViewWhite, ClipKind.Sprite),
                "McSimpleSpikesViewWhite" => new(ClipIds.Chapter4.McSimpleSpikesViewWhite, ClipKind.MovieClip),
                "McSnotEndWhite" => new(ClipIds.Chapter4.McSnotEndWhite, ClipKind.Sprite),
                "McSnotStartWhite" => new(ClipIds.Chapter4.McSnotStartWhite, ClipKind.Sprite),
                "McSpikesCenterWhite" => new(ClipIds.Chapter4.McSpikesCenterWhite, ClipKind.Sprite),
                "McSpikesPartWhite" => new(ClipIds.Chapter4.McSpikesPartWhite, ClipKind.MovieClip),
                "McSpikesPartWhiteRight" => new(ClipIds.Chapter4.McSpikesPartWhiteRight, ClipKind.MovieClip),
                "McSpikesViewWhite" => new(ClipIds.Chapter4.McSpikesViewWhite, ClipKind.Composite, static () => new Clips.chapter4.McSpikesViewWhite()),
                "McSpringShadowWhite" => new(ClipIds.Chapter4.McSpringShadowWhite, ClipKind.Sprite),
                "McSpringViewWhite" => new(ClipIds.Chapter4.McSpringViewWhite, ClipKind.MovieClip),
                "McStrongSnotEndWhite" => new(ClipIds.Chapter4.McStrongSnotEndWhite, ClipKind.Sprite),
                "McTailTextureWhite" => new(ClipIds.Chapter4.McTailTextureWhite, ClipKind.Sprite),
                "McTeleportPartRed" => new(ClipIds.Chapter4.McTeleportPartRed, ClipKind.Sprite),
                "McTrampolineEndWhite" => new(ClipIds.Chapter4.McTrampolineEndWhite, ClipKind.Sprite),
                "McWhiteGrass" => new(ClipIds.Chapter4.McWhiteGrass, ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry Chapter5(string name)
        {
            return name switch
            {
                "Mc5ChapterParticle" => new(ClipIds.Chapter5.Mc5ChapterParticle, ClipKind.Sprite),
                "McBaloonGrassAll" => new(ClipIds.Chapter5.McBaloonGrassAll, ClipKind.Composite, static () => new Clips.chapter5.McBaloonGrassAll()),
                "McBaloonLegs" => new(ClipIds.Chapter5.McBaloonLegs, ClipKind.Sprite),
                "McBaloonTailEnd" => new(ClipIds.Chapter5.McBaloonTailEnd, ClipKind.Composite, static () => new Clips.chapter5.McBaloonTailEnd()),
                "McBridgeHideHintView" => new(ClipIds.Chapter5.McBridgeHideHintView, ClipKind.Sprite),
                "McBridgeHintView" => new(ClipIds.Chapter5.McBridgeHintView, ClipKind.Sprite),
                "McCircleSpikesView_5" => new(ClipIds.Chapter5.McCircleSpikesView_5, ClipKind.MovieClip),
                "McEggBridgeView_5" => new(ClipIds.Chapter5.McEggBridgeView_5, ClipKind.Sprite),
                "McEggView0_5" => new(ClipIds.Chapter5.McEggView0_5, ClipKind.Sprite),
                "McEggView2_5" => new(ClipIds.Chapter5.McEggView2_5, ClipKind.Sprite),
                "McEggView3_5" => new(ClipIds.Chapter5.McEggView3_5, ClipKind.Sprite),
                "McEyeBall" => new(ClipIds.Chapter5.McEyeBall, ClipKind.Sprite),
                "McEyeDead" => new(ClipIds.Chapter5.McEyeDead, ClipKind.Sprite),
                "McEyeSpotlight" => new(ClipIds.Chapter5.McEyeSpotlight, ClipKind.Sprite),
                "McFlyBody" => new(ClipIds.Chapter5.McFlyBody, ClipKind.Sprite),
                "McFlyWing" => new(ClipIds.Chapter5.McFlyWing, ClipKind.Sprite),
                "McFurCircle" => new(ClipIds.Chapter5.McFurCircle, ClipKind.Sprite),
                "McGrass_5" => new(ClipIds.Chapter5.McGrass_5, ClipKind.MovieClip),
                "McKaktusView0_5" => new(ClipIds.Chapter5.McKaktusView0_5, ClipKind.Sprite),
                "McKaktusView2_5" => new(ClipIds.Chapter5.McKaktusView2_5, ClipKind.Sprite),
                "McKaktusView3_5" => new(ClipIds.Chapter5.McKaktusView3_5, ClipKind.Sprite),
                "McRotatableSpring" => new(ClipIds.Chapter5.McRotatableSpring, ClipKind.MovieClip),
                "McRotatableSpringHintView" => new(ClipIds.Chapter5.McRotatableSpringHintView, ClipKind.Sprite),
                "McRotatorArrowPart" => new(ClipIds.Chapter5.McRotatorArrowPart, ClipKind.MovieClip),
                "McRotatorBase" => new(ClipIds.Chapter5.McRotatorBase, ClipKind.Sprite),
                "McRotatorCircle" => new(ClipIds.Chapter5.McRotatorCircle, ClipKind.Sprite),
                "McRotatorGrass0" => new(ClipIds.Chapter5.McRotatorGrass0, ClipKind.Sprite),
                "McRotatorGrass1" => new(ClipIds.Chapter5.McRotatorGrass1, ClipKind.Sprite),
                "McRotatorGrass2" => new(ClipIds.Chapter5.McRotatorGrass2, ClipKind.Sprite),
                "McRotatorGrassAll" => new(ClipIds.Chapter5.McRotatorGrassAll, ClipKind.Composite, static () => new Clips.chapter5.McRotatorGrassAll()),
                "McRotatorHintView" => new(ClipIds.Chapter5.McRotatorHintView, ClipKind.Sprite),
                "McRotatorPoint" => new(ClipIds.Chapter5.McRotatorPoint, ClipKind.Sprite),
                "McRotatorTouch" => new(ClipIds.Chapter5.McRotatorTouch, ClipKind.Sprite),
                "McRoundDragView_5" => new(ClipIds.Chapter5.McRoundDragView_5, ClipKind.Sprite),
                "McSimpleSpikesView_5" => new(ClipIds.Chapter5.McSimpleSpikesView_5, ClipKind.MovieClip),
                "McSpringBridgeHintView" => new(ClipIds.Chapter5.McSpringBridgeHintView, ClipKind.Sprite),
                "McSpringTouchPoint" => new(ClipIds.Chapter5.McSpringTouchPoint, ClipKind.Sprite),
                "McSuckerBody" => new(ClipIds.Chapter5.McSuckerBody, ClipKind.Sprite),
                "McSuckerBodyStrong" => new(ClipIds.Chapter5.McSuckerBodyStrong, ClipKind.Sprite),
                "McSuckerHighlite" => new(ClipIds.Chapter5.McSuckerHighlite, ClipKind.Sprite),
                "McSuckerLegs" => new(ClipIds.Chapter5.McSuckerLegs, ClipKind.Sprite),
                "McSuckerStrongSnotTexture" => new(ClipIds.Chapter5.McSuckerStrongSnotTexture, ClipKind.Sprite),
                "McSuckerStrongSnotTextureipad3" => new(ClipIds.Chapter5.McSuckerStrongSnotTextureipad3, ClipKind.Sprite),
                "McTear" => new(ClipIds.Chapter5.McTear, ClipKind.MovieClip),
                "McTubusBase" => new(ClipIds.Chapter5.McTubusBase, ClipKind.Sprite),
                "McTubusBody" => new(ClipIds.Chapter5.McTubusBody, ClipKind.Sprite),
                "McTubusEyeBackground" => new(ClipIds.Chapter5.McTubusEyeBackground, ClipKind.Sprite),
                "McTubusEyeBall" => new(ClipIds.Chapter5.McTubusEyeBall, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter5Backgrounds(string name)
        {
            return name switch
            {
                "McBackgroundContent5_1" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_1, ClipKind.Composite, static () => new Clips.chapter5Backgrounds.McBackgroundContent5_1()),
                "McBackgroundContent5_2" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_2, ClipKind.Composite, static () => new Clips.chapter5Backgrounds.McBackgroundContent5_2()),
                "McBackgroundContent5_3" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_3, ClipKind.Sprite),
                "McBackgroundContent5_4" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_4, ClipKind.Composite, static () => new Clips.chapter5Backgrounds.McBackgroundContent5_4()),
                "McBackgroundContent5_5" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_5, ClipKind.Sprite),
                "McBackgroundContent5_6" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_6, ClipKind.Sprite),
                "McBackgroundContent5_7" => new(ClipIds.Chapter5Backgrounds.McBackgroundContent5_7, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter4Backgrounds(string name)
        {
            return name switch
            {
                "McBackgroundContent4_3" => new(ClipIds.Chapter4Backgrounds.McBackgroundContent4_3, ClipKind.Sprite),
                "McBackgroundContent4_4" => new(ClipIds.Chapter4Backgrounds.McBackgroundContent4_4, ClipKind.Sprite),
                "McBackgroundContent4_5" => new(ClipIds.Chapter4Backgrounds.McBackgroundContent4_5, ClipKind.Composite, static () => new Clips.chapter4Backgrounds.McBackgroundContent4_5()),
                "McBackgroundContent4_6" => new(ClipIds.Chapter4Backgrounds.McBackgroundContent4_6, ClipKind.Sprite),
                "McBackgroundContent4_7" => new(ClipIds.Chapter4Backgrounds.McBackgroundContent4_7, ClipKind.Sprite),
                "McColorFix" => new(ClipIds.Chapter4Backgrounds.McColorFix, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Chapter3More(string name)
        {
            return name switch
            {
                "McRelsHintView" => new(ClipIds.Chapter3More.McRelsHintView, ClipKind.Sprite),
                "McShesterna2Background" => new(ClipIds.Chapter3More.McShesterna2Background, ClipKind.Sprite),
                "McShesternaBackground" => new(ClipIds.Chapter3More.McShesternaBackground, ClipKind.Sprite),
                "McShesternaBackgroundBlur" => new(ClipIds.Chapter3More.McShesternaBackgroundBlur, ClipKind.Sprite),
                "McShesternaOut" => new(ClipIds.Chapter3More.McShesternaOut, ClipKind.Sprite),
                "McStoneLongView" => new(ClipIds.Chapter3More.McStoneLongView, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Common(string name)
        {
            return name switch
            {
                "McAngryStickView" => new(ClipIds.Common.McAngryStickView, ClipKind.Sprite),
                "McArrowView" => new(ClipIds.Common.McArrowView, ClipKind.Sprite),
                "McBackgroundStoneView0" => new(ClipIds.Common.McBackgroundStoneView0, ClipKind.Sprite),
                "McBackgroundStoneView1" => new(ClipIds.Common.McBackgroundStoneView1, ClipKind.Sprite),
                "McBackgroundStoneView2" => new(ClipIds.Common.McBackgroundStoneView2, ClipKind.Sprite),
                "McBackgroundStoneView3" => new(ClipIds.Common.McBackgroundStoneView3, ClipKind.Sprite),
                "McBlackSquare" => new(ClipIds.Common.McBlackSquare, ClipKind.Sprite),
                "McCircleSpikesView" => new(ClipIds.Common.McCircleSpikesView, ClipKind.MovieClip),
                "McCircleSpikesViewBlack" => new(ClipIds.Common.McCircleSpikesViewBlack, ClipKind.MovieClip),
                "McDragLimit" => new(ClipIds.Common.McDragLimit, ClipKind.Sprite),
                "McDragSquareView" => new(ClipIds.Common.McDragSquareView, ClipKind.Sprite),
                "McDragView_5" => new(ClipIds.Common.McDragView_5, ClipKind.Sprite),
                "McDust" => new(ClipIds.Common.McDust, ClipKind.Sprite),
                "McEggBridgeView" => new(ClipIds.Common.McEggBridgeView, ClipKind.Sprite),
                "McEnergyBall" => new(ClipIds.Common.McEnergyBall, ClipKind.Sprite),
                "McEnergyView" => new(ClipIds.Common.McEnergyView, ClipKind.Sprite),
                "McEye" => new(ClipIds.Common.McEye, ClipKind.Sprite),
                "McEyeAngry" => new(ClipIds.Common.McEyeAngry, ClipKind.MovieClip),
                "McEyeBall" => new(ClipIds.Common.McEyeBall, ClipKind.Sprite),
                "McEyeBallHit" => new(ClipIds.Common.McEyeBallHit, ClipKind.MovieClip),
                "McEyeBallMonster" => new(ClipIds.Common.McEyeBallMonster, ClipKind.Sprite),
                "McEyeBallWhite" => new(ClipIds.Common.McEyeBallWhite, ClipKind.Sprite),
                "McEyeBlink" => new(ClipIds.Common.McEyeBlink, ClipKind.MovieClip),
                "McEyeBlinkMonster" => new(ClipIds.Common.McEyeBlinkMonster, ClipKind.MovieClip),
                "McEyeBlinkOneTime" => new(ClipIds.Common.McEyeBlinkOneTime, ClipKind.MovieClip),
                "McEyeBlinkOneTimeMonster" => new(ClipIds.Common.McEyeBlinkOneTimeMonster, ClipKind.MovieClip),
                "McEyeClose" => new(ClipIds.Common.McEyeClose, ClipKind.MovieClip),
                "McEyeCloseMonster" => new(ClipIds.Common.McEyeCloseMonster, ClipKind.MovieClip),
                "McEyeClosePause" => new(ClipIds.Common.McEyeClosePause, ClipKind.MovieClip),
                "McEyeCloseSlow" => new(ClipIds.Common.McEyeCloseSlow, ClipKind.MovieClip),
                "McEyeDead" => new(ClipIds.Common.McEyeDead, ClipKind.Sprite),
                "McEyeHit" => new(ClipIds.Common.McEyeHit, ClipKind.MovieClip),
                "McEyeMonster" => new(ClipIds.Common.McEyeMonster, ClipKind.Sprite),
                "McEyeOpen" => new(ClipIds.Common.McEyeOpen, ClipKind.MovieClip),
                "McEyeOpenMonster" => new(ClipIds.Common.McEyeOpenMonster, ClipKind.MovieClip),
                "McEyeSleep" => new(ClipIds.Common.McEyeSleep, ClipKind.MovieClip),
                "McEyeSmile" => new(ClipIds.Common.McEyeSmile, ClipKind.MovieClip),
                "McEyeSpotlight" => new(ClipIds.Common.McEyeSpotlight, ClipKind.Sprite),
                "McEyeWink" => new(ClipIds.Common.McEyeWink, ClipKind.MovieClip),
                "McFallParticle" => new(ClipIds.Common.McFallParticle, ClipKind.MovieClip),
                "McFinishPart" => new(ClipIds.Common.McFinishPart, ClipKind.Sprite),
                "McFlowerHead" => new(ClipIds.Common.McFlowerHead, ClipKind.Sprite),
                "McFly" => new(ClipIds.Common.McFly, ClipKind.Sprite),
                "McGreySmoke" => new(ClipIds.Common.McGreySmoke, ClipKind.Sprite),
                "McGroundPart" => new(ClipIds.Common.McGroundPart, ClipKind.Sprite),
                "McGroundPartBlack" => new(ClipIds.Common.McGroundPartBlack, ClipKind.Sprite),
                "McHeroBackView" => new(ClipIds.Common.McHeroBackView, ClipKind.Sprite),
                "McHeroShadow" => new(ClipIds.Common.McHeroShadow, ClipKind.Sprite),
                "McHotspotwhite" => new(ClipIds.Common.McHotspotwhite, ClipKind.Sprite),
                "McLeafView0" => new(ClipIds.Common.McLeafView0, ClipKind.Sprite),
                "McLeafView1" => new(ClipIds.Common.McLeafView1, ClipKind.Sprite),
                "McLeafView2" => new(ClipIds.Common.McLeafView2, ClipKind.Sprite),
                "McLeafView3" => new(ClipIds.Common.McLeafView3, ClipKind.Sprite),
                "McLeafView4" => new(ClipIds.Common.McLeafView4, ClipKind.Sprite),
                "McLeafView5" => new(ClipIds.Common.McLeafView5, ClipKind.Sprite),
                "McRedDot" => new(ClipIds.Common.McRedDot, ClipKind.Sprite),
                "McRicochetView" => new(ClipIds.Common.McRicochetView, ClipKind.Sprite),
                "McRotatableEggView0" => new(ClipIds.Common.McRotatableEggView0, ClipKind.Sprite),
                "McRoundDragFrameView" => new(ClipIds.Common.McRoundDragFrameView, ClipKind.Sprite),
                "McRoundDragSquareView" => new(ClipIds.Common.McRoundDragSquareView, ClipKind.Sprite),
                "McShadowView0" => new(ClipIds.Common.McShadowView0, ClipKind.Sprite),
                "McSimpleSpikesViewBlack" => new(ClipIds.Common.McSimpleSpikesViewBlack, ClipKind.MovieClip),
                "McSkipIcon" => new(ClipIds.Common.McSkipIcon, ClipKind.Sprite),
                "McSmokeBlack" => new(ClipIds.Common.McSmokeBlack, ClipKind.Sprite),
                "McSnotEnd" => new(ClipIds.Common.McSnotEnd, ClipKind.Sprite),
                "McSnotEndHighlite" => new(ClipIds.Common.McSnotEndHighlite, ClipKind.Sprite),
                "McSnotStart" => new(ClipIds.Common.McSnotStart, ClipKind.Sprite),
                "McSnowParticle" => new(ClipIds.Common.McSnowParticle, ClipKind.Sprite),
                "McSpikesCenter" => new(ClipIds.Common.McSpikesCenter, ClipKind.Sprite),
                "McSpikesFlowerShadow" => new(ClipIds.Common.McSpikesFlowerShadow, ClipKind.Sprite),
                "McSpikesPart" => new(ClipIds.Common.McSpikesPart, ClipKind.MovieClip),
                "McSpikesView" => new(ClipIds.Common.McSpikesView, ClipKind.Composite, static () => new Clips.common.McSpikesView()),
                "McSpringShadow" => new(ClipIds.Common.McSpringShadow, ClipKind.Sprite),
                "McSpringShadow_5" => new(ClipIds.Common.McSpringShadow_5, ClipKind.Sprite),
                "McSpringView_5" => new(ClipIds.Common.McSpringView_5, ClipKind.MovieClip),
                "McStrongSnotEnd" => new(ClipIds.Common.McStrongSnotEnd, ClipKind.Sprite),
                "McSuckerStart" => new(ClipIds.Common.McSuckerStart, ClipKind.Sprite),
                "McTailTexture" => new(ClipIds.Common.McTailTexture, ClipKind.Sprite),
                "McTailTextureWhite" => new(ClipIds.Common.McTailTextureWhite, ClipKind.Sprite),
                "McTeleportPart" => new(ClipIds.Common.McTeleportPart, ClipKind.Sprite),
                "McTeleportPartBlack" => new(ClipIds.Common.McTeleportPartBlack, ClipKind.Sprite),
                "McTeleportPartBlue" => new(ClipIds.Common.McTeleportPartBlue, ClipKind.Sprite),
                "McTotalGrass" => new(ClipIds.Common.McTotalGrass, ClipKind.MovieClip),
                "McTrampolineEnd" => new(ClipIds.Common.McTrampolineEnd, ClipKind.Sprite),
                "McTrampolinePath" => new(ClipIds.Common.McTrampolinePath, ClipKind.Sprite),
                "McWhiteSmoke" => new(ClipIds.Common.McWhiteSmoke, ClipKind.Sprite),
                "McWhiteSmokeBlack" => new(ClipIds.Common.McWhiteSmokeBlack, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Common2(string name)
        {
            return name switch
            {
                "McEyeAngryBlack" => new(ClipIds.Common2.McEyeAngryBlack, ClipKind.MovieClip),
                "McEyeBallWhite" => new(ClipIds.Common2.McEyeBallWhite, ClipKind.Sprite),
                "McEyeBlack" => new(ClipIds.Common2.McEyeBlack, ClipKind.Sprite),
                "McEyeBlinkBlack" => new(ClipIds.Common2.McEyeBlinkBlack, ClipKind.MovieClip),
                "McEyeBlinkMonsterBlack" => new(ClipIds.Common2.McEyeBlinkMonsterBlack, ClipKind.MovieClip),
                "McEyeBlinkOneTimeBlack" => new(ClipIds.Common2.McEyeBlinkOneTimeBlack, ClipKind.MovieClip),
                "McEyeBlinkOneTimeMonsterBlack" => new(ClipIds.Common2.McEyeBlinkOneTimeMonsterBlack, ClipKind.MovieClip),
                "McEyeCloseBlack" => new(ClipIds.Common2.McEyeCloseBlack, ClipKind.MovieClip),
                "McEyeCloseMonsterBlack" => new(ClipIds.Common2.McEyeCloseMonsterBlack, ClipKind.MovieClip),
                "McEyeClosePauseBlack" => new(ClipIds.Common2.McEyeClosePauseBlack, ClipKind.MovieClip),
                "McEyeCloseSlowBlack" => new(ClipIds.Common2.McEyeCloseSlowBlack, ClipKind.MovieClip),
                "McEyeDeadBlack" => new(ClipIds.Common2.McEyeDeadBlack, ClipKind.Sprite),
                "McEyeMonsterBlack" => new(ClipIds.Common2.McEyeMonsterBlack, ClipKind.Sprite),
                "McEyeOpenBlack" => new(ClipIds.Common2.McEyeOpenBlack, ClipKind.MovieClip),
                "McEyeOpenMonsterBlack" => new(ClipIds.Common2.McEyeOpenMonsterBlack, ClipKind.MovieClip),
                "McEyeSleepBlack" => new(ClipIds.Common2.McEyeSleepBlack, ClipKind.MovieClip),
                "McEyeSmileBlack" => new(ClipIds.Common2.McEyeSmileBlack, ClipKind.MovieClip),
                "McEyeWinkBlack" => new(ClipIds.Common2.McEyeWinkBlack, ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry Lights(string name)
        {
            return name switch
            {
                "McColorFix" => new(ClipIds.Lights.McColorFix, ClipKind.Sprite),
                "McLightView1" => new(ClipIds.Lights.McLightView1, ClipKind.Composite, static () => new Clips.lights.McLightView1()),
                "McLightView11" => new(ClipIds.Lights.McLightView11, ClipKind.Composite, static () => new Clips.lights.McLightView11()),
                "McLightView11ContentExport" => new(ClipIds.Lights.McLightView11ContentExport, ClipKind.Sprite),
                "McLightView12" => new(ClipIds.Lights.McLightView12, ClipKind.Composite, static () => new Clips.lights.McLightView12()),
                "McLightView1Content" => new(ClipIds.Lights.McLightView1Content, ClipKind.Sprite),
                "McLightView2" => new(ClipIds.Lights.McLightView2, ClipKind.Composite, static () => new Clips.lights.McLightView2()),
                "McLightView7" => new(ClipIds.Lights.McLightView7, ClipKind.Composite, static () => new Clips.lights.McLightView7()),
                "McLightView7Content" => new(ClipIds.Lights.McLightView7Content, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry Planets(string name)
        {
            return name switch
            {
                "McChapter1Blur" => new(ClipIds.Planets.McChapter1Blur, ClipKind.Sprite),
                "McChapter2Blur" => new(ClipIds.Planets.McChapter2Blur, ClipKind.Sprite),
                "McChapter3Blur" => new(ClipIds.Planets.McChapter3Blur, ClipKind.Sprite),
                "McChapterLockedBlur" => new(ClipIds.Planets.McChapterLockedBlur, ClipKind.Sprite),
                "McGreenPlanetFly" => new(ClipIds.Planets.McGreenPlanetFly, ClipKind.Sprite),
                "McPlanet1Background" => new(ClipIds.Planets.McPlanet1Background, ClipKind.Sprite),
                "McPlanet1EyeBall" => new(ClipIds.Planets.McPlanet1EyeBall, ClipKind.Sprite),
                "McPlanet1Foreground" => new(ClipIds.Planets.McPlanet1Foreground, ClipKind.Sprite),
                "McPlanet2Background" => new(ClipIds.Planets.McPlanet2Background, ClipKind.Sprite),
                "McPlanet3Background" => new(ClipIds.Planets.McPlanet3Background, ClipKind.Sprite),
                "McPlanet3Foreground" => new(ClipIds.Planets.McPlanet3Foreground, ClipKind.Sprite),
                "McPlanet3Light" => new(ClipIds.Planets.McPlanet3Light, ClipKind.Sprite),
                "McPlanetCommingBackground" => new(ClipIds.Planets.McPlanetCommingBackground, ClipKind.Sprite),
                "McPlanetCross" => new(ClipIds.Planets.McPlanetCross, ClipKind.Sprite),
                "McPlanetEye" => new(ClipIds.Planets.McPlanetEye, ClipKind.Sprite),
                "McPlanetEyeBlink" => new(ClipIds.Planets.McPlanetEyeBlink, ClipKind.MovieClip),
                "McPlanetEyeBlinkOneTime" => new(ClipIds.Planets.McPlanetEyeBlinkOneTime, ClipKind.MovieClip),
                "McPlanetLocked" => new(ClipIds.Planets.McPlanetLocked, ClipKind.Sprite),
                "McPlanetRoseLight" => new(ClipIds.Planets.McPlanetRoseLight, ClipKind.Sprite),
                "McPlanetShesterna" => new(ClipIds.Planets.McPlanetShesterna, ClipKind.Sprite),
                "McPlanetShesterna2" => new(ClipIds.Planets.McPlanetShesterna2, ClipKind.Sprite),
                "McPlanetShesterna3" => new(ClipIds.Planets.McPlanetShesterna3, ClipKind.Sprite),
                "McPlanetShesternaBlur" => new(ClipIds.Planets.McPlanetShesternaBlur, ClipKind.Sprite),
                "McPlanetSpringBack" => new(ClipIds.Planets.McPlanetSpringBack, ClipKind.Sprite),
                "McPlanetSpringView" => new(ClipIds.Planets.McPlanetSpringView, ClipKind.Sprite),
                "McPlanetSpringView_58" => new(ClipIds.Planets.McPlanetSpringView_58, ClipKind.Sprite),
                "McPlanetStick" => new(ClipIds.Planets.McPlanetStick, ClipKind.Composite, static () => new Clips.planets.McPlanetStick()),
                "McPlanetStickBall" => new(ClipIds.Planets.McPlanetStickBall, ClipKind.Sprite),
                "McPlanetStickContent" => new(ClipIds.Planets.McPlanetStickContent, ClipKind.Sprite),
                "McPlanetStickGraphics" => new(ClipIds.Planets.McPlanetStickGraphics, ClipKind.Composite, static () => new Clips.planets.McPlanetStickGraphics()),
                "McPlanetTablo" => new(ClipIds.Planets.McPlanetTablo, ClipKind.Sprite),
                _ => null,
            };
        }

        private static ClipEntry FinalLevel(string name)
        {
            return name switch
            {
                "McEndRose" => new(ClipIds.FinalLevel.McEndRose, ClipKind.MovieClip),
                _ => null,
            };
        }

        private static ClipEntry MenuBackgrounds(string name)
        {
            return name switch
            {
                "McBackground4Content" => new(ClipIds.MenuBackgrounds.McBackground4Content, ClipKind.Sprite),
                "McBackgroundContent1_5" => new(ClipIds.MenuBackgrounds.McBackgroundContent1_5, ClipKind.Sprite),
                _ => null,
            };
        }
    }
}
