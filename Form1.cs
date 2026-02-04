using Google.Protobuf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic_Management_System
{
    public partial class Form1 : Form
    {
        string connStr = "server=127.0.0.1;port=3306;database=clinic_db;uid=root;pwd=;";

        public Form1()
        {
            InitializeComponent();
        }
        void clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            textBox1.Focus();
        }
        void load_patient()
        {
            using(MySqlConnection con =new MySqlConnection(connStr))
            {
                string query = "SELECT patient_id, name, age, phone FROM patients;\r\n";
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataTable da_ta = new DataTable();
                da.Fill(da_ta);
                dataGridView1.DataSource = da_ta;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            textBox1.Text = dataGridView1.CurrentRow.Cells["name"].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells["age"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["phone"].Value.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox1, "");
            errorProvider1.SetError(textBox2, "");

            try
            {


                if (String.IsNullOrWhiteSpace(textBox1.Text))
                {
                    errorProvider1.SetError(textBox1, "this field is required");
                    return;
                }
                using (var con = new MySqlConnection(connStr))
                {
                    con.Open();
                    string query = @"INSERT INTO patients(name,age,gender,phone,address,notes)
              VALUES(@name,@age,@gender,@phone,@address,@notes)";

                    using (var cmd = new MySqlCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("@name", textBox1.Text);
                        if (int.TryParse(textBox2.Text, out int r))
                        {
                            cmd.Parameters.AddWithValue("@age", r);

                        }
                        else
                        {
                            errorProvider1.SetError(textBox2, "this must be digit");
                            textBox2.Focus();
                            textBox2.SelectionStart = 0;
                            textBox2.SelectionLength = textBox2.Text.Length;

                            return;
                        }
                        cmd.Parameters.AddWithValue("@gender", comboBox1.SelectedItem);

                        cmd.Parameters.AddWithValue("@phone", textBox3.Text);
                        cmd.Parameters.AddWithValue("@address", textBox4.Text);
                        cmd.Parameters.AddWithValue("@notes", textBox5.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("the informatin was saved");
                        clear();
                        load_patient();


                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection(connStr);
            try
            {
                con.Open();
                MessageBox.Show("yes");
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button5.Visible = false;
            load_patient();
            dataGridView1.CellClick += dataGridView1_CellClick; 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a patient first");
                return;
            }

            int selected_patient_id = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["patient_id"].Value);

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                string query = @"UPDATE patients 
                         SET name=@name, age=@age, phone=@phone 
                         WHERE patient_id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", textBox1.Text);

                    if (int.TryParse(textBox2.Text, out int ageValue))
                        cmd.Parameters.AddWithValue("@age", ageValue);
                    else
                    {
                        MessageBox.Show("Age must be a number");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@phone", textBox3.Text);
                    cmd.Parameters.AddWithValue("@id", selected_patient_id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            clear();
             MessageBox.Show("Updated successfully");
            load_patient(); 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "SQL File (*.sql)|*.sql";
            sfd.Title = "Save Database Backup";
            sfd.FileName = "clinic_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".sql";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = sfd.FileName;

                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.Arguments = $"/c \"C:\\Program Files\\MySQL\\MySQL Server 8.4\\bin\\mysqldump.exe\" -u root clinic_db > \"{path}\"";
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;

                    Process.Start(psi);

                    MessageBox.Show("Backup created successfully ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a patient first");
                return;
            }

            int selected_patient_id = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["patient_id"].Value);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this patient?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(connStr))
                    {
                        string query = "DELETE FROM patients WHERE patient_id=@id";

                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", selected_patient_id);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Patient deleted successfully ");
                    load_patient(); 
                    clear(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
