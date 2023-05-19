using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    //класс сообщения
    public class Messag
    {
        public string Name { get; set; } = "empty"; // имя отправителя
        public string Mes { get; set; } // содержание сообщения
        public bool Common  { get; set; } = true; // общее ли сообщение
        public string choise { get; set; }= "z"; // выбор в игре
        public Messag (string mes, string name)
        {
            Mes = mes;
            Name = name;
        }
        public Messag(string mes, string name, string c)
        {
            Mes = mes;
            Name = name;
            choise = c;
        }
        public Messag() { }
    }
}
