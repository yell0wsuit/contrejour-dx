namespace Mokus2D.Util.Factory;

public interface IFactory<out T>
{
	T New();
}
