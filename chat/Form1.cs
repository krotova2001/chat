﻿using System;
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
                    //Task.Run(() => { Listen_pool(); });
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
                    mes = JsonSerializer.Deserialize<Messag>(sr.ReadToEnd());
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
                    NetworkStream sm = tcpClient.GetStream();
                    byte[] m = new byte[1024];
                    string jsonString;
                    int l = sm.Read(m,0,m.Length);
                    jsonString = Base64Decode(Encoding.UTF8.GetString(m));
                    List <string> lst = JsonSerializer.Deserialize<List<string>>(jsonString);
                    foreach (string cc in lst)
                        listBox1.Items.Add(cc);
                    sm.Close();
                }
            }
           
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes) + "/r/n";
        }

    }
}