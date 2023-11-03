using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ForDenis
{
    public partial class Form3 : Form
    {
        private ZooDatabase db;
        private SQLiteConnection sqliteConnection;
        private string currentTableName;
        public Form3()
        {
            InitializeComponent();
            // Проверяем наличие базы данных и создаем экземпляр ZooDatabase только если база данных не существует.
            if (!System.IO.File.Exists("ZooDatabase.db"))
            {
                db = new ZooDatabase("ZooDatabase");
                db.PopulateData();
            }
            else
            {
                db = new ZooDatabase("ZooDatabase");
            }
            sqliteConnection = new SQLiteConnection("Data Source=ZooDatabase.db;Version=3;");
            sqliteConnection.Open();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string tableName = textBox5.Text;

            // Проверяем, существует ли указанная таблица
            if (TableExistsMenu(tableName))
            {
                LoadDataToDataGridView(tableName);

                // Растянуть столбцы равномерно
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            else
            {
                // Если таблица не существует, выводим сообщение об ошибке
                MessageBox.Show("Указанная таблица не существует.");
            }
        }

        private bool TableExistsMenu(string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(sqliteConnection))
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString() == tableName)
                            {
                                return true; // Таблица существует
                            }
                        }
                    }
                }
            }
            return false; // Таблица не существует
        }

        private void LoadDataToDataGridView(string tableName)
        {
            using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {tableName}", sqliteConnection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = "Animals";
            currentTableName = textBox5.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Text = "Enclosures";
            currentTableName = textBox5.Text;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox5.Text = "AnimalEnclosure";
            currentTableName = textBox5.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, что пользователь выбрал хотя бы одну строку в DataGridView.
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку.
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем значение в первой ячейке выбранной строки, которая может быть уникальным идентификатором записи.
                string idValue = selectedRow.Cells[0].Value.ToString();

                // Удаляем строку из базы данных.
                using (SQLiteCommand command = new SQLiteCommand($"DELETE FROM {currentTableName} WHERE ID = @id", sqliteConnection))
                {
                    command.Parameters.AddWithValue("@id", idValue);
                    command.ExecuteNonQuery();
                }

                // Удаляем выбранную строку из DataGridView.
                dataGridView1.Rows.Remove(selectedRow);

                // Опционально: Обновляем DataGridView и таблицу данных.
                LoadDataToDataGridView(currentTableName);
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Сначала проверьте, что все текстовые поля заполнены.
            if (string.IsNullOrWhiteSpace(tb1.Text) || string.IsNullOrWhiteSpace(tb2.Text) || string.IsNullOrWhiteSpace(tb3.Text) || string.IsNullOrWhiteSpace(tb4.Text))
            {
                MessageBox.Show("Заполните все текстовые поля.");
                return;
            }

            using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO Animals (ID, SpeciesName, DailyFoodConsumption, Family, Continent) VALUES (@val0, @val1, @val2, @val3, @val4)", sqliteConnection))
            {
                command.Parameters.AddWithValue("@val0", tbID1.Text);
                command.Parameters.AddWithValue("@val1", tb1.Text);
                command.Parameters.AddWithValue("@val2", tb2.Text);
                command.Parameters.AddWithValue("@val3", tb3.Text);
                command.Parameters.AddWithValue("@val4", tb4.Text);
                command.ExecuteNonQuery();
            }

            // Опционально: Обновляем DataGridView и таблицу данных.
            LoadDataToDataGridView(currentTableName);

            // Очищаем текстовые поля после вставки.
            tb1.Text = "";
            tb2.Text = "";
            tb3.Text = "";
            tb4.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Сначала проверьте, что все текстовые поля заполнены.
            if (string.IsNullOrWhiteSpace(tb5.Text) || string.IsNullOrWhiteSpace(tb6.Text) || string.IsNullOrWhiteSpace(tb7.Text) || string.IsNullOrWhiteSpace(tb8.Text) || string.IsNullOrWhiteSpace(tb9.Text))
            {
                MessageBox.Show("Заполните все текстовые поля.");
                return;
            }

            using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO Enclosures (ID, ComplexName, RoomNumber, HasWater, HasHeating, AnimalCount) VALUES (@val0, @val1, @val2, @val3, @val4, @val5)", sqliteConnection))
            {
                command.Parameters.AddWithValue("@val0", tbID2.Text);
                command.Parameters.AddWithValue("@val1", tb5.Text);
                command.Parameters.AddWithValue("@val2", tb6.Text);
                command.Parameters.AddWithValue("@val3", tb7.Text);
                command.Parameters.AddWithValue("@val4", tb8.Text);
                command.Parameters.AddWithValue("@val5", tb9.Text);
                command.ExecuteNonQuery();
            }

            // Опционально: Обновляем DataGridView и таблицу данных.
            LoadDataToDataGridView(currentTableName);

            // Очищаем текстовые поля после вставки.
            tb5.Text = "";
            tb6.Text = "";
            tb7.Text = "";
            tb8.Text = "";
            tb9.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Сначала проверьте, что все текстовые поля заполнены.
            if (string.IsNullOrWhiteSpace(tb10.Text) || string.IsNullOrWhiteSpace(tb11.Text))
            {
                MessageBox.Show("Заполните все текстовые поля.");
                return;
            }

            using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO AnimalEnclosure (ID, AnimalID, EnclosureID) VALUES (@val0, @val1, @val2)", sqliteConnection))
            {
                command.Parameters.AddWithValue("@val0", tbID3.Text);
                command.Parameters.AddWithValue("@val1", tb10.Text);
                command.Parameters.AddWithValue("@val2", tb11.Text);
                command.ExecuteNonQuery();
            }

            // Опционально: Обновляем DataGridView и таблицу данных.
            LoadDataToDataGridView(currentTableName);

            // Очищаем текстовые поля после вставки.
            tb10.Text = "";
            tb11.Text = "";
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Закрытие подключения к базе данных при закрытии формы
            if (sqliteConnection != null && sqliteConnection.State == ConnectionState.Open)
            {
                sqliteConnection.Close();
            }
            Application.Exit();
        }
    }
}
