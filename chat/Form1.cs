using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace chat
{
    public partial class Form1 : Form
    {
        public TcpClient tcpClient = null;
        public string self_name;
        public Form1()
        {
            InitializeComponent();
        }

        //Connect
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "")
            {
                tcpClient = new TcpClient();
                try
                {
                    tcpClient.Connect("127.0.0.1", 777);
                    button1.Enabled = false;
                    NetworkStream sm = tcpClient.GetStream();
                    string d = textBox1.Text.Trim() + "\n";
                    byte[] m = Encoding.Unicode.GetBytes(d);
                    sm.Write(m, 0, m.Length);
                    Task.Run(() => { ReadData(); });
                    Task.Run(() => { Listen_pool(); });
                    self_name = d;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    tcpClient = null;
                    self_name = null;
                }
            }
            else
            {
                MessageBox.Show("Input Name");
            }
        }

        void ReadData()
        {
            try
            {
                string d = "";
                Messag mes;
                StreamReader sr = new StreamReader(tcpClient.GetStream(), Encoding.Unicode);
                do
                {
                    string data = Base64Decode(sr.ReadLine());
                    if (data != null)
                    {
                        mes = JsonSerializer.Deserialize<Messag>(data);
                        if (mes.Common) // тут фильтруются игровые сообщения от общения
                        {
                            d = mes.Mes;
                            if (d.Trim() != "")
                            {
                                this.Invoke(new Action(
                                    () =>
                                    {
                                        cmd.Text += $"\n{d}";
                                    }
                                    ));
                            }
                        }
                        else
                        {
                            //если это сообщение не общее и приглашение к игре
                            if (mes.Mes == "game"&&!mes.Common)
                            {
                                Task.Run(() => { Game_call(mes); });
                            }
                        }
                    }
                } while (d != "exit");
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(
                    () =>
                    {
                        cmd.Text += $"\n---- Error ----\n {ex.Message}";
                    }
                    ));
            }
            finally
            {
                tcpClient.Close();
                tcpClient = null;
                this.Invoke(
                    new Action(() => { button1.Enabled = true; })
                    );
            }
        }

        //send
        private void button2_Click(object sender, EventArgs e)
        {
            if (tcpClient != null)
            {
                NetworkStream sm = tcpClient.GetStream();
                string d = textBox2.Text.Trim() + "\n";
                Messag msg = new Messag(d, self_name);
                string jsonString = Base64Encode(JsonSerializer.Serialize<Messag>(msg));
                jsonString +="\r\n";
                byte[] m = Encoding.Unicode.GetBytes(jsonString);
                sm.Write(m, 0, m.Length);
                cmd.Text += $"\nI'm:{msg.Mes.Trim()}";
            }
            else
            {
                MessageBox.Show("Connect to Server");
            }
        }

        //прием входящих сообщений о том кто онлайн
        private void Listen_pool()
        {
            while (true)
            {
                if (tcpClient != null)
                {
                    Messag mes;
                    StreamReader sr = new StreamReader(tcpClient.GetStream(), Encoding.Unicode);
                    string data = Base64Decode(sr.ReadLine());
                    if (data != null)
                    {
                        mes = JsonSerializer.Deserialize<Messag>(data);
                        if (!mes.Common && mes.Name == "List") // тут фильтруются сообщения
                        {
                            {
                                this.Invoke(new Action(
                                () =>
                                {
                                    foreach (string item in mes.list)
                                        listBox1.Items.Add(item);
                                }
                                ));
                            }
                        }
                    }
                }
            }
           
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.Unicode.GetString(base64EncodedBytes);
        }

        //Game
        private void button3_Click(object sender, EventArgs e)
        {
            Messag messag = new Messag("game", self_name);
            messag.Common = false;
            NetworkStream sm = tcpClient.GetStream();
            string jsonString = Base64Encode(JsonSerializer.Serialize<Messag>(messag));
            jsonString += "\r\n";
            byte[] m = Encoding.Unicode.GetBytes(jsonString);
            sm.Write(m, 0, m.Length);
            Game game = new Game(self_name,"", tcpClient);
            game.ShowDialog();
        }

        //Game пассивно
        private void Game_call(Messag m)
        {
            Game game = new Game(self_name, m.Name, tcpClient);
            game.ShowDialog(); //тут обязательно в диалоговом режиме, иначе виснет
        }

        
    }
}
