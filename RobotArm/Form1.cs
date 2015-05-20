using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace RobotArm
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool found;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string com in SerialPort.GetPortNames())
            {
                Thread t = new Thread(Check) { IsBackground = true };
                t.Start(com);
            }

            while (!found)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }

            /*serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM5";
            serialPort.Open();*/

        }

        private void Check(object name)
        {
            SerialPort p = new SerialPort((string)name, 9600);
            bool closeme = true;

            try
            {
                p.Open();

                while (!found)
                {
                    if (p.ReadByte() == '0')
                    {
                        found = true;
                        closeme = false;
                        serialPort = p;
                    }

                    Thread.Sleep(1);
                }

                if(closeme) p.Close();

            }
            catch (Exception)
            {
                
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort.Write("1");
            //MessageBox.Show(serialPort.PortName.ToString());
        }
    }
}
