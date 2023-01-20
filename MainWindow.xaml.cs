using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using NAudio.Midi;
using NAudio.Wave.SampleProviders;

namespace Synth_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int BUF_SIZE = 44100;
        MIDI midi_handler;
        static Synthesator[] synths = new Synthesator[128];
        static WaveType wt;
        static WaveType wt2;
        bool chck1 = false;
        bool chck2 = false;
        double mf = 1;
        short amp1 = 8000;
        short amp2 = 8000;
        double[] adsr1 = new double[4];
        double[] adsr2 = new double[4];
        DirectSoundOut waveout;
        IWaveProvider provider;
        WaveFormat format;
        static int keysCount = 0;
        List<string> presets = new List<string>();


        public MainWindow()
        {
            midi_handler = new MIDI();
            InitializeComponent();
            format = new WaveFormat(44100, 16, 1);           
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
             timer.Tick += Timer1_Tick;
            timer.Start();
            Devices.ItemsSource = midi_handler.GetDevices().Select(x => x.Value);
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (Directory.Exists(path))
            {
                presets = Directory.GetFiles(path).ToList();
                List<string> temp = new List<string>();
                foreach (string preset in presets)
                {
                    temp.Add(preset.Remove(0, preset.LastIndexOf("\\") + 1));
                }
                Presets.ItemsSource = temp;
            }
            waveout = new DirectSoundOut();
           
            adsr1[0] = a1.Value;
            adsr1[1] = d1.Value;
            adsr1[2] = s1.Value;
            adsr1[3] = r1.Value;
           
            adsr2[0] = a2.Value;
            adsr2[1] = d2.Value;
            adsr2[2] = s2.Value;
            adsr2[3] = r2.Value;
        }


        private void MessageRecived(object sender, MidiInMessageEventArgs e) 
        {

            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                (double f, int d, double v) = midi_handler.ParseMessage(e.RawMessage); 
                NoteOn(f, d, v);
            }
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                (double f, int d, double v) = midi_handler.ParseMessage(e.RawMessage);
                NoteOff(d);
            }
        }

        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SaveDialog sd = new SaveDialog();
            if (sd.ShowDialog() == true)
            {
                if (!File.Exists(path + "\\" + sd.PresetName + ".txt"))
                {
                    using (File.Create(path + "\\" + sd.PresetName + ".txt"));
                }
                    if (!chck1 && !chck2)
                    {
                        string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value) + '\n';
                        string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value) + '\n';
                        File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os1);
                        File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os2);
                    }
                    else
                    {
                        if (chck1)
                        {
                            string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value, Ratio.Value) + '\n';
                            string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value) + '\n';
                            File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os1);
                            File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os2);
                        }
                        else
                        {
                            string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value) + '\n';
                            string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value, Ratio.Value) + '\n';
                            File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os1);
                            File.AppendAllText(path + "\\" + sd.PresetName + ".txt", os2);
                        }
                    }
            }
            
        }

        private void Devices_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (Devices.SelectedIndex >= 0)
                midi_handler.InvokeMidiIn(Devices.SelectedIndex, MessageRecived);
            else
                midi_handler.InvokeMidiIn(0, MessageRecived);
        }

        private void NoteOn(double freq, int index, double vel)
        {
            if (synths[index] == null)
            {
                Synthesator s = new Synthesator(freq);
                if (!chck1 && !chck2)
                {
                    Generator g1 = new Generator();
                    g1.Setamp(ref amp1);
                    g1.SetWave(wt);
                    g1.SetVelocity(vel);
                    g1.SetADSR(new ADSR(adsr1[0], adsr1[1], adsr1[2], adsr1[3]));
                    Generator g2 = new Generator();
                    g2.Setamp(ref amp2);
                    g2.SetWave(wt2);
                    g2.SetVelocity(vel);
                    g2.SetADSR(new ADSR(adsr2[0], adsr2[1], adsr2[2], adsr2[3]));
                    s.AddCarrier(g1);
                    s.AddCarrier(g2);
                }
                else
                {
                    if (chck2)
                    {
                        Carrier c1 = new Carrier();
                        c1.Setamp(ref amp2);
                        c1.SetWave(wt);
                        c1.SetADSR(new ADSR(adsr2[0], adsr2[1], adsr2[2], adsr2[3]));
                        Modulator m1 = new Modulator();
                        m1.SetWave(wt2);
                        m1.SetFreq(freq);
                        m1.SetRatio(ref mf);
                        m1.SetADSR(new ADSR(adsr1[0], adsr1[1], adsr1[2], adsr1[3]));
                        c1.SetModulator(m1);

                        s.AddCarrier(c1);
                    }

                    else
                    {
                        Carrier c1 = new Carrier();
                        c1.Setamp(ref amp1);
                        c1.SetWave(wt2);
                        c1.SetADSR(new ADSR(adsr1[0], adsr1[1], adsr1[2], adsr1[3]));
                        Modulator m1 = new Modulator();
                        m1.SetWave(wt);
                        m1.SetFreq(freq);
                        m1.SetRatio(ref mf);
                        m1.SetADSR(new ADSR(adsr2[0], adsr2[1], adsr2[2], adsr2[3]));
                        c1.SetModulator(m1);
                        s.AddCarrier(c1);
                       
                    }
                }
                synths[index] = s;
                keysCount++;
            }
        }

        private void NoteOff(int index)
        {
            if (synths[index] != null)
            {
                synths[index].CallNoteOff(index);
            }  
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (keysCount != 0)
            {
                byte[] buf = new byte[BUF_SIZE*2];
                for (int i = 0; i < buf.Length/2; i++)
                {
                    short v = 0;
                    for (int j = 0; j < synths.Length; j++)
                    {
                        if (synths[j] != null)
                            v = Mix(v, synths[j].GetOut());
                    }
                    buf[i*2] = (byte)(v & 0xFF);
                    buf[i*2+1] = (byte)(v >> 8);
                }
                MemoryStream buff_stream = new MemoryStream(buf);
                provider = new RawSourceWaveStream(buff_stream, format);
                waveout.Init(provider);
                if (provider != null)
                    waveout.Play();
            }
        }

        private short Mix(double v1, double v2)
        {
            double v = (v1 + v2) / 2;
            if (v > short.MaxValue)
                v = short.MaxValue;
            if (v < short.MinValue)
                v = short.MinValue;
            return (short)v;
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            chck1 = true;
            checkBox2.IsEnabled = false;
        }

        private void CheckBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            chck1 = false;
            checkBox2.IsEnabled = true;
        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            chck2 = true;
            checkBox1.IsEnabled = false;
        }

        private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            chck2 = false;
            checkBox1.IsEnabled = true;
        }


        private string SetStr(string w, double a, double d, double s, double r, double v)
        {
            return w + " " + false + " " + a + " " + d + " " + s + " " + r + " " + v;
        }

        private string SetStr(string w, double a, double d, double s, double r, double v, double rt)
        {
            return w + " " + true + " " + a + " " + d + " " + s + " " + r + " " + v + " " + rt;
        }

        private void Presets_DropDownClosed(object sender, EventArgs e)
        {
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (Directory.Exists(path))
            {
               if (File.Exists(path + "\\" + Presets.Text.ToString()))
               {
                    string[] temp = File.ReadAllLines(path + "\\" + Presets.Text.ToString());
                    string[] st1 = temp[0].Split(" ");
                    string[] st2 = temp[1].Split(" ");

                    Osc1Wave.Text = st1[0];
                    Osc2Wave.Text = st2[0];
                    Enum.TryParse(st1[0], out wt);
                    Enum.TryParse(st2[0], out wt2);
                    checkBox1.IsChecked = chck1 = Convert.ToBoolean(st1[1]);
                    checkBox2.IsChecked = chck2 = Convert.ToBoolean(st2[1]);
                    a1.Value = Convert.ToDouble(st1[2]);
                    a2.Value = Convert.ToDouble(st2[2]);
                    d1.Value = Convert.ToDouble(st1[3]);
                    d2.Value = Convert.ToDouble(st2[3]);
                    s1.Value = Convert.ToDouble(st1[4]);
                    s2.Value = Convert.ToDouble(st2[4]);
                    r1.Value = Convert.ToDouble(st1[5]);
                    r2.Value = Convert.ToDouble(st2[5]);
                    Osc1V.Value = Convert.ToDouble(st1[6]);
                    Osc2V.Value = Convert.ToDouble(st2[6]);



                    if (Convert.ToBoolean(st1[1]))
                    {
                        mf = Ratio.Value = Convert.ToDouble(st1[7]);
                    }

                    if (Convert.ToBoolean(st2[1]))
                    {
                        mf = Ratio.Value = Convert.ToDouble(st2[7]);
                    }
                }
            }

        }

        static public void DeleteSynth(int index)
        {
            if (synths[index] != null)
            {
                synths[index] = null;
                keysCount--;
            }
        }

        private void Osc1Wave_DropDownClosed(object sender, EventArgs e)
        {
            
            if (Osc1Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);
            }
            else
            {
                Osc1Wave.SelectedIndex = 0;
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);

            }
        }

        private void Osc2Wave_DropDownClosed(object sender, EventArgs e)
        {
           
            if (Osc2Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt2);
            }
            else
            {
                Osc2Wave.SelectedIndex = 0;
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt2);
            }
        }

        private void Ratio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mf = Ratio.Value;
        }

        private void Atk1_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr1[0] = a1.Value;
        }
        private void Dc1_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr1[1] = d1.Value;
        }
        private void Sus1_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr1[2] = s1.Value;
        }
        private void Rls1_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr1[3] = r1.Value;
        }

        private void Atk2_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr2[0] = a2.Value;
        }
        private void Dc2_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr2[1] = d2.Value;
        }
        private void Sus2_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr2[2] = s2.Value;
        }
        private void Rls2_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            adsr2[3] = r2.Value;
        }

        private void Osc1V_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amp1 = (short)Osc1V.Value;
        }
        private void Osc2V_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amp2 = (short)Osc2V.Value;
        }
        private void MasterV_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double temp = 0;
            if (chck2)
            {
                temp = Osc1V.Value;
                Osc1V.Maximum = MasterV.Value;
                if (temp > Osc1V.Maximum)
                    Osc1V.Value = Osc1V.Maximum;
                temp = Osc2V.Value;
                Osc2V.Maximum = MasterV.Value;
                if (temp > MasterV.Value)
                    Osc2V.Value = Osc2V.Maximum;
            }
            else if (chck1)
            {
                temp = Osc1V.Value;
                Osc1V.Maximum = MasterV.Value;
                if (temp > Osc1V.Maximum)
                    Osc1V.Value = Osc1V.Maximum;
                temp = Osc2V.Value; 
                Osc2V.Maximum = MasterV.Value;
                if (temp > Osc2V.Maximum)
                    Osc2V.Value = Osc1V.Maximum;
            }
            else
            {
                temp = Osc1V.Value;
                Osc1V.Maximum = MasterV.Value / 2;
                if (temp > Osc1V.Maximum)
                    Osc1V.Value = Osc1V.Maximum;
                temp = Osc2V.Value;
                Osc2V.Maximum = MasterV.Value / 2;
                if (temp > Osc2V.Maximum)
                    Osc2V.Value = Osc2V.Maximum;
            }
        }
    }
}
