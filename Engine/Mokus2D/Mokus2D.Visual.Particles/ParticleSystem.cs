using System;
using System.Collections.Generic;

using Mokus2D.Util;
using Mokus2D.Visual.Particles.Data;

namespace Mokus2D.Visual.Particles;

public class ParticleSystem : Node
{
    public ParticleSystemConfig ParticlesConfig;

    private readonly List<ParticleData> _visibleParticles = new List<ParticleData>();

    private readonly List<ParticleData> _invisibleParticles = new List<ParticleData>();

    private readonly List<ParticleData> _reusableParticles = new List<ParticleData>();

    private readonly List<ParticleData> _toHide = new List<ParticleData>();

    private float _timeToCreate;

    public bool CanCreateParticles = true;

    public Predicate<ParticleData> RemovePredicate;

    public int VisibleParticles => _visibleParticles.Count;

    public event Action FinishEvent;

    protected ParticleSystem()
    {
    }

    public ParticleSystem(string id)
        : this(Mokus2DGame.LoadResource<ParticleSystemConfig>(id))
    {
    }

    public ParticleSystem(ParticleSystemConfig config)
    {
        Initialize(config);
    }

    protected void Initialize(ParticleSystemConfig config)
    {
        ParticlesConfig = config;
        _timeToCreate = config.CreateDelay.GetValueInRange();
    }

    public void Clear()
    {
        RemoveAllChildren();
        _visibleParticles.Clear();
        _invisibleParticles.Clear();
        _timeToCreate = 0f;
    }

    public void FastForward(float time)
    {
        float num = (float)Mokus2DGame.Instance.TargetElapsedTime.TotalSeconds;
        while (time > 0f)
        {
            Update(Math.Min(time, num));
            time -= num;
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        time *= ParticlesConfig.UpdateSpeed;
        UpdateParticles(time);
        CreateParticles(time);
    }

    public void Refresh()
    {
        _reusableParticles.AddItemsNoGarbage(_visibleParticles);
        _reusableParticles.AddItemsNoGarbage(_invisibleParticles);
        _visibleParticles.Clear();
        _invisibleParticles.Clear();
    }

    internal override void SetInstanceConfig(IDictionary<string, string> config)
    {
        if (base.Config == null)
        {
            SetMainConfig(new Dictionary<string, string>());
        }
        base.SetInstanceConfig(config);
    }

    private void CreateParticles(float time)
    {
        _timeToCreate -= time;
        while (_timeToCreate < 0f)
        {
            int num = _visibleParticles.Count;
            if (!ParticlesConfig.ReuseParticles)
            {
                num += _invisibleParticles.Count;
            }
            if (CanCreateParticles && num < ParticlesConfig.MaxParticles && !ParticlesConfig.IsEmpty)
            {
                CreateParticle();
            }
            _timeToCreate += ParticlesConfig.CreateDelay.GetValueInRange();
        }
    }

    private void CreateParticle()
    {
        ParticleData particleData = null;
        if (!_reusableParticles.Empty())
        {
            particleData = RefreshLastParticle(_reusableParticles);
        }
        else if (!_invisibleParticles.Empty())
        {
            particleData = RefreshLastParticle(_invisibleParticles);
        }
        else
        {
            particleData = ParticlesConfig.CreateParticleData();
            AddChild(particleData.Particle);
        }
        _visibleParticles.Add(particleData);
    }

    private ParticleData RefreshLastParticle(List<ParticleData> particlesList)
    {
        ParticleData particleData = particlesList.RemoveLast();
        particleData.Particle.Visible = true;
        ParticlesConfig.Initialize(particleData);
        return particleData;
    }

    private void UpdateParticles(float time)
    {
        foreach (ParticleData visibleParticle in _visibleParticles)
        {
            visibleParticle.Update(time);
            if (visibleParticle.HasRemove || (RemovePredicate != null && RemovePredicate(visibleParticle)))
            {
                visibleParticle.Particle.Visible = false;
                _toHide.Add(visibleParticle);
            }
        }
        _visibleParticles.RemoveListNoGarbage(_toHide);
        _invisibleParticles.AddItemsNoGarbage(_toHide);
        _toHide.Clear();
        if (_visibleParticles.Empty() && _invisibleParticles.Count >= ParticlesConfig.MaxParticles)
        {
            this.FinishEvent.Dispatch();
        }
    }
}
