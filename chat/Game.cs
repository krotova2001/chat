using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class Game : Form
    {
        public TcpClient tcpClient = null;
        string client1; // игрок 1
        string client2; // игрок 2
        int cl1_choise=0; // выбор игрока 1
        int cl2_choise=0; // выбор игрока 2
        int client_1_score = -1; // счет одного 
        int client_2_score = -1; // счет второго


        private void Game_Load(object sender, EventArgs e)
        {
            Task.Run(() => { Scoring(); }); // мониторим счет и выводим его
            Task.Run(() => { Start_game(); });
        }


        public Game()
        {
            InitializeComponent();
        }

        public Game(string cl1, string cl2, TcpClient tcp)
        {
            InitializeComponent();
            client1 = cl1;
            client2 = cl2;
            //tcpClient = tcp;
            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 777);
            NetworkStream sm = tcpClient.GetStream();
            string d = "Game "+ client1 + "\n";
            byte[] m = Encoding.Unicode.GetBytes(d);
            sm.Write(m, 0, m.Length);
            this.Text = client1 + " играет c " + client2;
        }

        //Камень
        private void button1_Click(object sender, EventArgs e)
        {
            cl1_choise = 1;
            Send();
        }

        //Ножницы
        private void button2_Click(object sender, EventArgs e)
        {
            cl1_choise = 2;
            Send();
        }

        //Бумага
        private void button3_Click(object sender, EventArgs e)
        {
            cl1_choise = 3;
            Send();
        }

        //сделать один ход и выбор победителя хода
        void Hod ()
        {
                if (cl1_choise > 0 && cl2_choise > 0)
                {
                    if (cl1_choise == cl2_choise)
                    {
                        label2.Text = "Ничья";
                    }
                    if (cl1_choise == 1 && cl2_choise == 2) // камень - ножницы
                    {
                        client_1_score++;
                    }
                    if (cl1_choise == 1 && cl2_choise == 3)// камень - бумага
                    {
                        client_2_score++;
                    }
                    if (cl1_choise == 2 && cl2_choise == 1)// ножницы - камень
                    {
                        client_2_score++;
                    }
                    if (cl1_choise == 2 && cl2_choise == 3)// ножницы - бумага
                    {
                        client_1_score++;
                    }
                    if (cl1_choise == 3 && cl2_choise == 1)// бумага - камень
                    {
                        client_1_score++;
                    }
                    if (cl1_choise == 3 && cl2_choise == 2)// бумага - ножницы
                    {
                        client_2_score++;
                    }
                cl1_choise = 0;
                cl2_choise = 0;
            }
                else
                {
                this.Invoke(new Action(
                  () =>
                  {
                      label2.Text = "Ждем ход...";
                  }
                  ));
                }
        }

        //запуск игры
        private void Start_game()
        {
            try
            {
                Messag mes;
                StreamReader sr = new StreamReader(tcpClient.GetStream(), Encoding.Unicode);
                while (true)
                {
                    string data = Base64Decode(sr.ReadLine()); // получим выбор оппонента
                    mes = JsonSerializer.Deserialize<Messag>(data);
                    {
                        cl2_choise = mes.choise; // записали выбор оппонента
                    }
                    if (cl1_choise > 0 && cl2_choise > 0)
                    {
                        Task.Run(() => { Hod(); });
                        textBox1.Text = mes.choise.ToString();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //завершить
            private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
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

        private void Scoring()
        {
            while (true)
            {
                this.Invoke(new Action(
                   () =>
                   {
                       label3.Text = client_1_score.ToString();
                       label4.Text = client_2_score.ToString();
                   }
                   ));
             
            }
        }

        private void Send()
        {
            {
                Messag to_send = new Messag("  ", client1);
                to_send.Common = false;
                to_send.choise = cl1_choise;
                NetworkStream sm = tcpClient.GetStream();
                string jsonString = Base64Encode(JsonSerializer.Serialize<Messag>(to_send));
                jsonString += "\r\n";
                byte[] m = Encoding.Unicode.GetBytes(jsonString);
                sm.Write(m, 0, m.Length);
            }
        }
    }
}
