using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace sen4ik
{
    public partial class AddUsers : Form
    {
        public AddUsers()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Нельзя оставлять пустые поля!!!");
                return;
            }
            // Создание подключения к базе данных
            NpgsqlConnection conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection);
            conn.Open();

            // SQL запрос на добавление данных с использованием функции хэширования gen_salt('bf') и crypt()
            string query = "INSERT INTO users (username, password) VALUES (:username, crypt(:password, gen_salt('bf')))";

            // Создание команды для выполнения запроса
            NpgsqlCommand command = new NpgsqlCommand(query, conn);

            // Добавление параметров
            command.Parameters.Add(new NpgsqlParameter("username", DbType.String));
            command.Parameters.Add(new NpgsqlParameter("password", DbType.String));

            // Установка значений параметров из текстовых полей
            command.Parameters[0].Value = textBox1.Text;   
            command.Parameters[1].Value = textBox2.Text;   

            try
            {
                // Выполнение запроса
                command.ExecuteNonQuery();
                MessageBox.Show("Пользователь успешно добавлен!");
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
