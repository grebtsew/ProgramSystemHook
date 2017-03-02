using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ProgramUsage
{
    public class SaveLog
    {

        string foldername = "data";
        bool r = false;
        int sleepTimer = 120000;

        public SaveLog()
        {
          
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

        public void clearDayLog()
        {
            try {
                System.IO.File.WriteAllText(getFilePath(), string.Empty);
            } catch
            {
                // no file created yet
            }
        }

        public void setSleepTime(int i)
        {
            sleepTimer = i * 1000;
        }

        private void run()
        {
            while (r)
            {
                save();

                Form1.main.clearText();
                Form1.main.appendText("----- Logdata saved to log file successful ------");

                resetTimer();
                Thread.Sleep(sleepTimer);
            }
        }

        private void resetTimer()
        {
            Form1.main.ChangeLabel2((sleepTimer / 1000).ToString());
        }

        private string getFileName()
        {
            return DateTime.Today.ToString("d") + ".txt";
        }

        private string getFilePath()
        {
            return getFileLocation() + "\\" +foldername + "\\" + getFileName();
        }

        private string getFolderPath()
        {
            return getFileLocation() + "\\" + foldername + "\\";
        }

        private void save()
        {
            if (!Directory.Exists(getFolderPath()))
            {
                Form1.main.appendText("----- Directory \"data\" added ------");
                Directory.CreateDirectory(getFolderPath());
            }

            if (!File.Exists(getFilePath()))
            {
                Form1.main.appendText("----- File \"" +  getFileName() +  "\" added ------");
                File.Create(getFileName());
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(getFilePath()))
            {
                String[] split = Form1.main.getRichText().Split('\n');
                foreach(string s in split) sw.WriteLine(s);

            }
            Form1.main.appendText("----- File \"" + getFileName() + "\" Successfully saved! ------");
            
        }

        private String getFileLocation()
        {
           return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); 
        }

}
}
