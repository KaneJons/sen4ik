using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace sen4ik
{
    public partial class AddService : Form
    {
        public AddService()
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

            // SQL запрос на добавление данных
            string query = "INSERT INTO services (service_name, price) VALUES (:service_name, :price)";

            // Создание команды для выполнения запроса
            NpgsqlCommand command = new NpgsqlCommand(query, conn);

            // Добавление параметров
            command.Parameters.Add(new NpgsqlParameter("service_name", DbType.String));
            command.Parameters.Add(new NpgsqlParameter("price", DbType.Decimal));

            // Установка значений параметров из текстовых полей
            command.Parameters[0].Value = textBox1.Text;  
            command.Parameters[1].Value = Convert.ToDecimal(textBox2.Text); 

            try
            {
                // Выполнение запроса
                command.ExecuteNonQuery();
                MessageBox.Show("Услуга успешно добавлена!");
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
