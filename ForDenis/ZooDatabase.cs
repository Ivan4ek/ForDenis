using System;
using System.Data;
using System.Data.SQLite;

public class ZooDatabase
{
    private SQLiteConnection connection;

    public ZooDatabase(string databaseName)
    {
        connection = new SQLiteConnection($"Data Source={databaseName}.db");
        connection.Open();
        // Проверяем, были ли данные уже заполнены
        CreateTables();
    }

    public void PopulateData()
    {
        // Добавляем записи о животных
        InsertAnimal("Лев", 5.0, "Кошачьи", "Африка");
        InsertAnimal("Тигр", 3.0, "Кошачьи", "Евразия");
        InsertAnimal("Слон", 150.0, "Слоновые", "Африка");
        InsertAnimal("Пингвин", 0.5, "Пингвиновые", "Антарктида");
        InsertAnimal("Горилла", 4.0, "Приматы", "Африка");

        // Добавляем записи о помещениях
        InsertEnclosure("Павильон 1", 101, true, true, 10);
        InsertEnclosure("Павильон 2", 102, false, true, 5);
        InsertEnclosure("Павильон 3", 103, true, false, 8);
        InsertEnclosure("Павильон 4", 104, false, true, 12);
        InsertEnclosure("Павильон 5", 105, true, true, 15);

        // Устанавливаем отношения между животными и помещениями
        int lionAnimalID = GetAnimalIDBySpeciesName("Слон");
        int monkeyAnimalID = GetAnimalIDBySpeciesName("Горилла");
        int elephantAnimalID = GetAnimalIDBySpeciesName("Лев");
        int penguinAnimalID = GetAnimalIDBySpeciesName("Пингвин");
        int gorillaAnimalID = GetAnimalIDBySpeciesName("Тигр");

        int pavilion1EnclosureID = GetEnclosureIDByComplexName("Павильон 4");
        int pavilion2EnclosureID = GetEnclosureIDByComplexName("Павильон 5");
        int pavilion3EnclosureID = GetEnclosureIDByComplexName("Павильон 3");
        int pavilion4EnclosureID = GetEnclosureIDByComplexName("Павильон 1");
        int pavilion5EnclosureID = GetEnclosureIDByComplexName("Павильон 2");

        InsertAnimalEnclosure(lionAnimalID, pavilion1EnclosureID);
        InsertAnimalEnclosure(monkeyAnimalID, pavilion2EnclosureID);
        InsertAnimalEnclosure(elephantAnimalID, pavilion3EnclosureID);
        InsertAnimalEnclosure(penguinAnimalID, pavilion4EnclosureID);
        InsertAnimalEnclosure(gorillaAnimalID, pavilion5EnclosureID);
    }

    public void CreateTables()
    {
        string createAnimalsTable = "CREATE TABLE IF NOT EXISTS Animals ("
            + "ID INTEGER PRIMARY KEY AUTOINCREMENT, "
            + "SpeciesName TEXT, "
            + "DailyFoodConsumption REAL, "
            + "Family TEXT, "
            + "Continent TEXT)";

        string createEnclosuresTable = "CREATE TABLE IF NOT EXISTS Enclosures ("
            + "ID INTEGER PRIMARY KEY AUTOINCREMENT, "
            + "ComplexName TEXT, "
            + "RoomNumber INTEGER, "
            + "HasWater BOOLEAN, "
            + "HasHeating BOOLEAN, "
            + "AnimalCount INTEGER)";

        string createAnimalEnclosureTable = "CREATE TABLE IF NOT EXISTS AnimalEnclosure ("
            + "ID INTEGER PRIMARY KEY AUTOINCREMENT, "
            + "AnimalID INTEGER, "
            + "EnclosureID INTEGER)";

        using (SQLiteCommand command = new SQLiteCommand(createAnimalsTable, connection))
        {
            command.ExecuteNonQuery();
        }

        using (SQLiteCommand command = new SQLiteCommand(createEnclosuresTable, connection))
        {
            command.ExecuteNonQuery();
        }

        using (SQLiteCommand command = new SQLiteCommand(createAnimalEnclosureTable, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void InsertAnimal(string speciesName, double dailyFoodConsumption, string family, string continent)
    {
        string insertQuery = "INSERT INTO Animals (SpeciesName, DailyFoodConsumption, Family, Continent) VALUES (@SpeciesName, @DailyFoodConsumption, @Family, @Continent)";

        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@SpeciesName", speciesName);
            command.Parameters.AddWithValue("@DailyFoodConsumption", dailyFoodConsumption);
            command.Parameters.AddWithValue("@Family", family);
            command.Parameters.AddWithValue("@Continent", continent);

            command.ExecuteNonQuery();
        }
    }

    public void InsertEnclosure(string complexName, int roomNumber, bool hasWater, bool hasHeating, int animalCount)
    {
        string insertQuery = "INSERT INTO Enclosures (ComplexName, RoomNumber, HasWater, HasHeating, AnimalCount) VALUES (@ComplexName, @RoomNumber, @HasWater, @HasHeating, @AnimalCount)";

        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@ComplexName", complexName);
            command.Parameters.AddWithValue("@RoomNumber", roomNumber);
            command.Parameters.AddWithValue("@HasWater", hasWater);
            command.Parameters.AddWithValue("@HasHeating", hasHeating);
            command.Parameters.AddWithValue("@AnimalCount", animalCount);

            command.ExecuteNonQuery();
        }
    }

    public void InsertAnimalEnclosure(int animalID, int enclosureID)
    {
        string insertQuery = "INSERT INTO AnimalEnclosure (AnimalID, EnclosureID) VALUES (@AnimalID, @EnclosureID)";

        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@AnimalID", animalID);
            command.Parameters.AddWithValue("@EnclosureID", enclosureID);

            command.ExecuteNonQuery();
        }
    }

