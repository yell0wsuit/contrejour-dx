using Mokus2D.Interfaces;

namespace Mokus2D.Behaviour;

public interface INodeController : IUpdatable
{
	void OnRemovedFromStage();

	void OnAddedToStage();

	void FirstUpdate();
}
