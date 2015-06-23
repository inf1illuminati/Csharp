using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace RobotArm
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private bool found; //voor het automatisch vinden van de comport

        private bool up, down, left, right; //pijltjestoetsen aangeven

        private string CHAR_STOP = "1";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string com in SerialPort.GetPortNames()) //code voor het vinden van de comport waarop de arduino is aangesloten, kan zowel via usb als via bleutooth
            {
                Thread t = new Thread(Check) { IsBackground = true };
                t.Start(com);
            }

            while (!found)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }


            //serialport uitlezen om te kijken wat de arduino terugstuurd
            Thread ja = new Thread(() => {
                while (true)
                {
                    try
                    {
                        //Console.Write(serialPort.ReadExisting());
                        Thread.Sleep(1);
                    }
                    catch (Exception) { }
                }
            });
            ja.IsBackground = true;
            ja.Start();

            /*serialPort = new SerialPort(); //oude code die de serialport niet automatisch vind
            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM5";
            serialPort.Open();*/

        }

        private void Check(object name) //verdere code voor het automatisch vinden van de comport
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) up = true;
            if (e.KeyCode == Keys.Down) down = true;
            if (e.KeyCode == Keys.Left) left = true;
            if (e.KeyCode == Keys.Right) right = true;

            if (e.KeyCode == Keys.Space) serialPort.Write("h");

            if (up) serialPort.Write("2");
            if (down) serialPort.Write("3");
            if (left && !up) serialPort.Write("f");
            if (right && !up) serialPort.Write("g");

            if (up && left)
            {
                serialPort.Write("7");
                Console.WriteLine("upleft");
            }
            if (up && right)
            {
                serialPort.Write("8");
                Console.WriteLine("upright");
            }

            if (e.KeyCode == Keys.D1) serialPort.Write("a");
            if (e.KeyCode == Keys.D2) serialPort.Write("b");
            if (e.KeyCode == Keys.D3) serialPort.Write("c");
            if (e.KeyCode == Keys.D4) serialPort.Write("d");
            if (e.KeyCode == Keys.D5) serialPort.Write("e");

            Console.WriteLine(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) up = false;
            if (e.KeyCode == Keys.Down) down = false;
            if (e.KeyCode == Keys.Left) left = false;
            if (e.KeyCode == Keys.Right) right = false;

            if (up) serialPort.Write("2");
            else if (down) serialPort.Write("3");
            else if (left) serialPort.Write("f");
            else if (right) serialPort.Write("g");
            else
            {
                serialPort.Write(CHAR_STOP);
                Console.WriteLine("stop");
            }
        }
    }
}
