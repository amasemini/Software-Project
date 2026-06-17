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
    public partial class BookInquiry : Form
    {
        string connString = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS;Initial Catalog=library;Integrated Security=True";
        public BookInquiry()
        {
            InitializeComponent();
        }

        private void BookInquiry_Load(object sender, EventArgs e)
        {
            LoadAllBooks();
        }
        private void LoadAllBooks()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT MAX(BookNo) AS [BookNo], BookTitle, AuthorName, Publisher, Classification, BookType, COUNT(BookTitle) AS [Quantity] FROM tbl_books GROUP BY BookTitle, AuthorName, Publisher, Classification, BookType";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    adapter.Fill(dt);
                    dgvBooks.DataSource = dt; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading books: " + ex.Message);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT MAX(BookNo) AS [BookNo], BookTitle, AuthorName, Publisher, Classification, BookType, COUNT(BookTitle) AS [Quantity] FROM tbl_books WHERE BookTitle LIKE @search OR BookNo LIKE @search GROUP BY BookTitle, AuthorName, Publisher, Classification, BookType";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    adapter.Fill(dt);
                    dgvBooks.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Search Error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count > 0)
            {
                string selectedBookNo = dgvBooks.SelectedRows[0].Cells[0].Value.ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to delete book: " + selectedBookNo + "?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string connectionString = "Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True";
                    string query = "DELETE FROM tbl_books WHERE BookNo = @BookNo";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@BookNo", selectedBookNo);

                            try
                            {
                                con.Open();
                                int rows = cmd.ExecuteNonQuery();

                                if (rows > 0)
                                {
                                    MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadAllBooks();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row from the table to delete!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count > 0)
            {
                string bNo = dgvBooks.SelectedRows[0].Cells[0].Value.ToString();
                string bTitle = dgvBooks.SelectedRows[0].Cells["BookTitle"].Value.ToString();
                string bAuthor = dgvBooks.SelectedRows[0].Cells["AuthorName"].Value.ToString();
                string bPublisher = dgvBooks.SelectedRows[0].Cells["Publisher"].Value.ToString();
                string bClass = dgvBooks.SelectedRows[0].Cells["Classification"].Value.ToString();
                string bType = dgvBooks.SelectedRows[0].Cells["BookType"].Value.ToString();
                int bQty = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["Quantity"].Value);

                AddBooks addBooksForm = new AddBooks();

                addBooksForm.LoadBookDataToUpdate(bNo, bTitle, bAuthor, bPublisher, bClass, bType, bQty);

                addBooksForm.ShowDialog();
                LoadAllBooks();
            }
            else
            {
                MessageBox.Show("Please select a book row from the table to update!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
