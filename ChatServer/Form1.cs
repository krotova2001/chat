using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Text.Json;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        Mutex mutex1;
        Mutex mutex2;
        Messag m1; // игровое сообщение в пределах раунда
        Messag m2;
        List<ChatClient> chatClients=new List<ChatClient>(); // список всех клиентов
        Sudya sudya;
        public Form1()
        {
            InitializeComponent();
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 777);
                listener.Start();
                Task.Run(() => { Listening(listener); });
                Task.Run(() => { Send_clients_list(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            mutex1 = new Mutex();
            mutex2 = new Mutex();
            sudya = new Sudya();
        }

        private void Listening(TcpListener listener)
        {
            try
            {
                while (true)
                {
                    if (listener.Pending())
                    {
                        TcpClient cl = listener.AcceptTcpClient();
                        this.Invoke(new Action(
                            () =>
                            {
                                cmd.Text += $"\n{cl.Client.RemoteEndPoint}";
                            }
                            ));
                        string Name = "";
                        StreamReader sr = new StreamReader(cl.GetStream(), Encoding.Unicode);
                        Name = sr.ReadLine();
                        ChatClient cln = new ChatClient();
                        cln.Name = Name;
                        cln.Client = cl;
                        lock (chatClients)
                        {
                            chatClients.Add(cln);
                        }
                        this.Invoke(new Action(
                                                    () =>
                                                    {
                                                        cmd.Text += $"\nclients:{chatClients.Count}";
                                                    }
                                                    ));
                        Task.Run(() => { ReadClient(cln); });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                listener.Stop();
            }
        }
        private void ReadClient(ChatClient client)
        {
            try
            {
                string d = "";
                string c;
                StreamReader sr = new StreamReader(client.Client.GetStream(), Encoding.Unicode);
                do
                {
                    string data_raw = sr.ReadLine();
                    string data = Base64Decode(data_raw);
                    Messag mes = JsonSerializer.Deserialize<Messag>(data);
                    d = mes.Mes;
                    c = mes.choise;
                    {
                        this.Invoke(new Action(
                            () =>
                            {
                                cmd.Text += $"\n{client.Name}-{mes.Mes}";
                                cmd.Text += $"\n{client.Name + " выбор"}-{mes.choise}";
                            }
                            ));
                    }
                    
                    if(!mes.Common&&mes.Mes=="hod")
                    {
                        //Gaming();
                        lock (sudya)
                        {
                            {
                                sudya.Add_mes(mes);
                            }
                            String winner = sudya.Winner();
                            if (winner != null)
                            {
                                this.Invoke(new Action(
                                () =>
                                {
                                    cmd.Text += $"\nWinner - {winner}";
                                }
                                ));
                            }
                        }
                    }
                    
                    {
                        byte[] m = Encoding.Unicode.GetBytes(data_raw+"\r\n");
                        foreach (ChatClient cl in chatClients)
                        {
                            if (cl != client)
                            {
                                NetworkStream sm = cl.Client.GetStream();
                                sm.Write(m, 0, m.Length);
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
                                        cmd.Text += $"\nerr-{ex.Message}";
                                    }
                                    ));
            }
            finally
            {
                client.Client.Close();
                this.Invoke(new Action(
                    () =>
                    {
                        cmd.Text += $"\nClose - {client.Name}";
                    }
                    ));
                lock (chatClients)
                {
                    chatClients.Remove(client);
                }
            }
        }

        //отпарвка клиенту всех соединенных клиентов
        private void Send_clients_list()
        {
            while (true)
            {
                if (chatClients.Count > 0)
                {
                    //с определенной периодичностью отсылаем всем актуальный список клиентов в сети
                    Thread.Sleep(1500);
                    Messag messag = new Messag();
                    messag.Name = "List";
                    messag.Common = false;
                    messag.Mes = " ";
                    foreach (ChatClient cl in chatClients)
                    {
                        messag.list.Add(cl.Name);
                    }
                    foreach (ChatClient cl in chatClients)
                    {
                        NetworkStream sm = cl.Client.GetStream();
                        string jsonString2 = Base64Encode(JsonSerializer.Serialize<Messag>(messag));
                        byte[] m = Encoding.Unicode.GetBytes(jsonString2+"\r\n");
                        sm.Write(m, 0, m.Length);
                        //sm.Close();
                    }
                }
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes)+"/r/n";
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.Unicode.GetString(base64EncodedBytes);
        }

        //начинаем игру
        private void Gaming()
        {
            timer1 = new System.Windows.Forms.Timer();
            timer1.Enabled = true;
            timer1.Start();
           
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            mutex1.ReleaseMutex();
            mutex2.ReleaseMutex();
        }
    }
}
