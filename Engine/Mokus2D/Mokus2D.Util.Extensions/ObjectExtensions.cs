using System.IO;
using System.Runtime.Serialization;

namespace Mokus2D.Util.Extensions;

public static class ObjectExtensions
{
    public static int ToInt(this bool value)
    {
        return !value ? 0 : 1;
    }

    public static T DeepClone<T>(this T a)
    {
        using MemoryStream memoryStream = new();
        DataContractSerializer dataContractSerializer = new(typeof(T));
        dataContractSerializer.WriteObject(memoryStream, a);
        memoryStream.Position = 0L;
        return (T)dataContractSerializer.ReadObject(memoryStream);
    }
}
