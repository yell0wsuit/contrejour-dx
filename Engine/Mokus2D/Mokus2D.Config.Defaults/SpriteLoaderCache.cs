using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Content;
using Mokus2D.Util;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Config.Defaults;

public class SpriteLoaderCache : IGraphicsLoader
{
    private readonly Dictionary<string, object> _data = [];

    private readonly Dictionary<string, object> _textureNodeData = [];

    private readonly IGraphicsLoader _baseLoader;

    private float _currentScaleFactor = 1f;

    public string GraphicsRootDirectory
    {
        get => _baseLoader.GraphicsRootDirectory;
        set => _baseLoader.GraphicsRootDirectory = value;
    }

    public bool IsAbsolutePath
    {
        get => _baseLoader.IsAbsolutePath;
        set => _baseLoader.IsAbsolutePath = value;
    }

    public bool FallbackToDefaultScaleFactor
    {
        get => _baseLoader.FallbackToDefaultScaleFactor;
        set => _baseLoader.FallbackToDefaultScaleFactor = value;
    }

    public float PrefferedScaleFactor
    {
        get => _currentScaleFactor;
        set
        {
            if (value != _currentScaleFactor)
            {
                _currentScaleFactor = value;
                _textureNodeData.Clear();
                _baseLoader.PrefferedScaleFactor = value;
            }
        }
    }

    public event Action<string, object> ResourceLoaded;

    public SpriteLoaderCache(IGraphicsLoader baseLoader)
    {
        _baseLoader = baseLoader;
        baseLoader.ResourceLoaded += BaseLoaderOnResourceLoaded;
    }

    public static void SetPrefferedScaleFactor(float mult)
    {
    }

    public T Load<T>(string name)
    {
        name = FixName(name);
        Dictionary<string, object> dictionary = typeof(ITextureNodeData).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) ? _textureNodeData : _data;
        T val;
        if (!dictionary.TryGetValue(name, out object cached))
        {
            val = _baseLoader.Load<T>(name);
        }
        else
        {
            val = (T)cached;
            RefreshTexture(val as ITextureNodeData);
        }
        return val;
    }

    private static string FixName(string name)
    {
        return name.Replace('\\', '/');
    }

    public void Clear()
    {
        _data.Clear();
    }

    private static void RefreshTexture(ITextureNodeData result)
    {
        if (result != null && result.Texture.IsDisposed)
        {
            result.Texture = Mokus2DGame.ContentManager.Load<Texture2D>(result.TextureName);
        }
    }

    private void BaseLoaderOnResourceLoaded(string name, object data)
    {
        Dictionary<string, object> dictionary = (data is ITextureNodeData) ? _textureNodeData : _data;
        if (!dictionary.ContainsKey(name) || !FallbackToDefaultScaleFactor)
        {
            dictionary[name] = data;
            ResourceLoaded.Dispatch(name, data);
        }
    }
}
