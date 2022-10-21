//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Automation.BDaq;

//namespace jf_FinalProject.Logic
//{
//    class HardwareInterface
//    {
//        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
//        private WaveformAiCtrl waveform = new WaveformAiCtrl();
//        private int sampleCounter = 0;
//        List<double> ePockSample = new List<double>();
//        string device = "USB-4711A,BID#0";

//        InstantAiCtrl instantAi = new InstantAiCtrl();            
//        public double ReadAnalog(int channel)
//        {
//            instantAi.SelectedDevice = new DeviceInformation(device);
//            double data;
//            instantAi.Read(channel, out data);
//            //instantAi.LoadProfile(@"E:\jf\FinalProject\BdaqProfile\profile.xml");
//            return data;
//        }
//        public void writeAnalog(int channel, double data)
//        {
//            InstantAoCtrl instanAo = new InstantAoCtrl();
//            instanAo.SelectedDevice = new DeviceInformation(device);
//            instanAo.Write(channel, data);
//        }
//        public void readWaveform()
//        {
//            waveform.SelectedDevice = new DeviceInformation(device);
//            watch.Start();
//            waveform.Start();
//            instantAi.LoadProfile(@"E:\jf\FinalProject\BdaqProfile\profile.xml");
//        }
//        private void WaveHandler(object sender, BfdAiEventArgs e)
//        {
//            if (this.sampleCounter != 10)
//            {
//                double[] temp = new double[e.Count];
//                waveform.GetData(e.Count, temp);
//                ePockSample.AddRange(temp);
//                this.sampleCounter++;
//            }
//            else
//            {
//                watch.Stop();
//                waveform.Stop();
//                double totalTime = watch.Elapsed.TotalMilliseconds;
//                this.sampleCounter = 0;
//                watch.Reset();
//            }

//        }
//        public void writeDigital(byte data)
//        {
//            InstantDoCtrl digital = new InstantDoCtrl();
//            digital.SelectedDevice = new DeviceInformation(device);

//            digital.Write(0,data);

//        }
//        private double CalculateSpeed(double time)
//        {
//            int counter = 0;
//            bool flag = false;
//            for (int i = 0; i < this.ePockSample.Count; i++)
//            {
//                if (ePockSample[i]>2)
//                {
//                    if (!flag)
//                    {
//                        flag = true;
//                    }
//                }
//                else
//                {
//                    if (flag)
//                    {
//                        counter++;
//                        flag = false;
//                    }
//                }
//            }
//            return (counter / time);
//        }
//    }
    
//}
