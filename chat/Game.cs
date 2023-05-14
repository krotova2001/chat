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
        ChatClient client1; // игрок 1
        ChatClient client2; // игрок 2
        public Game()
        {
            InitializeComponent();
        }
    }
}
