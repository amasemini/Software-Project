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
    public partial class ReturnBook : Form
    {
        string connStr = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";
        DateTime dueDate;

        private void LoadIssuedBooks()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT LoanID, UserNumber, BookNumber, IssueDate, DueDate FROM tbl_loans WHERE Status = 'Issued'";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvIssuedBooks.DataSource = dt;
            }
        }

        public ReturnBook()
        {
            InitializeComponent();
        }

        private void ReturnBook_Load(object sender, EventArgs e)
        {
            LoadIssuedBooks();
        }

        private void dgvIssuedBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtUserNumber.Text = dgvIssuedBooks.Rows[e.RowIndex].Cells["UserNumber"].Value.ToString();
                txtBookNumber.Text = dgvIssuedBooks.Rows[e.RowIndex].Cells["BookNumber"].Value.ToString();
                dueDate = Convert.ToDateTime(dgvIssuedBooks.Rows[e.RowIndex].Cells["DueDate"].Value);

                DateTime returnDate = dtpReturnDate.Value.Date;
                if (returnDate > dueDate)
                {
                    TimeSpan ts = returnDate - dueDate;
                    int daysLate = ts.Days;
                    txtFine.Text = (daysLate * 20).ToString(); 
                }
                else
                {
                    txtFine.Text = "0";
                }
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserNumber.Text) || string.IsNullOrEmpty(txtBookNumber.Text))
            {
                MessageBox.Show("Please select a record from the list!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal fineAmount = 0;
            if (!string.IsNullOrEmpty(txtFine.Text))
            {
                decimal.TryParse(txtFine.Text, out fineAmount);
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    string updateLoan = "UPDATE tbl_loans SET Status = 'Returned', FineAmount = @Fine " +
                                        "WHERE UserNumber = @User AND BookNumber = @Book AND Status = 'Issued'";

                    using (SqlCommand cmd1 = new SqlCommand(updateLoan, con))
                    {
                        cmd1.Parameters.AddWithValue("@User", txtUserNumber.Text.Trim());
                        cmd1.Parameters.AddWithValue("@Book", txtBookNumber.Text.Trim());
                        cmd1.Parameters.AddWithValue("@Fine", fineAmount); 

                        cmd1.ExecuteNonQuery();
                    }

                    MessageBox.Show("Book Returned Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtUserNumber.Clear();
                    txtBookNumber.Clear();
                    txtFine.Clear();
                    LoadIssuedBooks(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dgvIssuedBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtUserNumber.Text = dgvIssuedBooks.Rows[e.RowIndex].Cells["UserNumber"].Value.ToString();
                txtBookNumber.Text = dgvIssuedBooks.Rows[e.RowIndex].Cells["BookNumber"].Value.ToString();
                dueDate = Convert.ToDateTime(dgvIssuedBooks.Rows[e.RowIndex].Cells["DueDate"].Value);

                DateTime returnDate = dtpReturnDate.Value.Date;

                if (returnDate > dueDate)
                {
                    TimeSpan ts = returnDate - dueDate;
                    int daysLate = ts.Days;
                    txtFine.Text = (daysLate * 20).ToString();
                }
                else
                {
                    txtFine.Text = "0"; 
                }
            }
        }
    }
}
