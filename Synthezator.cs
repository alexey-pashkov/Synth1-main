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
        /// <returns>short int</returns>
        public short GetOut()
        {
            short res = 0;
            time++;
            for (int i = 0; i < carriers.Count; i++)
            {
                res += carriers[i].GetOut();
            }
            res = (short)(res / (short)carriers.Count);
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




       


    }
} 