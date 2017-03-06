using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Windows;
using System.Threading;
using System.Management;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.IO;

namespace ProgramUsage
{
    public partial class Form1 : Form
    {
        GlobalKeyboardHook gHook;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        PreformanceCheck pcc;
       public TimeSpan elapsedTime;

        internal static Form1 main;
        SynchronizationContext ctx;

        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);

        Thread PreformanceThread;
        Thread SaveListenThread;
        Thread ProgramListenThread;
        public Label label;
        public Label l2;

        public static DateTime StartTime = DateTime.Now;

        bool createremove = false;
        bool first = true;
        bool first2 = true;
        bool save = false;
        bool mouse = false;
        bool windows = false;
        bool keyboard = false;
        bool programs = false;
         bool first3 = true;
        bool preformance = false;

        TextBox tb;

        public RichTextBox rtb;

        

        SaveLog sl;
        IntPtr hhook;
        ProgramCheck pc;

        
        public Form1()
        {
     
            InitializeComponent();
            trackBar1.Value = 5;
            trackBar2.Value = 120;
            main = this;
            rtb = richTextBox1;
            richTextBox1.ReadOnly = true;
            tb = textBox1;
            pcc = new PreformanceCheck();
            timer3.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gHook.unhook();
            MouseHook.stop();
            UnhookWinEvent(hhook);
            pc.stop();
        }

        
       

        private void Form1_Load_1(object sender, EventArgs e)
    {
            HttpListener hl = new HttpListener();

            sl = new SaveLog();
            label = label4;
            l2 = label8;
            ctx = SynchronizationContext.Current;
            pc = new ProgramCheck(this);

            gHook = new GlobalKeyboardHook(); // Create a new GlobalKeyboardHook
                                              // Declare a KeyDown Event
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);


