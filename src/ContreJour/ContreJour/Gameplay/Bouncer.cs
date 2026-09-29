using System;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class Bouncer : IUpdatable
    {
        private readonly CosChanger changer;

        public float Amplitude { get; set; }

        public float CurrentAmplitude { get; private set; }

        public float AmplitudeStep { get; set; }

        public float Step
        {
            get => changer.Step;
            set => changer.Step = value;
        }

        public float Value => changer.Value * CurrentAmplitude;

        [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
        public Bouncer(float amplitude, float amplitudeStep, float step)
        {
            changer = new CosChanger(-1f, 1f, step);
            Amplitude = amplitude;
            AmplitudeStep = amplitudeStep;
        }

        public void Start()
        {
            CurrentAmplitude = Amplitude;
            changer.Progress = (float)Math.PI / 2f;
        }

        public void Update(float time)
        {
            if (Maths.FuzzyNotEquals(CurrentAmplitude, 0f))
            {
                CurrentAmplitude = Maths.StepTo(CurrentAmplitude, 0f, AmplitudeStep * time);
                changer.Update(time);
            }
        }
    }
}
