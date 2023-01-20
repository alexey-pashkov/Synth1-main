using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Midi;

namespace Synth_1
{
    public class MIDI
    {
        MidiIn midiIn;

        public Dictionary<int, string> GetDevices()
        {
            Dictionary<int, string> devs = new Dictionary<int, string>();
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
                devs.Add(i, MidiIn.DeviceInfo(i).ProductName);
            return devs;
        }

        public void InvokeMidiIn(int devNum, EventHandler<MidiInMessageEventArgs> handler)
        {
            midiIn = new MidiIn(devNum);
            midiIn.MessageReceived += handler;
            midiIn.Start();
        }

        public (double, int, double) ParseMessage(int raw_msg)
        {
            int data1 = (raw_msg >> 8) & 0xFF;
            int data2 = (raw_msg >> 16) & 0xFF;
            double d = (data1 - 69) / 12.0d;
            double freq = 440 * Math.Pow(2, d);
            double vel = data2 / 127.0d;
            if (vel > 1)
            {
                vel = 1;
            }
            return (freq, data1, vel);
        }
    }
}
