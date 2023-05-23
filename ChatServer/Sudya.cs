using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    //класс судьи для решения того, кто победил
    internal static class Sudya
    {
        public static Messag cl1;
        public static Messag cl2;
        static string  cl1_choise = "z"; // выбор игрока 1
        static string cl2_choise = "z"; // выбор игрока 2
        static string Reshenie =null;
        static List<Messag> messagList = new List<Messag>();



        static public void Add_mes(Messag m)
        {
            messagList.Add(m);
            if (messagList.Count > 1)
            {
                cl1 = messagList.First<Messag>();
                cl2 = messagList.Last<Messag>();
                cl1_choise = cl1.choise;
                cl2_choise = cl2.choise;
            }
        }

        static void Hod()
        {
            if (cl1_choise != "z" && cl2_choise != "z")
            {
                if (cl1_choise == cl2_choise)
                {
                    Reshenie = "Ничья";
                }
                if (cl1_choise == "k" && cl2_choise == "n") // камень - ножницы
                {
                    Reshenie = cl1.Name;
                }
                if (cl1_choise == "k" && cl2_choise == "b")// камень - бумага
                {
                   Reshenie = cl2.Name;
                }
                if (cl1_choise == "n" && cl2_choise == "k")// ножницы - камень
                {
                    Reshenie = cl2.Name;
                }
                if (cl1_choise == "n" && cl2_choise == "b")// ножницы - бумага
                {
                    Reshenie = cl1.Name;
                }
                if (cl1_choise == "b" && cl2_choise == "k")// бумага - камень
                {
                    Reshenie = cl1.Name;
                }
                if (cl1_choise == "b" && cl2_choise == "n")// бумага - ножницы
                {
                    Reshenie = cl2.Name;
                }
            }
           
        }

        //выбор победителя
        static public string Winner()
        {
            string buf=null; 
            if (messagList.Count == 2)
            {
                Hod();
            }
            if (Reshenie !=null)
            {
                buf = Reshenie;
                Reshenie = null;
                messagList.Clear();
                cl1 = null;
                cl2 = null;
            }
            return buf;
        }
    }
}
