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
    public partial class ViewUsers : Form
    {
        public ViewUsers()
        {
            InitializeComponent();
        }

        private void ViewUsers_Load(object sender, EventArgs e)
        {
            string connStr = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS; Initial Catalog=library; Integrated Security=true";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Members";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                string selectedUserNumber = dgvUsers.SelectedRows[0].Cells[0].Value.ToString();
                DialogResult result = MessageBox.Show("Are you sure you want to delete user: " + selectedUserNumber + "?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string connectionString = "Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True";
                    string query = "DELETE FROM Members WHERE UserNumber = @UserNumber";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@UserNumber", selectedUserNumber);
                            try
                            {
                                con.Open();
                                int rows = cmd.ExecuteNonQuery();

                                if (rows > 0)
                                {
                                    MessageBox.Show("User deleted successfully from Database!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    string refreshQuery = "SELECT * FROM Members";
                                    using (SqlDataAdapter da = new SqlDataAdapter(refreshQuery, con))
                                    {
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                        dgvUsers.DataSource = dt;
                                    }
                                }

                                else
                                {
                                    MessageBox.Show("Delete failed! User Number not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            catch (Exception ex)
                            {
                                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a full row from the table to delete!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow != null)
            {
                string uNo = dgvUsers.CurrentRow.Cells[0].Value.ToString();
                string uName = dgvUsers.CurrentRow.Cells[1].Value.ToString();
                string uSex = dgvUsers.CurrentRow.Cells[2].Value.ToString();
                string uNIC = dgvUsers.CurrentRow.Cells[3].Value.ToString();
                string uContact = dgvUsers.CurrentRow.Cells[4].Value.ToString();
                string uAddress = dgvUsers.CurrentRow.Cells[5].Value.ToString();
                string uType = dgvUsers.CurrentRow.Cells[6].Value.ToString();

                AddUsers addUsersForm = new AddUsers();
                addUsersForm.LoadUserDataToUpdate(uNo, uName, uSex, uNIC, uContact, uAddress, uType);
                addUsersForm.ShowDialog();

                string refreshQuery = "SELECT * FROM Members";
                using (SqlConnection con = new SqlConnection("Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True"))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(refreshQuery, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvUsers.DataSource = dt;
                    }
                }
            }

            else
            {
                MessageBox.Show("Please select a user from the table to update!", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
