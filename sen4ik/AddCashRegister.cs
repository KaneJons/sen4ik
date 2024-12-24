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
    public partial class AddCashRegister : Form
    {

        public AddCashRegister()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Нельзя оставлять пустые поля!!!");
                return;
            }
            try
            {
                // Получаем значения из полей ввода
                int clientId = int.Parse(textBox1.Text);
                int serviceId = int.Parse(textBox2.Text);
                int employeeId = int.Parse(textBox3.Text);
                decimal amount = decimal.Parse(textBox4.Text);

                // Проверяем внешние ключи
                if (!IsForeignKeyValid("client_id", clientId) ||
                    !IsForeignKeyValid("service_id", serviceId) ||
                    !IsForeignKeyValid("employee_id", employeeId))
                {
                    MessageBox.Show("Некоторые из указанных внешних ключей не существуют.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Добавляем запись в таблицу
                using (var conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection))
                {
                    conn.Open();
                    string query = @"
                    INSERT INTO cash_register (client_id, service_id, employee_id, amount)
                    VALUES (:clientId, :serviceId, :employeeId, :amount)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("clientId", clientId);
                        cmd.Parameters.AddWithValue("serviceId", serviceId);
                        cmd.Parameters.AddWithValue("employeeId", employeeId);
                        cmd.Parameters.AddWithValue("amount", amount);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsForeignKeyValid(string columnName, object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return false;

                string referenceTable = "";
                switch(columnName)
                {
                    case "client_id":
                        referenceTable = "clients";
                            break;
                    case "service_id":
                        referenceTable = "services";
                        break;
                    case "employee_id":
                        referenceTable = "employees";
                        break;
                    default:
                        break;
                };

                if (referenceTable == null)
                    return false;

                using (var conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection))
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM {referenceTable} WHERE {columnName.Replace("_id", "_id")} = :value";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("value", value);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
