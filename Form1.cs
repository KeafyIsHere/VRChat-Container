using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRChatContainer
{
    public partial class HentaiWare : Form
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);
        public HentaiWare()
        {
            InitializeComponent();
            try
            {
                process = new Process();
                process.StartInfo.FileName = "VRChat.exe";
                process.StartInfo.Arguments = "--no-vr -parentHWND " + panel1.Handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();
                EnumChildWindows(panel1.Handle, WindowEnum, IntPtr.Zero); 
                ReSizePannel();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to UnityGame.exe.");
            }
        }
        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void HentaiWare_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void HentaiWare_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        private void HentaiWare_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (!process.HasExited)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void HentaiWare_ClientSizeChanged(object sender, EventArgs e)
        {
            ReSizePannel();
        }

        //Need to fix
        private void ReSizePannel()
        {
            int hHeight = Height / 27;
            int wWidth = Width / 16;
            MoveWindow(unityHWND, 0, 0, Width + wWidth, Height - hHeight, true);
            ActivateUnityWindow();
            panel1.Width = Width + wWidth;
            panel1.Height = Height - hHeight;
        }
    }
}
