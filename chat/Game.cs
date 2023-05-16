using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class Game : Form
    {
        string client1; // игрок 1
        string client2; // игрок 2
        int cl1_choise=0; // выбор игрока 1
        int cl2_choise=0; // выбор игрока 2
        int client_1_score = 0; // счет одного 
        int client_2_score = 0; // счет второго
        public Game()
        {
            InitializeComponent();
        }

        public Game(string cl1)
        {
            client1 = cl1;
        }

        //Камень
        private void button1_Click(object sender, EventArgs e)
        {
            cl1_choise = 1;
        }

        //Ножницы
        private void button2_Click(object sender, EventArgs e)
        {
            cl1_choise = 2;
        }

        //Бумага
        private void button3_Click(object sender, EventArgs e)
        {
            cl1_choise = 3;
        }

        //сделать один ход
        void Hod ()
        {
            while (cl1_choise != -1 || cl2_choise != -1)
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
                }
                else
                {
                    label2.Text = "Ждем ход...";
                }
            }
            
        }

        //завершить
        private void button4_Click(object sender, EventArgs e)
        {
            cl1_choise = -1;
            this.Close();
        }
    }
}
