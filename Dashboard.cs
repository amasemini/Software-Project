using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddBooks f = new AddBooks();
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddUsers au = new AddUsers();
            au.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReturnBook rb = new ReturnBook();
            rb.Show();
        }

        private void button2_Click_(object sender, EventArgs e)
        {
            BookInquiry bi = new BookInquiry();
            bi.Show();
        }

        private void button4_Click_(object sender, EventArgs e)
        {
            NewReservation nr = new NewReservation();
            nr.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoanBook lb = new LoanBook();
            lb.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
    }
}
