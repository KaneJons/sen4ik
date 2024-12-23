using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sen4ik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }
        
            try
            {
                using (var conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection))
                {
                    conn.Open();
                    string query = "SELECT user_id FROM users WHERE username = @username AND password = (crypt(@password, password))";
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        int userId = Convert.ToInt32(command.ExecuteScalar());
                        if (userId != 0)
                        {
                            MessageBox.Show($"Добро пожаловать, пользователь ID: {userId}!");

                            // Переход на следующую форму
                            Form2 frm2 = new Form2();
                            frm2.Show();
                            this.Hide();

                        }
                        else
                        MessageBox.Show("Неверный логин или пароль.");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

    }
}
