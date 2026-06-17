using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Library_Management_System
{
    public partial class NewReservation : Form
    {
        public NewReservation()
        {
            InitializeComponent();
        }

        private void btnReserve_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserNumber.Text) || string.IsNullOrEmpty(txtBookNumber.Text))
            {
                MessageBox.Show("Please enter both User Number and Book Number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connString = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";

            using (SqlConnection con = new SqlConnection(connString))
            {
                string query = "INSERT INTO tbl_reservations (UserNumber, BookNumber, ReservationDate, Status) " +
                               "VALUES (@UserNumber, @BookNumber, @ReservationDate, 'Pending')";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserNumber", txtUserNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@BookNumber", txtBookNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@ReservationDate", dtpReservedDate.Value.Date);

                    try
                    {
                        con.Open();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Book Reserved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUserNumber.Clear();
                            txtBookNumber.Clear();
                            dtpReservedDate.Value = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error keeping reservation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        string connString = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";
        private void NewReservation_Load(object sender, EventArgs e)
        {
            LoadUsers();
            LoadBooks();
        }
        private void LoadUsers()
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                string query = "SELECT UserNumber, UserName FROM Members";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvUsers.DataSource = dt;
            }
        }

        private void LoadBooks()
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                string query = "SELECT BookNo, BookTitle FROM tbl_books";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBooks.DataSource = dt;
            }
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                if (row.Cells[0].Value != null)
                {
                    txtUserNumber.Text = row.Cells[0].Value.ToString();
                }
            }
        }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBooks.Rows[e.RowIndex];
                if (row.Cells[0].Value != null)
                {
                    txtBookNumber.Text = row.Cells[0].Value.ToString();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
