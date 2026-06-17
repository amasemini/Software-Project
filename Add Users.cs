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
    public partial class AddUsers : Form
    {
        bool isUpdateMode = false;
        string connString = "Data Source=DESKTOP-N8ERL0B\\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True";

        public AddUsers()
        {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void AddUsers_Load(object sender, EventArgs e)
        {
            if (isUpdateMode == false)
            {
                GetNextUserID();
            }
        }
        private void GetNextUserID()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT MAX(CAST(SUBSTRING(UserNumber, 2, LEN(UserNumber)) AS INT)) FROM Members";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    int nextID = 1; 

                    if (result != DBNull.Value && result != null)
                    {
                        nextID = Convert.ToInt32(result) + 1;
                    }

                    txtUserNumber.Text = "U" + nextID;
                    txtUserNumber.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error ID Generation: " + ex.Message);
                }
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (cmbUserType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select User Type!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Members VALUES (@ID, @Name, @Sex, @NIC, @Contact, @Address, @Type)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", txtUserNumber.Text);
                cmd.Parameters.AddWithValue("@Name", txtUserName.Text);
                cmd.Parameters.AddWithValue("@Sex", rdoMale.Checked ? "Male" : "Female");
                cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Type", cmbUserType.SelectedItem.ToString());

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User Added Successfully!");
                    txtUserName.Clear();
                    txtNIC.Clear();
                    txtContact.Clear();
                    txtAddress.Clear();
                    cmbUserType.SelectedIndex = -1;

                    GetNextUserID();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void btnAddUser_Click_1(object sender, EventArgs e)
        {
            if (cmbUserType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select User Type!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Members VALUES (@ID, @Name, @Sex, @NIC, @Contact, @Address, @Type)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", txtUserNumber.Text);
                cmd.Parameters.AddWithValue("@Name", txtUserName.Text);
                cmd.Parameters.AddWithValue("@Sex", rdoMale.Checked ? "Male" : "Female");
                cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Type", cmbUserType.SelectedItem.ToString());

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User Added Successfully!");
                    txtUserName.Clear();
                    txtNIC.Clear();
                    txtContact.Clear();
                    txtAddress.Clear();
                    cmbUserType.SelectedIndex = -1;
                    GetNextUserID();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ViewUsers viewUsersForm = new ViewUsers();
            viewUsersForm.ShowDialog();
        }

        internal void LoadUserDataToUpdate(string uNo, string uName, string uSex, string uNIC, string uContact, string uAddress, string uType)
        {
            txtUserNumber.Text = uNo;   
            txtUserName.Text = uName;     
            txtNIC.Text = uNIC;         
            txtContact.Text = uContact; 
            txtAddress.Text = uAddress;     
            cmbUserType.Text = uType;   

            if (uSex == "Male")
            {
                rdoMale.Checked = true;
            }
            else
            {
                rdoFemale.Checked = true;
            }

            button1.Enabled = false;
            isUpdateMode = true;
        }

        private void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source = DESKTOP-N8ERL0B\\SQLEXPRESS ;Initial Catalog=library;Integrated Security=True";
            string query = "UPDATE Members SET UserName = @UserName, Sex = @Sex, NICNo = @NIC, ContactNo = @Contact, Address = @Address, UserType = @UserType WHERE UserNumber = @UserNumber";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserNumber", txtUserNumber.Text); 
                    cmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                    cmd.Parameters.AddWithValue("@Sex", rdoMale.Checked ? "Male" : "Femail");
                    cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                    cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@UserType", cmbUserType.Text);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); 
                        }

                        else
                        {
                            MessageBox.Show("Update failed! User Number not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}

