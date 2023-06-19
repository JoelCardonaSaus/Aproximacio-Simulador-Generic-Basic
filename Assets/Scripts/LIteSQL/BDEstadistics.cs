using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class BDEstadistics : MonoBehaviour
{

    private static BDEstadistics instancia;

    private BDEstadistics() { }

    public static BDEstadistics Instancia {
        get {
            if (instancia == null){
                instancia = FindObjectOfType<BDEstadistics>();

                if (instancia == null){
                    GameObject singletonObject = new GameObject();
                    instancia = singletonObject.AddComponent<BDEstadistics>();
                    singletonObject.name = typeof(BDEstadistics).ToString();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instancia;
        }
    }


    void Start()
    {
  
    }

    public void ActualitzaEstadistics(string nomObjecte, string nomEstadistic, float valor){
        IDbConnection dbConnection = CrearIObrirBD();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        string val = valor.ToString().Replace(",", ".");
        dbCommandInsertValue.CommandText = "INSERT OR REPLACE INTO Estadistics VALUES ('" +nomObjecte+"', '" + nomEstadistic +"', " + val + ")";
        dbCommandInsertValue.ExecuteNonQuery(); // 11

        // Remember to always close the connection at the end.
        dbConnection.Close(); // 12
    }

    private IDbConnection CrearIObrirBD() // 3
    {
        // Open a connection to the database.
        string dbUri = "URI=file:Estadistics.sqlite";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        // Create a table for the hit count in the database if it does not exist yet.
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS Estadistics (nomObjecte VARCHAR(100) NOT NULL, nomEstadistic VARCHAR(100) NOT NULL, valor REAL, PRIMARY KEY ('nomObjecte', 'nomEstadistic') )"; // 7
        dbCommandCreateTable.ExecuteReader();

        return dbConnection;
    }
}