    public int GetAnimalIDBySpeciesName(string speciesName)
    {
        string query = "SELECT ID FROM Animals WHERE SpeciesName = @SpeciesName";
        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@SpeciesName", speciesName);
            object result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                // Если не найдено, вернуть -1 или другое значение, которое указывает на отсутствие записи.
                return -1;
            }
        }
    }

    public int GetEnclosureIDByComplexName(string complexName)
    {
        string query = "SELECT ID FROM Enclosures WHERE ComplexName = @ComplexName";
        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ComplexName", complexName);
            object result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                // Если не найдено, вернуть -1 или другое значение, которое указывает на отсутствие записи.
                return -1;
            }
        }
    }

    public double GetDailyFoodConsumptionByComplex(string complexName)
    {
        string query = "SELECT SUM(A.DailyFoodConsumption) FROM Animals A " +
                       "JOIN AnimalEnclosure AE ON A.ID = AE.AnimalID " +
                       "JOIN Enclosures E ON AE.EnclosureID = E.ID " +
                       "WHERE E.ComplexName = @ComplexName";

        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ComplexName", complexName);
            object result = command.ExecuteScalar();

            if (result != DBNull.Value)
            {
                return Convert.ToDouble(result);
            }
            else
            {
                return 0.0;
            }
        }
    }

    public bool HasReproductionWithoutWater(string speciesName)
    {
        string query = "SELECT COUNT(*) FROM Animals A " +
                       "JOIN AnimalEnclosure AE ON A.ID = AE.AnimalID " +
                       "JOIN Enclosures E ON AE.EnclosureID = E.ID " +
                       "WHERE A.SpeciesName = @SpeciesName AND E.HasWater = 0";

        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@SpeciesName", speciesName);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }

    public int GetTotalNumberOfFamily(string familyName)
    {
        string query = "SELECT SUM(E.AnimalCount) FROM Enclosures E " +
                       "JOIN AnimalEnclosure AE ON E.ID = AE.EnclosureID " +
                       "JOIN Animals A ON AE.AnimalID = A.ID " +
                       "WHERE A.Family = @FamilyName";

        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@FamilyName", familyName);
            object result = command.ExecuteScalar();

            if (result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }
    }

    public string GetPairsOfSpeciesInEnclosures(string complexName)
    {
        string query = "SELECT DISTINCT A1.SpeciesName AS Species1, A2.SpeciesName AS Species2 " +
                       "FROM Animals A1 " +
                       "JOIN AnimalEnclosure AE1 ON A1.ID = AE1.AnimalID " +
                       "JOIN Enclosures E ON AE1.EnclosureID = E.ID " +
                       "JOIN AnimalEnclosure AE2 ON E.ID = AE2.EnclosureID " +
                       "JOIN Animals A2 ON AE2.AnimalID = A2.ID " +
                       "WHERE A1.ID < A2.ID AND E.ComplexName = @ComplexName";

        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ComplexName", complexName);

            DataTable pairsTable = new DataTable();
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
            {
                adapter.Fill(pairsTable);
            }

            // Создаем строку для вывода результата
            string result = "Пары видов, которые содержатся в одном помещении:\n";
            foreach (DataRow row in pairsTable.Rows)
            {
                string species1 = row["Species1"].ToString();
                string species2 = row["Species2"].ToString();
                result += $"{species1} и {species2}\n";
            }

            return result;
        }
    }

    public void CloseConnection()
    {
        connection.Close();
    }
}
