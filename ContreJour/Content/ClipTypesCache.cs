using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mokus2D.Visual;

namespace ContreJour.Content;

public class ClipTypesCache
{
    private static List<string> SkipList = new List<string>(new string[4] { "McEndLevelView", "McRoseView", "McTeleportView", "McSpringView" });

    private static readonly string ClipsFolder = "ContreJour.Clips.";

    private static readonly string[] FolderNames = new string[18]
    {
        "level1", "spikes", "fakeHero", "chapter1", "chapter2", "chapter3", "chapter4", "chapter5", "chapter5Backgrounds", "chapter4Backgrounds",
        "chapter3More", "chapter6", "common", "common2", "lights", "planets", "finalLevel", "menuBackgrounds"
    };

    private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();

    public static Node CreateNewNode(string name)
    {
        if (!Cache.ContainsKey(name))
        {
            AddTypeToCache(name);
        }
        return (Node)Activator.CreateInstance(Cache[name]);
    }

    private static void AddTypeToCache(string name)
    {
        Type type = null;
        string[] folderNames = FolderNames;
        foreach (string text in folderNames)
        {
            type = Type.GetType(string.Format("{0}{1}.{2}", new object[3] { ClipsFolder, text, name }));
            if ((object)type != null)
            {
                Cache[name] = type;
                break;
            }
        }
        if ((object)type == null)
        {
            if (!SkipList.Contains(name))
            {
                Trace.TraceInformation("Cannot find " + name);
            }
            Cache[name] = typeof(Node);
        }
    }
}
