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


        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);

        Boolean shift = false;
        Boolean first2 = true;
        Boolean mouse = false;
        Boolean windows = false;
        Boolean keyboard = false;
        Boolean first = true;
        Boolean first3 = true;
        Boolean programs = false;
        IntPtr hhook;

        public Form1()
        {
     
            InitializeComponent();

           
        }


       
 

    
      
    private void Form1_Load_1(object sender, EventArgs e)
    {
            

            

    
        gHook = new GlobalKeyboardHook(); // Create a new GlobalKeyboardHook
                                              // Declare a KeyDown Event
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);

            // Add the keys you want to hook to the HookedKeys list
  

            foreach (Keys key in Enum.GetValues(typeof(Keys))) { 
        
            gHook.HookedKeys.Add(key);
        }
        }

      
    

    // Handle the KeyDown Event
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


        private void appendText( String s)
        {
            richTextBox1.AppendText(s+ "\t Time : " + string.Format("{0:HH:mm:ss tt}", DateTime.Now) + "\t" + DateTime.Today.ToString("d") + "\n");
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (keyboard)
            {
                keyboard = false;
                gHook.unhook();
                button1.BackColor = Color.Red; 
            } else
            {
                keyboard = true;
                gHook.hook();
                button1.BackColor = Color.Green;
            }
            
            
        }

     
        private void button3_Click(object sender, EventArgs e)
        {

            if (mouse)
            {
                MouseHook.stop();
                mouse = false;
                button3.BackColor = Color.Red;
            } else
            {
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

            appendText2(" ------  All Running Programs ------");
            // active programs
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                 
                    appendText2("Name: " + p.MainWindowTitle + "\t\t StartTime: " + p.StartTime + "\t ProcessorTime: " + p.TotalProcessorTime + "\t VMsize: " + p.VirtualMemorySize + "\t PMsize: " + p.WorkingSet);
                }
            }


            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // agents
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            appendText2(" ------  All Services ------");

            // try to find service name
            foreach (ServiceController service in services)
            {
                appendText2("Name: " + service.DisplayName + " Type: " + service.ServiceType + " Status: " +service.Status + " Depend: " + service.ServicesDependedOn);
                
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
                appendText2("-----  Windows hook Ended ------");
            }
            else
            {
                windows = true;
                appendText2("----- Starting Windows hook ------");
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
            appendText2(sender.ToString());
            
        }

        public static event EventHandler WindowsAction = delegate { };

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
           IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {

            WindowsAction("Event: " +eventType + " Thread: " + dwEventThread.ToString() + " HWND: " + hwnd.ToString() + " OBJECT: " + idObject.ToString() + " CHILD: " + idChild.ToString() + " TIME: " + dwmsEventTime.ToString(), new EventArgs());
            
        }

    
        private void appendText2(String s)
        {
            richTextBox2.AppendText(s +"\t" + "\n");

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
     
            richTextBox2.ScrollToCaret();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // agents
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            appendText2(" ------  All None Stopped Services ------");

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.Status != ServiceControllerStatus.Stopped) {
                appendText2("Name: " + service.DisplayName + "\t\t Type: " + service.ServiceType + "\t Status: " + service.Status + "\t ServiceName: " + service.ServiceName);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
                if (programs)
            {
                MouseHook.stop();
                programs = false;
                button7.BackColor = Color.Red;
            }
            else
            {
                programs = true;
                button7.BackColor = Color.Green;
                MouseHook.Start();
                if (first3)
                {
                    first3 = false;
                    WatchPrograms();
                }
           


            }
        }

        public void ProcessStartEventArrived(object sender, EventArrivedEventArgs e)
        {
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                //appendText2(pd.Name + " " + pd.Type + " " + pd.Value);
                Console.WriteLine(pd.Name + " " + pd.Type + " " + pd.Value);
                pd.
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
    }



}