            // Add the keys you want to hook to the HookedKeys list
            foreach (Keys key in Enum.GetValues(typeof(Keys))) { 
            gHook.HookedKeys.Add(key);
        }

        }

        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            // keyboard
         appendText("Keypress identified :\t" + e.KeyCode + " " + "\t Key number : " + e.KeyValue);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
         
            richTextBox1.ScrollToCaret();
        }

        delegate void AppendTextDelegate(string text);
        public void appendText( string s)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new AppendTextDelegate(this.appendText), new object[] { s  });
            }
            else
            {
                richTextBox1.Text += s + "\t Time : " + string.Format("{0:HH:mm:ss tt}", DateTime.Now) + "\t" + DateTime.Today.ToString("d") + Environment.NewLine;
            }
        }

        delegate void clearTextDelegate();
        public void clearText()
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new clearTextDelegate(this.clearText));
            }
            else
            {
                richTextBox1.Text = "";
            }
        }

        delegate string getRichTextDelegate();
        public string getRichText()
        {

            if (richTextBox1.InvokeRequired)
            {
            return richTextBox1.Invoke(new getRichTextDelegate(this.getRichText)).ToString();
            }
            else
            {
             return  richTextBox1.Text;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (keyboard)
            {
                appendText(" ----- KeyBoard Hook Closed -----");
                keyboard = false;
                gHook.unhook();
                button1.BackColor = Color.Red; 
            } else
            {
                appendText(" ----- KeyBoard Hook Active -----");
                keyboard = true;
                gHook.hook();
                button1.BackColor = Color.Green;
            }
        }
 
        private void button3_Click(object sender, EventArgs e)
        {
            if (mouse)
            {
                appendText(" ----- Mouse Hook Closed -----");
                MouseHook.stop();
                mouse = false;
                button3.BackColor = Color.Red;
            } else
            {
                appendText(" ----- Mouse Hook Active -----");
                mouse = true;

                MouseHook.Start();
                if (first) {
                    first = false;
                MouseHook.MouseAction += new EventHandler(Event);
                }
                button3.BackColor = Color.Green;
            }
        }

        private void Event(object sender, EventArgs e) {

            MouseHook.MSLLHOOKSTRUCT temp = (MouseHook.MSLLHOOKSTRUCT)sender;

            switch (temp.MouseAction)
            {
                case "WM_LBUTTONDOWN":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                case "WM_LBUTTONUP":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                case "WM_RBUTTONUP":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                case "WM_RBUTTONDOWN":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                case "WM_MOUSEWHEEL":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                case "WM_MOUSEMOVE":             
                    break;
                case "WM_MOUSEWHEELDOWN":
                    appendText("Mouse Event Detected : " + temp.MouseAction + "\t Position : " + temp.pt.x + " " + temp.pt.y);
                    break;
                default:
                    Console.WriteLine("Unknown mouse event!");
                    break;
            }
 
        }
      
        private void button4_Click(object sender, EventArgs e)
        {
            appendText(" ------  All Running Programs ------");
            // active programs
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    appendText("Name: " + p.MainWindowTitle + "\t\t StartTime: " + p.StartTime +"\t ProcessName : "+p.ProcessName+ "\t ProcessorTime: " + p.TotalProcessorTime + "\t VMsize: " + p.VirtualMemorySize + "\t PMsize: " + p.WorkingSet);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // agents
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            appendText(" ------  All Services ------");

            // try to find service name
            foreach (ServiceController service in services)
            {
                appendText("Name: " + service.DisplayName + " Type: " + service.ServiceType + " Status: " +service.Status + " Depend: " + service.ServicesDependedOn);
                
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // windows

            if (windows)
            {
                UnhookWinEvent(hhook);
                windows = false;
                button5.BackColor = Color.Red;
                appendText("-----  Windows hook Ended ------");
            }
            else
            {
                windows = true;
                appendText("----- Starting Windows hook ------");
                // Listen for name change changes across all processes/threads on current desktop...
                hhook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                        procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

                if (first2) {
                    first2 = false;
                WindowsAction += new EventHandler(updateWindowsText);
                }

                // MessageBox provides the necessary mesage loop that SetWinEventHook requires.
                // In real-world code, use a regular message loop (GetMessage/TranslateMessage/
                // DispatchMessage etc or equivalent.)

                button5.BackColor = Color.Green;

            }
        }

        private void updateWindowsText(object sender, EventArgs e)
        {
            appendText(sender.ToString());    
        }

        public static event EventHandler WindowsAction = delegate { };

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
           IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            WindowsAction("Windows Event detected : " + " Event: " +eventType + " Thread: " + dwEventThread.ToString() + " HWND: " + hwnd.ToString() + " OBJECT: " + idObject.ToString() + " CHILD: " + idChild.ToString() + " TIME: " + dwmsEventTime.ToString(), new EventArgs());  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // agents
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            appendText(" ------  All None Stopped Services ------");

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.Status != ServiceControllerStatus.Stopped) {
                appendText("Name: " + service.DisplayName + "\t\t Type: " + service.ServiceType + "\t Status: " + service.Status + "\t ServiceName: " + service.ServiceName);
                }
            }
        }

        public void ProcessStartEventArrived(object sender, EventArrivedEventArgs e)
        {
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                //appendText2(pd.Name + " " + pd.Type + " " + pd.Value);
                Console.WriteLine(pd.Name + " " + pd.Type + " " + pd.Value);
                
            }
        }

        private void WatchPrograms()
        {
            
            ManagementEventWatcher w = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent",
            new TimeSpan(0, 0, 1),
            "TargetInstance isa \"Win32_Process\"");
            try
            {

                w.Query = query;
              //  Console.WriteLine(query.QueryString);
                w.EventArrived += new EventArrivedEventHandler(this.ProcessStartEventArrived);
                w.Start();
                ManagementBaseObject l = w.WaitForNextEvent();
                while (programs)
                {
                    w.WaitForNextEvent();
                }

            }
            finally
            {
                w.Stop();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            pc.setSleeptime(trackBar1.Value * 1000);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
            if (programs)
            {
                appendText("----- Listen to program actvites stopped -----");
                pc.stop();
                timer1.Stop();
                ProgramListenThread.Abort();
                programs = false;
                button8.BackColor = Color.Red;
            }
            else
            {
                appendText("----- Listen to program actvites active -----");
                programs = true;
                button8.BackColor = Color.Green;
                ProgramListenThread = new Thread(new ThreadStart(pc.start));
                ProgramListenThread.IsBackground = true;
                timer1.Start();
                ProgramListenThread.Start();
                
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int i = Int32.Parse(label4.Text);
                label4.Text =  (i - 1).ToString();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        delegate void ChangeLabelDelegate(string text);

        public void ChangeLabel(String s)
        {
            if (label4.InvokeRequired)
            {
                label4.Invoke(new ChangeLabelDelegate(this.ChangeLabel), new object[] { s });
            }
            else
            {
                label4.Text = s;
            }
        }

        delegate void ChangeLabelDelegate2(string text);

        public void ChangeLabel2(String s)
        {
            if (l2.InvokeRequired)
            {
                l2.Invoke(new ChangeLabelDelegate2(this.ChangeLabel2), new object[] { s });
            }
            else
            {
                l2.Text = s;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (save)
            {
                appendText("----- Write Data to File Stopped -----");
                sl.stop();
                timer2.Stop();
                SaveListenThread.Abort();
                save = false;
                button9.BackColor = Color.Red;
            }
            else
            {
                appendText("----- Write Data to File Started -----");
                save = true;
                button9.BackColor = Color.Green;
                label8.Text = label5.Text;
                timer2.Start();

                SaveListenThread = new Thread(new ThreadStart(sl.start));
                SaveListenThread.IsBackground = true;
                SaveListenThread.Start();

            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label5.Text = trackBar2.Value.ToString();
            sl.setSleepTime(trackBar2.Value);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int i = Int32.Parse(l2.Text);
            l2.Text = (i - 1).ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            sl.clearDayLog();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void FileSysHook(Form1 f)
        {
            string args = main.tb.Text;

            // If a directory is not specified, exit program.
            if (string.IsNullOrEmpty(args))
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = args;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
          //  watcher.Filter = "*.txt";

            if (f.first3) {
                f.first3 = false;
            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            }
            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
         //   Console.WriteLine("Press \'q\' to quit the sample.");
          //  while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.

            if (Form1.main.createremove) Form1.main.appendText("Filechange detected: " + e.FullPath + " " + e.ChangeType);
           
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {

            // Specify what is done when a file is renamed.
           // Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
           if(Form1.main.createremove) Form1.main.appendText("Filechange detected:" + e.OldFullPath + " renamed to "+e.FullPath);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (createremove)
            {
                appendText("----- Listen to Create/Remove stopped -----");
                // stop

                createremove = false;
                button14.BackColor = Color.Red;
            }
            else
            {
                appendText("----- Listen to Create/Remove active -----");
                createremove = true;
                button14.BackColor = Color.Green;
                // start
                FileSysHook(this);

            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (preformance)
            {
                appendText("----- Listen to Preformance Check stopped -----");
                // stop
                PreformanceThread.Abort();
                timer1.Stop();
               

                preformance = false;
                button18.BackColor = Color.Red;
            }
            else
            {
                appendText("----- Listen to Preformance Check active -----");
                preformance = true;
                button18.BackColor = Color.Green;
                // start
                PreformanceThread = new Thread(new ThreadStart(pcc.start));
                PreformanceThread.IsBackground = true;
                timer1.Start();
                PreformanceThread.Start();

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void timer3_Tick(object sender, EventArgs e)
        {
            elapsedTime = DateTime.Now - StartTime;
          
        }
    }

}

