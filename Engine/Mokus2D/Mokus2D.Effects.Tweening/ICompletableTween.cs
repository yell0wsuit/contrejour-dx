using System;

using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening;

public interface ICompletableTween : ITween, ICleanable, IUpdatable
{
    ICompletableTween OnComplete(Action action);
}
