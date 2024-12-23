using Npgsql;
using sen4ik.shopDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace sen4ik
{
    public enum TypeTable
    {
        None,
        Clients,
        Services,
        Employees,
        CashRegister,
        Users
    }
    public partial class Form2 : Form
    {
        TypeTable CurrentTable = TypeTable.None;
        TypeTable OldCurrentTable = TypeTable.None;

        Dictionary<TypeTable, string> TableLabel = new Dictionary<TypeTable, string> 
        {
            { TypeTable.Clients, "Таблица \"Клиенты\"" },
            { TypeTable.Services, "Таблица \"Услуги\"" },
            { TypeTable.Employees, "Таблица \"Сотрудники\"" },
            { TypeTable.CashRegister, "Таблица \"Кассовый журнал\"" },
            { TypeTable.Users, "Таблица \"Пользователи\"" },
            { TypeTable.None, "" },
        };

        Dictionary<TypeTable, string> NameTableLabel = new Dictionary<TypeTable, string>
        {
            { TypeTable.Clients, "clients" },
            { TypeTable.Services, "services" },
            { TypeTable.Employees, "employees" },
            { TypeTable.CashRegister, "cash_register" },
            { TypeTable.Users, "users" },
            { TypeTable.None, "" },
        };

        Dictionary<TypeTable, string> IdTableLabel = new Dictionary<TypeTable, string>
        {
            { TypeTable.Clients, "client_id" },
            { TypeTable.Services, "service_id" },
            { TypeTable.Employees, "employee_id" },
            { TypeTable.CashRegister, "transaction_id" },
            { TypeTable.Users, "user_id" },
            { TypeTable.None, "" },
        };
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                this.clientsTableAdapter.Fill(this.shopDataSet.clients);
                this.servicesTableAdapter.Fill(this.shopDataSet.services);
                this.employeesTableAdapter.Fill(this.shopDataSet.employees);
                this.cash_registerTableAdapter.Fill(this.shopDataSet.cash_register);
                this.usersTableAdapter.Fill(this.shopDataSet.users);

                // Связываем BindingSource
                clientsBindingSource.DataSource = shopDataSet.clients;
                servicesBindingSource.DataSource = shopDataSet.services;
                employeesBindingSource.DataSource = shopDataSet.employees;
                cashregisterBindingSource.DataSource = shopDataSet.cash_register;
                usersBindingSource.DataSource = shopDataSet.users;

                CurrentTable = TypeTable.Clients;
                label1.Text = TableLabel[CurrentTable];

                // Устанавливаем начальный источник
                dataGridView1.DataSource = clientsBindingSource;
                bindingNavigator1.BindingSource = clientsBindingSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }

        }
        private void ReloadData()
        {
            switch(CurrentTable)
            {
                case TypeTable.Clients:
                    clientsTableAdapter.Fill(shopDataSet.clients);
                    dataGridView1.DataSource = clientsBindingSource;
                    bindingNavigator1.BindingSource = clientsBindingSource;
                    break;
                case TypeTable.Services:
                    servicesTableAdapter.Fill(shopDataSet.services);
                    dataGridView1.DataSource = servicesBindingSource;
                    bindingNavigator1.BindingSource = servicesBindingSource;
                    break;
                case TypeTable.Employees:
                    employeesTableAdapter.Fill(shopDataSet.employees);
                    dataGridView1.DataSource = employeesBindingSource;
                    bindingNavigator1.BindingSource = employeesBindingSource;
                    break;
                case TypeTable.CashRegister:
                    cash_registerTableAdapter.Fill(shopDataSet.cash_register);
                    dataGridView1.DataSource = cashregisterBindingSource;
                    bindingNavigator1.BindingSource = cashregisterBindingSource;
                    break;
                case TypeTable.Users:
                    usersTableAdapter.Fill(shopDataSet.users);
                    dataGridView1.DataSource = usersBindingSource;
                    bindingNavigator1.BindingSource = usersBindingSource;

                    break;
                default:
                    break;

            }
            label1.Text = TableLabel[CurrentTable];
        }

       

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли выделенная строка
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Получаем индекс строки и значение ID
                    int row = dataGridView1.CurrentRow.Index;
                    int column = 0; // Предполагаем, что ID находится в первом столбце
                    int id = Convert.ToInt32(dataGridView1[column, row].Value);

                    // Удаляем запись из базы данных
                    DeleteRecordFromDatabase(id);

                    // Удаляем строку из DataGridView
                    dataGridView1.Rows.RemoveAt(row);

                    // Сбрасываем выделение в DataGridView
                    dataGridView1.ClearSelection();

                    MessageBox.Show($"Запись {id} успешно удалена!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Выберите строку для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecordFromDatabase( int id)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection))
            {
                conn.Open();
                string query = $"DELETE FROM {NameTableLabel[CurrentTable]} WHERE {IdTableLabel[CurrentTable]} = @id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void клиентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTable = TypeTable.Clients;
            ReloadData();
        }

        private void услугиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTable = TypeTable.Services;
            ReloadData();
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTable = TypeTable.Employees;
            ReloadData();
        }

        private void кассовыйЖурналToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTable = TypeTable.CashRegister;
            ReloadData();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTable = TypeTable.Users;
            ReloadData();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (CurrentTable == TypeTable.None)
                return;

            if (OldCurrentTable == CurrentTable)
            {

                try
                {
                    string tableName = NameTableLabel[CurrentTable]; // Имя текущей таблицы.
                    string primaryKeyColumn = IdTableLabel[CurrentTable]; // Первичный ключ.
                    string columnName = dataGridView1.Columns[e.ColumnIndex].Name; // Имя изменённого столбца.
                    object newValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value; // Новое значение.
                    object primaryKeyValue = dataGridView1.Rows[e.RowIndex].Cells[primaryKeyColumn].Value; // Значение первичного ключа.

                    using (var conn = new NpgsqlConnection(Properties.Settings.Default.DBConnection))
                    {
                        conn.Open();

                        string query;

                        // Обработка для таблицы "users" при изменении пароля.
                        if (CurrentTable == TypeTable.Users && columnName == "password")
                        {
                            query = $"UPDATE {tableName} SET {columnName} = crypt(:value, gen_salt('bf')) WHERE {primaryKeyColumn} = :id";
                        }
                        // Обработка для таблицы "cash_register" с внешними ключами.
                        else if (CurrentTable == TypeTable.CashRegister)
                        {
                            // Проверка на корректность значений внешних ключей.
                            if ((columnName == "client_id" || columnName == "service_id" || columnName == "employee_id") && !IsForeignKeyValid(columnName, newValue))
                            {
                                MessageBox.Show($"Недопустимое значение для внешнего ключа {columnName}.");
                                return;
                            }

                            query = $"UPDATE {tableName} SET {columnName} = :value WHERE {primaryKeyColumn} = :id";
                        }
                        // Обработка остальных случаев.
                        else
                        {
                            query = $"UPDATE {tableName} SET {columnName} = :value WHERE {primaryKeyColumn} = :id";
                        }

                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("value", newValue ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("id", primaryKeyValue);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Данные успешно обновлены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления данных: {ex.Message}");
                }
                finally
                {
                    dataGridView1.ClearSelection();
                    ReloadData();
                }
            }
        }

        private bool IsForeignKeyValid(string columnName, object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                    return false;

                string referenceTable = NameTableLabel[CurrentTable];


                if (string.IsNullOrEmpty(referenceTable))
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


        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            OldCurrentTable = CurrentTable;
        }
    }
}
