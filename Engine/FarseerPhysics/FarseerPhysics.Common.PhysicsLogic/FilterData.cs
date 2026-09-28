using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public abstract class FilterData
{
    private Category DisabledOnCategories;

    private int DisabledOnGroup;

    private Category EnabledOnCategories = Category.All;

    private int EnabledOnGroup;

    public virtual bool IsActiveOn(Body body)
    {
        if (body == null || !body.Enabled || body.IsStatic)
        {
            return false;
        }
        if (body.FixtureList == null)
        {
            return false;
        }
        foreach (Fixture fixture in body.FixtureList)
        {
            if (fixture.CollisionGroup == DisabledOnGroup && fixture.CollisionGroup != 0 && DisabledOnGroup != 0)
            {
                return false;
            }
            if ((fixture.CollisionCategories & DisabledOnCategories) != Category.None)
            {
                return false;
            }
            if (EnabledOnGroup != 0 || EnabledOnCategories != Category.All)
            {
                if (fixture.CollisionGroup == EnabledOnGroup && fixture.CollisionGroup != 0 && EnabledOnGroup != 0)
                {
                    return true;
                }
                if ((fixture.CollisionCategories & EnabledOnCategories) != Category.None && EnabledOnCategories != Category.All)
                {
                    return true;
                }
                continue;
            }
            return true;
        }
        return false;
    }

    public void AddDisabledCategory(Category category)
    {
        DisabledOnCategories |= category;
    }

    public void RemoveDisabledCategory(Category category)
    {
        DisabledOnCategories &= ~category;
    }

    public bool IsInDisabledCategory(Category category)
    {
        return (DisabledOnCategories & category) == category;
    }

    public void AddEnabledCategory(Category category)
    {
        EnabledOnCategories |= category;
    }

    public void RemoveEnabledCategory(Category category)
    {
        EnabledOnCategories &= ~category;
    }

    public bool IsInEnabledInCategory(Category category)
    {
        return (EnabledOnCategories & category) == category;
    }
}
