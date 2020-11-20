using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using System.Runtime.InteropServices;

namespace MiniBrower
{
    public partial class frmBrower : Form
    {
        private ChromiumWebBrowser browser;
        private clsGlobalKeyboardHook gHook;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        public frmBrower()
        {
            InitializeComponent();

            browser = new ChromiumWebBrowser("www.google.co.jp");
            this.Controls.Add(browser);

            browser.Dock = DockStyle.Fill;

            browser.FrameLoadEnd += browser_FrameLoadEnd;

            gHook = new clsGlobalKeyboardHook();
            gHook.KeyDown += new KeyEventHandler(frmBrower_KeyDown);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gHook.HookedKeys.Add(key);
            }
            gHook.hook();

            Boolean Registered_ESC = RegisterHotKey(this.Handle, 1, 0x0000, (int)Keys.Escape);
            Boolean Registered_F1 = RegisterHotKey(this.Handle, 2, 0x0000, (int)Keys.F1);
        }

        #region 初期化
        /// <summary>
        /// frmBrower_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBrower_Load(object sender, EventArgs e)
        {
            txtURL.Visible = false;
            this.Opacity = 0.3;
        }

        /// <summary>
        /// frmBrower_FormClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBrower_FormClosing(object sender, FormClosingEventArgs e)
        {
            gHook.unhook();
        }
        #endregion

        #region イベント
        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 1: //ESC
                        if (notifyIcon1.Visible)
                        {
                            this.Visible = true;
                            notifyIcon1.Visible = false;
                            this.Focus();
                        }
                        else
                        {
                            this.Visible = false;
                            notifyIcon1.Visible = true;
                        }
                        break;
                    case 2: //F1
                        this.Close();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// browser_FrameLoadEnd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {

            ChromiumWebBrowser browser = (ChromiumWebBrowser)sender;

            browser.SetZoomLevel(0.3);

        }

        /// <summary>
        /// txtURL_Leave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtURL_Leave(object sender, EventArgs e)
        {
            if (!txtURL.Text.Equals(""))
            {
                if (!txtURL.Text.Equals("") && Uri.IsWellFormedUriString(txtURL.Text, UriKind.RelativeOrAbsolute))
                {
                    browser.Load(txtURL.Text);
                }
            }
            txtURL.Visible = false;
        }

        /// <summary>
        /// txtURL_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!txtURL.Text.Equals("") && Uri.IsWellFormedUriString(txtURL.Text, UriKind.RelativeOrAbsolute))
                {
                    browser.Load(txtURL.Text);
                }
                txtURL.Visible = false;
            }
        }

        /// <summary>
        /// notifyIcon1_MouseDoubleClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            notifyIcon1.Visible = false;
        }

        /// <summary>
        /// frmBrower_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBrower_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case (int)Keys.Left:
                    browser.Back();
                    break;
                case (int)Keys.Right:
                    browser.Forward();
                    break;
                case (int)Keys.NumPad0:
                    txtURL.Visible = true;
                    break;
                case (int)Keys.NumPad1:
                    this.Opacity = 0.3;
                    break;
                case (int)Keys.NumPad2:
                    this.Opacity = 0.5;
                    break;
                case (int)Keys.NumPad3:
                    this.Opacity = 0.7;
                    break;
                case (int)Keys.NumPad4:
                    this.Opacity = 1;
                    break;
                case (int)Keys.RControlKey:
                    this.TopMost = !this.TopMost;
                    break;
                case (int)Keys.NumPad7:
                    browser.Load("https://3g.163.com/touch/exclusive/subchannel/qsyk?dataversion=A&uversion=A&version=v_standard&referFrom=");
                    break;
                case (int)Keys.NumPad8:
                    browser.Load("m.cnbeta.com/wap");
                    break;
                case (int)Keys.NumPad9:
                    browser.Load("www.google.com");
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
