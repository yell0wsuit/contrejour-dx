using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mokus2D.Visual;
using Windows.Storage;

namespace ContreJour.WinRT;

public class LiveTileDrawer
{
    private const int Capacity = 512000;

    private readonly Vector2 _size;

    public readonly string FilePath;

    private readonly LiveTileAnimation _animation;

    private readonly RenderRootNode _root;

    public LiveTileDrawer(Vector2 size, string filePath, LiveTileAnimation animation)
    {
        _size = size;
        FilePath = filePath;
        _animation = animation;
        _root = new RenderRootNode(size);
        _root.AddChild(_animation);
    }

    public async Task<bool> DrawLiveTile(int stars)
    {
        MemoryStream stream = new MemoryStream(512000);
        try
        {
            _animation.Count = stars;
            _root.UpdateAndDraw(0f);
            await Task.Run(delegate
            {
                _root.RenderTarget.SaveAsPng(stream, (int)_size.X, (int)_size.Y);
            });
            TaskAwaiter<StorageFile> taskAwaiter = WindowsRuntimeSystemExtensions.GetAwaiter<StorageFile>(ApplicationData.Current.LocalFolder.CreateFileAsync(FilePath, (CreationCollisionOption)1));
            if (!taskAwaiter.IsCompleted)
            {
                await taskAwaiter;
                TaskAwaiter<StorageFile> taskAwaiter2 = default(TaskAwaiter<StorageFile>);
                taskAwaiter = taskAwaiter2;
            }
            TaskAwaiter taskAwaiter3 = WindowsRuntimeSystemExtensions.GetAwaiter(FileIO.WriteBytesAsync((IStorageFile)(object)taskAwaiter.GetResult(), stream.ToArray()));
            if (!taskAwaiter3.IsCompleted)
            {
                await taskAwaiter3;
                TaskAwaiter taskAwaiter4 = default(TaskAwaiter);
                taskAwaiter3 = taskAwaiter4;
            }
            taskAwaiter3.GetResult();
            return true;
        }
        finally
        {
            if (stream != null)
            {
                ((IDisposable)stream).Dispose();
            }
        }
    }
}
