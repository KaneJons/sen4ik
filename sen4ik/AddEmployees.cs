using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sen4ik
{
    public partial class AddEmployees : Form
    {
        public AddEmployees()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Нельзя оставлять пустые поля!!!");
                return;
            }
            // Создание подключения к базе данных
            NpgsqlConnection conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection);
            conn.Open();

            // SQL запрос на добавление данных
            string query = "INSERT INTO employees (name, position, phone) VALUES (:name, :position, :phone)";

            // Создание команды для выполнения запроса
            NpgsqlCommand command = new NpgsqlCommand(query, conn);

            // Добавление параметров
            command.Parameters.Add(new NpgsqlParameter("name", DbType.String));
            command.Parameters.Add(new NpgsqlParameter("position", DbType.String));
            command.Parameters.Add(new NpgsqlParameter("phone", DbType.String));

            // Установка значений параметров из текстовых полей
            command.Parameters[0].Value = textBox1.Text;       
            command.Parameters[1].Value = textBox2.Text;   
            command.Parameters[2].Value = textBox3.Text;   

            try
            {
                // Выполнение запроса
                command.ExecuteNonQuery();
                MessageBox.Show("Сотрудник успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                // Закрытие соединения
                conn.Close();
                Close();
            }
        }
    }
}
