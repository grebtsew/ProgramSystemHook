using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ProgramUsage
{

    public class ProgramCheck
    {
        List<Process> processes = new List<Process>();
        List<Process> temp = new List<Process>();
        List<Process> remove = new List<Process>();

        int sleepTime = 5000;
        bool first = true;
        bool r = true;
        bool changed;
        bool found;

        public ProgramCheck(Form1 _form)
        {
            processes.AddRange(getProcesslist());            
        }

        public void setSleeptime(int _sleepTime)
        {
            sleepTime = _sleepTime;
        }

        private void print(Process p, bool t)
        {
            if (!String.IsNullOrEmpty(p.MainWindowTitle))
            {
                if (t)
                {
                     Form1.main.appendText("Program Start Detected : "+ "Name: " + p.MainWindowTitle + "\t\t ProcessName : " + p.ProcessName + "\t StartTime: " + p.StartTime + "\t ProcessorTime: " + p.TotalProcessorTime );
                }
                else
                {
                    Form1.main.appendText("Program Stop Detected : " + "Name: " + p.ToString());
                }
            }
        }

        public void start()
        {
            if (first) {
                first = false;
                foreach (Process p in Process.GetProcesses())
                {
                    if (!String.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        print(p, true);
                    }
                }

            } else
            {
                doChecks();
            }
          
            

            r = true;
            run();
        }

        private void showArray(List<Process> l)
        {
            foreach (Process p in l)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    print(p, true);
                }
            }
        }

        private void consoleshowArray(List<Process> l)
        {
            foreach (Process p in l)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    Console.WriteLine(p);
                }
            }
        }

        private List<Process> getProcesslist()
        {
            List<Process> c = new List<Process>();
            foreach (Process p in Process.GetProcesses())
                {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    c.Add(p);
                }
            }
            return c;
        }

        public static bool IsRunning(Process process)
        {
            try { Process.GetProcessById(process.Id); }
            catch (InvalidOperationException) { return false; }
            catch (ArgumentException) { return false; }
            return true;
        }

        private void doChecks()
        {
            changed = false;
            temp.Clear();
            temp.AddRange(getProcesslist());
            
            remove.Clear();
            remove.AddRange(processes);


            // check processes closed
            foreach (Process p in processes)
            {
                if (!IsRunning(p)) {
                    print(p, false);
                    remove.Remove(p);
                    changed = true;
                }
            }

            // check  processes started
            foreach (Process p in temp)
            {    
                if (IsRunning(p)) {
                    found = true;
             
                    foreach (Process c in remove)
                    {
                        if (c.MainWindowTitle == p.MainWindowTitle)
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {                       
                        print(p, true);
                        changed = true;
                    }
                }
           }

            if (changed)
            {
                processes.Clear();
                processes.AddRange(temp); 
            }
        }

        public void stop()
        {
            r = false;
        }

        private void run()
        {
            while (r)
            {
                if (!Same_Check())
                {
                    
                    doChecks();
                }

                resetTimer();
                Thread.Sleep(sleepTime);
            }
        }

        private void resetTimer()
        {
            Form1.main.ChangeLabel((sleepTime / 1000).ToString());
        }

        private bool Same_Check()
        {
            List<Process> t = new List<Process>();
            t.AddRange(getProcesslist());
            return processes.Equals(t);
        }
    }
}
