using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramUsage
{
    public class PreformanceCheck
    {


        int sleepTime = 5000;
        bool r = true;

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        


        public PreformanceCheck()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

       

        public void setSleeptime(int _sleepTime)
        {
            sleepTime = _sleepTime;
        }

        private string getUsedMemory()
        {
            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
                FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
            }).FirstOrDefault();

            
            if (memoryValues != null)
            {
                return Math.Round(((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100).ToString() + " %";
            }
            return "";
        }

        public string getCurrentCpuUsage()
        {
            return Math.Round(cpuCounter.NextValue()) + "%";
        }

        public string getAvailableRAM()
        {
            return Math.Round(ramCounter.NextValue()) + "MB";
        }

        private void print()
        {
            Form1.main.appendText("PerfomanceCheck: "+"CPUuse: "+ getCurrentCpuUsage() + " MemoryUse: "+ getUsedMemory() +" Runtime: " +Form1.main.elapsedTime + " ");  
        }

        public void start()
        {
            r = true;
            run();
        }

    
        public void stop()
        {
            r = false;
        }

        private void run()
        {
            while (r)
            {

                    print();
                resetTimer();
                Thread.Sleep(sleepTime);
            }
        }

        private void resetTimer()
        {
            Form1.main.ChangeLabel((sleepTime / 1000).ToString());
        }

     
    }

}
