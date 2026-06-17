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
    public partial class LoanBook : Form
    {
        public LoanBook()
        {
            InitializeComponent();
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            if (txtUserNumber.Text == "" || txtBookNumber.Text == "")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            string connStr = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                try
                {
                    con.Open();
                    string checkUserQuery = "SELECT " +
                                            "(SELECT COUNT(*) FROM tbl_loans WHERE UserNumber = @User AND Status = 'Issued') AS CurrentLoans, " +
                                            "(SELECT COUNT(*) FROM tbl_loans WHERE UserNumber = @User AND Status = 'Issued' AND DueDate < CAST(GETDATE() AS DATE)) AS OverdueLoans";

                    int currentLoans = 0;
                    int overdueLoans = 0;

                    using (SqlCommand cmdCheckUser = new SqlCommand(checkUserQuery, con))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@User", txtUserNumber.Text.Trim());
                        using (SqlDataReader reader = cmdCheckUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentLoans = Convert.ToInt32(reader["CurrentLoans"]);
                                overdueLoans = Convert.ToInt32(reader["OverdueLoans"]);
                            }
                        }
                    }

                    if (overdueLoans > 0)
                    {
                        MessageBox.Show("Cannot issue books! This member has overdue books to be returned.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (currentLoans >= 5)
                    {
                        MessageBox.Show("Maximum limit reached! This member has already borrowed 5 books.", "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string checkBookQuery = "SELECT BookType FROM tbl_books WHERE BookNo = @Book";
                    string bookType = "";

                    using (SqlCommand cmdCheckBook = new SqlCommand(checkBookQuery, con))
                    {
                        cmdCheckBook.Parameters.AddWithValue("@Book", txtBookNumber.Text.Trim());
                        object result = cmdCheckBook.ExecuteScalar();

                        if (result != null)
                        {
                            bookType = result.ToString();
                        }
                    }

                    if (bookType.Equals("Reference", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("This book is for 'Reference' only and cannot be borrowed!", "Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string insertQuery = "INSERT INTO tbl_loans (UserNumber, BookNumber, IssueDate, DueDate, Status) " +
                                         "VALUES (@User, @Book, @IDate, @DDate, 'Issued')";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@User", txtUserNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@Book", txtBookNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@IDate", dtpIssueDate.Value.Date);
                        cmd.Parameters.AddWithValue("@DDate", dtpDueDate.Value.Date);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Book Issued Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtUserNumber.Clear();
                        txtBookNumber.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoanBook_Load(object sender, EventArgs e)
        {
            string connStr = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter daUsers = new SqlDataAdapter("SELECT UserNumber, UserName FROM Members", con);
                DataTable dtUsers = new DataTable();
                daUsers.Fill(dtUsers);
                dgvUsers.DataSource = dtUsers;

                SqlDataAdapter daBooks = new SqlDataAdapter("SELECT BookNo, BookTitle FROM tbl_books", con);
                DataTable dtBooks = new DataTable();
                daBooks.Fill(dtBooks);
                dgvBooks.DataSource = dtBooks;
            }
            dtpDueDate.Value = dtpIssueDate.Value.AddDays(14);
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvUsers.Rows[e.RowIndex].Cells["UserNumber"].Value != null)
                {
                    txtUserNumber.Text = dgvUsers.Rows[e.RowIndex].Cells["UserNumber"].Value.ToString();
                }
            }
        }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvBooks.Rows[e.RowIndex].Cells["BookNo"].Value != null)
                {
                    txtBookNumber.Text = dgvBooks.Rows[e.RowIndex].Cells["BookNo"].Value.ToString();
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
