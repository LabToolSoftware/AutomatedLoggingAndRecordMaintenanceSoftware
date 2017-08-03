
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ALARMS_x86
{
    public class DBManager
    {
        private SQLiteConnection conn;
        private string _connectionString;

        public DBManager(string dbname)
        {
            this._connectionString = "Data Source=" + dbname + ".sqlite;Version=3;";
            try
            {
                conn = new SQLiteConnection(this._connectionString);
            }
            catch (SQLiteException e)
            {
                MessageBox.Show("Could not connect to database: \n " + e.Message);


                throw;
            }

        }

        public void ExecuteQuery(SQLiteCommand query)
        {
            try
            {
                query.Connection = conn;
                conn.Open();
                query.ExecuteNonQuery();

            }
            catch (SQLiteException e)
            {
                MessageBox.Show("An error occurred when adding data to database. /n The error has been logged.");
                throw;
            }
            finally
            {
                conn.Close();
            }

        }

        public DataTable ExecuteRetrieveDataQuery(SQLiteCommand query)
        {
            try
            {
                query.Connection = conn;
                conn.Open();
                DataTable return_rows = new DataTable();
                return_rows.Load(query.ExecuteReader());
                return return_rows;
            }
            catch (SQLiteException e)
            {
                MessageBox.Show("An error occurred when retrieving data from database: \n" + e.Message);
                ErrorManager erMgr = new ErrorManager();
                erMgr.LogException(e);
                return null;
            }
            finally
            {
                conn.Close();
            }

        }


    }
}

