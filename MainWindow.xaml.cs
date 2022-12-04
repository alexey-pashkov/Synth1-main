using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Synth_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int BUF_SIZE = 44100;
        ReadMidi midi = new ReadMidi();
        static Synthezator[] synths = new Synthezator[128];
        static Generator g1;
        static Generator g2;
        static WaveOut wo = new WaveOut();


        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer1_Tick;
            timer.Start();
            Devices.ItemsSource = midi.SelectDevice().Select(x => x.Value);
            g1 = new Generator();
            g2 = new Generator();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Devices_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (Devices.SelectedIndex >= 0)
                midi.InvokeMidiIn(Devices.SelectedIndex);
            else
                midi.InvokeMidiIn(0);
        }

        public static void NoteOn(double freq, int index)
        {
            if (synths[index] == null)
            {
                g1.SetFreq(freq);
                g1.SetAmplitude(7000);
                Synthezator s = new Synthezator();
                s.AddCarrier(g1);
                //s.AddCarrier(g2);
                synths[index] = s;
            }
        }

        public static void NoteOff(double freq, int index)
        {
            if (synths[index] != null)
                synths[index] = null;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            byte[] buf = new byte[BUF_SIZE];

            for (int i = 0; i < BUF_SIZE; i++)
            {   
                short v = 0;
                for (int j = 0; j < synths.Length; j++)
                {
                    if (synths[j] != null)
                        v = Mix(v,  synths[j].GetOut());
                }             
            
                buf[i++] = (byte)(v & 0xFF);
                buf[i] = (byte)(v >> 8);
            }

            IWaveProvider provider = new RawSourceWaveStream(new MemoryStream(buf), new WaveFormat(44100, 16, 1));
            wo.Init(provider); 
            wo.Play();
            // MixingSampleProvider mix = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1));
            // for (int i=0; i < synths.Length; i++) 
            // {
            //     if (synths[i] != null) 
            //     {
            //         byte[] b = FillBuffer(synths[i]);
            //         mix.AddMixerInput(new RawSourceWaveStream(new MemoryStream(b), new WaveFormat(44100, 16, 1)));
                    
            //     }
            // }

            // wo.Init(mix);
            // wo.Play();
        }

        public static short Mix(short v1, short v2)
        {
            int v = v1 + v2;
            // if (v1 != 0 && v2 != 0)
            //     v = v / 2;
            if (v > short.MaxValue)
                v = short.MaxValue;
            if (v < short.MinValue)
                v = short.MinValue;
            return (short)v;
        }

        private void Osc1Wave_DropDownClosed(object sender, EventArgs e)
        {
            WaveType wt;
            if (Osc1Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);
                g1.SetWave(wt);
            }
            else
            {
                Osc1Wave.SelectedIndex = 0;
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);
                g1.SetWave(wt);
            }

        }

        static byte[] FillBuffer(Synthezator s) 
        {
            byte[] buf = new byte[BUF_SIZE];
            for (int i = 0; i < BUF_SIZE; i++)
            {
                short v = s.GetOut();
                buf[i++] = (byte)(v & 0xFF);
                buf[i] = (byte)(v >> 8);
            }
            return buf;
        }


        private void Osc2Wave_DropDownClosed(object sender, EventArgs e)
        {
            WaveType wt;
            if (Osc2Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt);
                g2.SetWave(wt);
            }
            else
            {
                Osc2Wave.SelectedIndex = 0;
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt);
                g2.SetWave(wt);
            }
        }
    }
}
