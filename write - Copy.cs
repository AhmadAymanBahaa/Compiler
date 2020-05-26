using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScannerProject
{
    public partial class write : Form
    {
        FileReader file = new FileReader();
        Scanner scanner;
        string text = "Lexeme \t\t\t Token" + Environment.NewLine + Environment.NewLine;
        string path = "";
        
        public write()
        {
            InitializeComponent();
        }

        private void write_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            path += textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))

                path = "code.txt";

            else
                path += "\\code.txt";

            StreamWriter newfile = new StreamWriter(path);
            newfile.Write(textBox2.Text);
            newfile.Close();
            file.readAllFile(path);
            display();

            if (scanner.checkUnmatchedBraces())
                MessageBox.Show("Unmached Braces Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (scanner.checkUnmatchedParanthesis())
                MessageBox.Show("Unmached Parenhesis Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (scanner.checkUnmatchedQuotes())
                MessageBox.Show("Unmached Quotes Error", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        void display()
        {
            scanner = new Scanner(file);

            while (file.lineno < file.lines)
            {
                scanner.getToken(file);
            }

            foreach (var lexeme in scanner.ScannedList)
            {

                text += String.Format("{0} \t\t\t {1}", lexeme.Key.Trim('\0'), lexeme.Value);
                text += Environment.NewLine;

            }
            //if (scanner.checkUnmatchedBraces()) text += "Unmatched Braces";
            textBox2.Text = text;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
