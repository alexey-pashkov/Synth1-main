using System;

namespace Synth_1
{
    enum EnvelopeState
    {
        Attack,
        Decay,
        Sustain,
        Release
    }

    public class ADSR
    {
        EnvelopeState state;

        double attak;
        double decay;
        double sustain;
        double release;

        double step;
        double coef;

        public ADSR(double a, double d, double s, double r)
        {
            attak = a;
            decay = d;
            sustain = s;
            release = r;
            coef = 0;
            step = 1 / (441 * attak * 100);
            state = EnvelopeState.Attack;
        }

        public double Process()
        {
            switch(state)
            { 
                case EnvelopeState.Attack:
                    coef += step;
                    if(coef == 1)
                    {
                        state = EnvelopeState.Decay;
                        step = 1 / (4410 * decay * 100);
                    }
                    break;
                case EnvelopeState.Decay:
                    coef -= step;
                    if(coef == sustain)
                    {
                        state = EnvelopeState.Sustain;
                    }
                    break;
                case EnvelopeState.Sustain:
                    return coef;
            }
            return coef;
        }

    }
}