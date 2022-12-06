using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public class Synthezator
    {



        /// <summary>
        /// 
        /// </summary>
        private List<Generator> carriers = new List<Generator>();
        long time;

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns>double int</returns>
        public double GetOut()
        {
            double res = 0;
            time++;
            for (int i = 0; i < carriers.Count; i++)
            {
                res += carriers[i].GetOut();
            }
            res = (res / carriers.Count);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Synthesator()
        {
            time = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public void AddCarrier(Generator carrier)
        {
            carriers.Add(carrier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        public void SetFreq(double freq)
        {
            foreach (Generator gen in carriers)
            {
                gen.SetFreq(freq);
        
            }
        }


       


    }
} 