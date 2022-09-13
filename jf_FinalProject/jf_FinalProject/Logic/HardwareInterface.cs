using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;

namespace jf_FinalProject.Logic
{
    class HardwareInterface
    {
        InstantAiCtrl instantAi = new InstantAiCtrl();            
        public void starter(int channel)
        {
            instantAi.SelectedDevice = new DeviceInformation(0);
            double data;
            instantAi.Read(channel, out data);
            instantAi.LoadProfile(@"E:\jf\FinalProject\BdaqProfile\profile.xml");
        }
    }
    
}
