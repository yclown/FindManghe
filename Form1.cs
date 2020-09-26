using Fiddler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manghe
{
    public partial class Form1 : Form
    {
        public int iSecureEndpointPort = 7777;
        public string sSecureEndpointHostname = "localhost";
        public static string targetboxno = "";
        public static int offsettime = 1000;
        public static int findcount = 0;
        public int startPort = 8877;
        public bool isIgnoreCertError = false;
        public Proxy oSecureEndpoint;
        public FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.AllowRemoteClients | FiddlerCoreStartupFlags.DecryptSSL;

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //模拟鼠标滚轮滚动操作，必须配合dwData参数
        const int MOUSEEVENTF_WHEEL = 0x0800;


        public static void TestMoveMouse()
        {
            Console.WriteLine("模拟鼠标移动5个像素点。");
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);//相对当前鼠标位置x轴和y轴分别移动50像素
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);//相对当前鼠标位置x轴和y轴分别移动50像素
            //mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -20, 0);//鼠标滚动，使界面向下滚动20的高度
        }


        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Shutting down...");
            Fiddler.FiddlerApplication.Shutdown();
            System.Threading.Thread.Sleep(750);
        }



        public void Start(int port)
        {
            if (FiddlerApplication.IsStarted())
            {
                Shutdown();
                FiddlerApplication.BeforeResponse -= FiddlerApplication_BeforeResponse;
            }

            if (!Fiddler.CertMaker.rootCertExists())
            {
                if (!Fiddler.CertMaker.createRootCert())
                {
                    throw new Exception("Unable to create cert for FiddlerCore.");
                }
            }
            if (!Fiddler.CertMaker.rootCertIsTrusted())
            {
                if (!Fiddler.CertMaker.trustRootCert())
                {
                    throw new Exception("Unable to install FiddlerCore's cert.");
                }
            }

            Fiddler.CONFIG.IgnoreServerCertErrors = isIgnoreCertError;
            Fiddler.CONFIG.bMITM_HTTPS = false;
            
            this.startPort = port;
            //FiddlerApplication.Startup(startPort, FiddlerCoreStartupFlags.DecryptSSL);//启动侦听 
            FiddlerApplication.Startup(startPort, FiddlerCoreStartupFlags.Default | FiddlerCoreStartupFlags.RegisterAsSystemProxy);
            //创建一个https侦听器，用于伪装成https服务器
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
            Fiddler.CertMaker.trustRootCert();
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;//注册事件，用于捕获网络流量
            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
        }

        public void Shutdown()
        {
            if (null != oSecureEndpoint) oSecureEndpoint.Dispose();
            Fiddler.FiddlerApplication.Shutdown();
            
            System.Threading.Thread.Sleep(500);
             
        }
        private void FiddlerApplication_BeforeResponse(Session oSession)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
           // this.textBox1.AppendText(oSession.url + "\n");
            var isMusicRequest = false;

            if (oSession.host == "boxonline.paquapp.com" &&
                oSession.url.Contains("extract") )   // url
            {
                isMusicRequest = true;

            }
            if (isMusicRequest)
            {
               findcount++;
               var responseStringOriginal = oSession.GetResponseBodyAsString();
               var boxno=  Regex.Match(responseStringOriginal, "\"box_no\":\"(\\d+)\"").Groups[1].Value;
               this.textBox1.AppendText("第"+ findcount + "个盲盒 编号："+boxno + "\r\n");
                if(targetboxno!= boxno)
                {
                    Thread.Sleep(offsettime);
                    TestMoveMouse();
                }
                
            }

                 
            
        }
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TestMoveMouse();
            targetboxno = this.textBox2.Text;
            if (string.IsNullOrEmpty(targetboxno))
            {
                MessageBox.Show("输入要找的盲盒编号");
                return;
            }

            if (!string.IsNullOrEmpty(this.textBox3.Text))
            {
                //MessageBox.Show("输入要找的盲盒编号");
                offsettime = Convert.ToInt32(this.textBox3.Text);
                //return;
            }
            this.button1.Enabled = false;
            this.Start(this.startPort);
            this.textBox1.Text = "开始\r\n";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = true;
            this.Shutdown();
        }

        //private void Form1_Load(object sender, EventArgs e)
        //{

        //}

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Shutdown();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox3.Text = "1000";
        }
    }

}
