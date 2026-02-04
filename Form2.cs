using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic_Management_System
{
    public partial class Form2 : Form
    {
        string connStr = "server=127.0.0.1;port=3306;database=clinic_db;uid=root;pwd=;";

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox1, "");
            errorProvider1.SetError(textBox2, "");

            string username = textBox1.Text.Trim();
            string user_password = textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                errorProvider1.SetError(textBox1, "this required");

            }
            else if (string.IsNullOrWhiteSpace(user_password))
            {
                errorProvider1.SetError(textBox2, "this required");

            }
            else
            {
                try {

                    using (MySqlConnection con = new MySqlConnection(connStr))
                    {
                        con.Open();
                        string query = "SELECT COUNT(*) FROM users WHERE username=@username AND user_password=@user_password";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@user_password", user_password);
                            int result = Convert.ToInt32(cmd.ExecuteScalar());
                            if (result > 0)
                            {
                                Form1 f1 = new Form1();
                                f1.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password!");

                            }

                        }

                    }
                    
                
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

              
        }
    }
}
