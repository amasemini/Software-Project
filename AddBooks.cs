using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class AddBooks : Form
    {
        private string fullBookNo;
        private object bookType;

        public AddBooks()
        {
            InitializeComponent();
        }

        public string Value { get; private set; }

        private void AddBooks_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True";
            string bookType = rdoReference.Checked ? "Reference" : "Borrowable";
            string classification = cmbClassification.Text.Substring(0, 1);
            int quantity = Convert.ToInt32(numQuantity.Value);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    int nextNumber = 1;
                    string checkQuery = "SELECT COUNT(DISTINCT SUBSTRING(BookNo, 1, 6)) FROM tbl_books WHERE Classification = '" + classification + "'";

                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conn))
                    {
                        int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                        nextNumber = count + 1;
                    }

                    string baseBookNo = classification + " " + nextNumber.ToString("D4");

                    for (int i = 1; i <= quantity; i++)
                    {
                        string fullBookNo = baseBookNo + "-" + i.ToString("D2");

                        using (SqlCommand cmd = new SqlCommand("SP_Reg_Book", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@BookNo", fullBookNo);
                            cmd.Parameters.AddWithValue("@BookTitle", txtBookTitle.Text);
                            cmd.Parameters.AddWithValue("@AuthorName", txtAuthorName.Text);
                            cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text);
                            cmd.Parameters.AddWithValue("@Classification", classification);
                            cmd.Parameters.AddWithValue("@BookType", bookType);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show(quantity + " Copies Registered Automatically! Base No: " + baseBookNo, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtBookTitle.Clear();
                    txtAuthorName.Clear();
                    txtPublisher.Clear();
                    cmbClassification.SelectedIndex = -1;
                    numQuantity.Value = 1;
                    rdoReference.Checked = false;
                    rdoBorrowable.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBookTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal void LoadBookDataToUpdate(string bNo, string bTitle, string bAuthor, string bPublisher, string bClass, string bType, int bQty)
        {
            this.fullBookNo = bNo;
            txtBookTitle.Text = bTitle;
            txtAuthorName.Text = bAuthor;
            txtPublisher.Text = bPublisher;
            numQuantity.Value = bQty;
            cmbClassification.Text = bClass;

            cmbClassification.SelectedIndex = -1;
            for (int i = 0; i < cmbClassification.Items.Count; i++)
            {
                if (cmbClassification.Items[i].ToString().StartsWith(bClass))
                {
                    cmbClassification.SelectedIndex = i;
                    break;
                }
            }

            if (bType == "Reference")
            {
                rdoReference.Checked = true;
            }
            else
            {
                rdoBorrowable.Checked = true;
            }

            button1.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fullBookNo))
            {
                MessageBox.Show("No book selected to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connectionString = "Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True";
            string bookNoBase = fullBookNo.Length >= 6 ? fullBookNo.Substring(0, 6) : fullBookNo;

            int newQuantity = Convert.ToInt32(numQuantity.Value); 
            int currentQuantity = 0; 
            if (newQuantity > 10)
            {
                MessageBox.Show("Maximum quantity allowed is 10!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string countQuery = "SELECT COUNT(*) FROM tbl_books WHERE BookNo LIKE @Base + '%'";
                    using (SqlCommand cmdCount = new SqlCommand(countQuery, con))
                    {
                        cmdCount.Parameters.AddWithValue("@Base", bookNoBase);
                        currentQuantity = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }

                    string updateDetailsQuery = "UPDATE tbl_books SET BookTitle = @Title, AuthorName = @Author, Publisher = @Pub, BookType = @Type, Quantity = @Qty WHERE BookNo LIKE @Base + '%'";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateDetailsQuery, con))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Base", bookNoBase);
                        cmdUpdate.Parameters.AddWithValue("@Title", txtBookTitle.Text);
                        cmdUpdate.Parameters.AddWithValue("@Author", txtAuthorName.Text);
                        cmdUpdate.Parameters.AddWithValue("@Pub", txtPublisher.Text);
                        cmdUpdate.Parameters.AddWithValue("@Type", rdoReference.Checked ? "Reference" : "Borrowable");
                        cmdUpdate.Parameters.AddWithValue("@Qty", newQuantity);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    if (newQuantity > currentQuantity)
                    {
                        string classification = cmbClassification.Text.Substring(0, 1);
                        string bookType = rdoReference.Checked ? "Reference" : "Borrowable";

                        for (int i = currentQuantity + 1; i <= newQuantity; i++)
                        {
                            string nextFullBookNo = bookNoBase + "-" + i.ToString("D2");
                            using (SqlCommand cmdInsert = new SqlCommand("SP_Reg_Book", con))
                            {
                                cmdInsert.CommandType = CommandType.StoredProcedure;
                                cmdInsert.Parameters.AddWithValue("@BookNo", nextFullBookNo);
                                cmdInsert.Parameters.AddWithValue("@BookTitle", txtBookTitle.Text);
                                cmdInsert.Parameters.AddWithValue("@AuthorName", txtAuthorName.Text);
                                cmdInsert.Parameters.AddWithValue("@Publisher", txtPublisher.Text);
                                cmdInsert.Parameters.AddWithValue("@Classification", classification);
                                cmdInsert.Parameters.AddWithValue("@BookType", bookType);
                                cmdInsert.Parameters.AddWithValue("@Quantity", newQuantity);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }
  
                    else if (newQuantity < currentQuantity)
                    {
                        for (int i = currentQuantity; i > newQuantity; i--)
                        {
                            string targetBookNo = bookNoBase + "-" + i.ToString("D2");
                            string deleteQuery = "DELETE FROM tbl_books WHERE BookNo = @TargetNo";

                            using (SqlCommand cmdDelete = new SqlCommand(deleteQuery, con))
                            {
                                cmdDelete.Parameters.AddWithValue("@TargetNo", targetBookNo);
                                cmdDelete.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Book details and Quantity updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
