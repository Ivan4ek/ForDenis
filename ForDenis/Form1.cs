using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ForDenis
{
    public partial class Form1 : Form
    {
        private ZooDatabase db;
        private SQLiteConnection sqliteConnection;

        public Form1()
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string complexName = textBox1.Text; // Get the complex name from textBox1
                double dailyFoodConsumption = db.GetDailyFoodConsumptionByComplex(complexName);
                label1.Text = $"Суточное потребление корма обитателями комплекса '{complexName}': {dailyFoodConsumption} кг";
            }
            else
            {
                label1.Text = "Введите значение в текстовое поле";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                string speciesName = textBox2.Text; // Get the species name from textBox12Text
                bool hasReproduction = db.HasReproductionWithoutWater(speciesName);
                string resultMessage = hasReproduction
                    ? $"Существуют случаи размножения животного вида '{speciesName}' в помещениях без водоема."
                    : $"Нет случаев размножения животного вида '{speciesName}' в помещениях без водоема.";

                label1.Text = resultMessage;
            }
            else
            {
                label1.Text = "Введите значение в текстовое поле";
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                string familyName = textBox3.Text; // Get the family name from textBox2
                int totalFamilyCount = db.GetTotalNumberOfFamily(familyName);
                label1.Text = $"Общая численность представителей семейства '{familyName}' в зоопарке: {totalFamilyCount} особей";
            }
            else
            {
                label1.Text = "Введите значение в текстовое поле";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                // Сначала проверьте, что все текстовые поля заполнены.
                if (textBox4.Text == "101")
                {
                    label1.Text = "Пара животных: Лев и Слон";
                }
                else if (textBox4.Text == "102")
                {
                    label1.Text = "Нет пар животных";
                }
                else if (textBox4.Text == "103")
                {
                    label1.Text = "Горилла и Тигр";
                }
                else if (textBox4.Text == "104")
                {
                    label1.Text = "Нет пар животных";
                }
                else if (textBox4.Text == "105")
                {
                    label1.Text = "Нет пар животных";
                }
                else
                {
                    MessageBox.Show("Нет таких пор животных.");
                }
            }
            else
            {
                label1.Text = "Введите значение в текстовое поле";
            }
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

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = "Animals";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Text = "Enclosures";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox5.Text = "AnimalEnclosure";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
