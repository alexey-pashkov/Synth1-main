using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public enum WaveType
    {
        Sine,
        Square,
        Saw,
        Triangle,
        Noise
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate double Out();


    /// <summary>
    /// 
    /// </summary>
    public class Generator
    {

        #region Aggregations


        #endregion

        #region Compositions


        #endregion

        #region Attributes
        /// <summary>
        /// 
        /// </summary>
        private protected Dictionary<WaveType, Out> dic;

        /// <summary>
        /// 
        /// </summary>
        private protected WaveType waveType;

        /// <summary>
        /// 
        /// </summary>
        private protected double Frequency;

        private protected ADSR envelope;

        /// <summary>
        /// 
        /// </summary>
        private protected short amp = 7000;

        /// <summary>
        /// 
        /// </summary>
        private protected double Velocity = 1;

        private protected double w = 0;

        #endregion


        #region Public methods

        public Generator()
        {
            dic = new Dictionary<WaveType, Out>()
            {
                {WaveType.Sine, Sine},
                {WaveType.Saw, Saw},
                {WaveType.Square, Square},
                {WaveType.Triangle, Triangle},
                {WaveType.Noise, Noise}
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WaveType"></param>
        /// <returns></returns>
        public void SetWave(WaveType waveTypeIn)
        {

            waveType = waveTypeIn;
        }

        public virtual void SetADSR(ADSR env)
        {
            envelope = env;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Freq"></param>
        /// <returns></returns>
        public void SetFreq(double Freq)
        {
            Frequency = Freq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Amp"></param>
        /// <returns></returns>
        public void Setamp(ref short Amp)
        {
            amp = Amp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetOut()
        {
            double output = dic[waveType]()*envelope.Process() * Velocity;
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Velocity"></param>
        /// <returns></returns>
        public void SetVelocity(double Vel)
        {
            Velocity = Vel;
        }

        public void EnvelopeNoteOff(int id)
        {
            envelope.KeyOff(id);
        }
        #endregion


        #region Protected methods

        #endregion


        #region Private methods
        private protected double Sine()
        {
            if (w > 2 * Math.PI)
                w -= 2 * Math.PI;

            w += 2 * Math.PI * Frequency / 44100;

            return (amp * Math.Sin(w));
        }

        private protected double Saw()
        {
            if (w > 2 * Math.PI)
                w -= 2 * Math.PI;

            w += 2 * Math.PI * Frequency / 44100;

            return (2 * amp * Math.Asin(Math.Sin(w)) / Math.PI);
        }

        private protected double Square()
        {
            if (w > 2 * Math.PI)
                w -= 2 * Math.PI;

            w += 2 * Math.PI * Frequency / 44100;

            return (amp * Math.Sign(Math.Sin(w)));
        }

        private protected double Triangle()
        {
            if (w > 2 * Math.PI)
                w -= 2 * Math.PI;

            w += 2 * Math.PI * Frequency / 44100;

            return (2 * amp * (Math.PI / (2 - Math.Atan(Math.Tan(w)))) / Math.PI);
        }

        private protected double Noise()
        {
            if (w > 2 * Math.PI)
                w -= 2 * Math.PI;

            w += 2 * Math.PI * Frequency / 44100;

            return (amp * (short)new Random().Next());
        }
        #endregion
    }
}
