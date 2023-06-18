using Mono.Data.Sqlite; // 1
using System.Data; // 1
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private int hitCount = 0;  
    private string connectionString;

    void Start()
    {
        string databaseName = "data.db";
        string databasePath = Application.persistentDataPath + "/" + databaseName;

        // Crear la cadena de conexión a la base de datos
        connectionString = "URI=file:" + databasePath;

        // Crear una tabla si no existe
        CreateTable();
        
        // Insertar datos de ejemplo
        InsertData("John Doe", 25);
        InsertData("Jane Smith", 30);
        
        // Consultar y mostrar los datos
        SelectData();
    }

    private void CreateTable()
    {
        using (IDbConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Player (Name TEXT, Age INTEGER)";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private void InsertData(string name, int age)
    {
        using (IDbConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Player (Name, Age) VALUES (@Name, @Age)";
                command.Parameters.Add(new SqliteParameter("@Name", name));
                command.Parameters.Add(new SqliteParameter("@Age", age));
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private void SelectData()
    {
        using (IDbConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Player";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int age = reader.GetInt32(1);

                        Debug.Log("Name: " + name + ", Age: " + age);
                    }
                }
            }

            connection.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            hitCount++;

            InsertData("Damian", hitCount);
        }
    }

    private IDbConnection CreateAndOpenDatabase() // 3
    {
        // Open a connection to the database.
        string dbUri = "URI=file:MyDatabase.sqlite"; // 4
        IDbConnection dbConnection = new SqliteConnection(dbUri); // 5
        dbConnection.Open(); // 6

        // Create a table for the hit count in the database if it does not exist yet.
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand(); // 6
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS HitCountTableSimple (id INTEGER PRIMARY KEY, hits INTEGER )"; // 7
        dbCommandCreateTable.ExecuteReader(); // 8

        return dbConnection;
    }
}
