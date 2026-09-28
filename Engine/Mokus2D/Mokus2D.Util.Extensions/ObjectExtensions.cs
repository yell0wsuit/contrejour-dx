using System.IO;
using System.Runtime.Serialization;

namespace Mokus2D.Util.Extensions;

public static class ObjectExtensions
{
	public static int ToInt(this bool value)
	{
		if (!value)
		{
			return 0;
		}
		return 1;
	}

	public static T DeepClone<T>(this T a)
	{
		using MemoryStream memoryStream = new MemoryStream();
		DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
		dataContractSerializer.WriteObject(memoryStream, a);
		memoryStream.Position = 0L;
		return (T)dataContractSerializer.ReadObject(memoryStream);
	}
}
