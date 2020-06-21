using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* Win32 Api */
using System.Runtime.InteropServices;
using WindowsFormsApp1;
/* --------- */

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private sbyte windowExist = -1,
            fullscreenDisable = -1;
        private bool windowMinimize = false;

        private String windowName = "屏幕广播"/*"Window On Top"*/;
        private IntPtr windowhWnd, buttonhWnd;
        AboutBox1 about = new AboutBox1();

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "GetWindow")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("User32.dll", EntryPoint = "EnableWindow")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex); //获得所属句柄窗体的样式函数
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern bool ShowWindow(IntPtr hWnd,int nCmdShow);

        private void log(string content)
        {
            logging.Text += DateTime.Now.ToString("[MM-dd hh:mm:ss] ") + content + Environment.NewLine;
            logging.SelectionStart = logging.Text.Length;
            logging.ScrollToCaret();
        }

        private void findButton()
        {
            buttonhWnd = windowhWnd;
            
            buttonhWnd = GetWindow(buttonhWnd, 5);      //GW_CHILD
            buttonhWnd = GetWindow(buttonhWnd, 5);
            buttonhWnd = GetWindow(buttonhWnd, 2);      //GW_HWNDNEXT
            buttonhWnd = GetWindow(buttonhWnd, 2);
            buttonhWnd = GetWindow(buttonhWnd, 2);
            buttonhWnd = GetWindow(buttonhWnd, 2);
            buttonhWnd = GetWindow(buttonhWnd, 2);
            buttonhWnd = GetWindow(buttonhWnd, 2);
            

            /* Testing */
            //buttonhWnd = GetWindow(buttonhWnd, 5);      //GW_CHILD
            //buttonhWnd = GetWindow(buttonhWnd, 2);      //GW_HWNDNEXT
            /* Testing */
        }

        public Form1()
        {
            InitializeComponent();
            log("开始运行");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           linkLabel1.Visible =  timer2.Enabled = checkBox1.Checked;
        }

        private void timerMini_Tick(object sender, EventArgs e)
        {
            ShowWindow(windowhWnd, 2);      //SW_SHOWMINIMIZED
        }

        private void button2_Click(object sender, EventArgs e)
        {
            windowMinimize = timerMini.Enabled = !windowMinimize;
            button2.Text = (windowMinimize ? "取消" : "") + "窗口最小化";
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    about.ShowDialog();
                    break;
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                this.TopMost = false;
            }
        }

        private Point prevPos;
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(linkLabel1.Text == "还原")
            {
                this.MinimizeBox = true;
                this.Size = new Size(335, 504);
                this.Location = prevPos;
                linkLabel1.Text = "缩小";
                linkLabel1.Location = new Point(95, 64);
            }
            else
            {
                this.MinimizeBox = false;
                this.Size = new Size(20, 70);
                //this.StartPosition = CenterToScreen;
                linkLabel1.Text = "还原";
                prevPos = this.Location;
                this.Location = new Point(0, 0);
                linkLabel1.Location = new Point(0, 0);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            about.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            log(string.Format("执行{0}全屏按钮", fullscreenDisable == 1 ? "解锁" : "禁用"));
            EnableWindow(buttonhWnd, fullscreenDisable == 1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            windowhWnd = FindWindow(null, windowName);
            if (windowhWnd == IntPtr.Zero && windowExist != 0)
            {
                log("广播窗口关闭");
                label2.Text = "未广播";
                label4.Text = "（不可用）";
                button1.Enabled = false;
                windowExist = 0;
                checkBox1.Checked = false;
            }
            if (windowhWnd != IntPtr.Zero && windowExist != 1)
            {
                log("广播窗口打开");
                label2.Text = "已广播";
                button1.Enabled = true;
                windowExist = 1;
                checkBox1.Checked = true;
                this.WindowState = FormWindowState.Normal;
            }
            if (windowExist == 1)
            {
                findButton();

                int temp = GetWindowLong(buttonhWnd, -16);  //GWL_STYLE
                if ((temp & 0x08000000L) != 0)              //WS_DISABLED 
                {
                    if (fullscreenDisable != 1)
                    {
                        fullscreenDisable = 1;
                        label4.Text = "已禁用";
                        button1.Text = "解锁全屏按钮";
                        log("全屏按钮变为禁用状态");
                    }
                }
                else
                {
                    if (fullscreenDisable != 0)
                    {
                        fullscreenDisable = 0;
                        label4.Text = "未禁用";
                        button1.Text = "禁用全屏按钮";
                        log("全屏按钮变为解锁状态");
                    }
                }
            }

            // log(windowhWnd.ToString() + "->" + buttonhWnd.ToString());
        }
    }
}
