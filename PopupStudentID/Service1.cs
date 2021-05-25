using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace PopupStudentID
{
    public partial class Service1 : ServiceBase
    {
        public object Constants { get; private set; }

        public Service1()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            SessionChangeDescription sschange = new SessionChangeDescription();
            OnSessionChange(sschange);
        }

        protected override void OnStop()
        {
        }
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Thread.Sleep(3000);
            try
            {
                base.OnSessionChange(changeDescription);
                if ((changeDescription.Reason == SessionChangeReason.SessionUnlock) || (changeDescription.Reason == SessionChangeReason.SessionLogon))
                {
                    WriteToFile("Logged");
                    ShowMessageBox();
                }
                else
                {
                    WriteToFile("Can't log");
                }
            }
            catch (Exception exp)
            {
                //ShowMessageBox();
                WriteToFile(exp.ToString());
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WTSGetActiveConsoleSessionID();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(
                IntPtr hServer,
                [MarshalAs(UnmanagedType.I4)] int SessionId,
                String pTitle,
                [MarshalAs(UnmanagedType.U4)] int TitleLength,
                String pMessage,
                [MarshalAs(UnmanagedType.U4)] int MessageLength,
                [MarshalAs(UnmanagedType.U4)] int Style,
                [MarshalAs(UnmanagedType.U4)] int Timeout,
                [MarshalAs(UnmanagedType.U4)] out int pResponse,
                bool bWait);
        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1;
        public void ShowMessageBox()
        {
            try
            {
                WriteToFile("18520175");
                bool result = false;
                String title = "MSSV";
                int tlen = title.Length;
                string msg = "18520175";
                int mlen = msg.Length;
                int resp = 0;
                result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, 1, title, tlen, msg, mlen, 0, 0, out resp, true);
                WriteToFile(result.ToString());
                int err = Marshal.GetLastWin32Error();
                WriteToFile("Test Show Message");
            }
            catch (Exception ex)
            {
                // Debug.WriteLine("no such thread exists", ex);
                WriteToFile(ex.ToString());
            }
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
