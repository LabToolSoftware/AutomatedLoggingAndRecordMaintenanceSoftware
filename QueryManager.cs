using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;


namespace ALARMS_x86
{
    public interface ILogger
    {
        void LogException(string errMsg);
    }
    class QueryManager : ILogger
    {
        private DBManager _dbMgr;

        public QueryManager()
        {
            ConnectToDB("PSFRU");
            Dictionary<string, string> InstrumentTableFields = new Dictionary<string, string>
            {
                { "Id", "integer primary key autoincrement" },
                { "DeviceID", "varchar(100)"},
                { "DeviceName","varchar(50)" },
                { "TimeUsed", "varchar(50)" },
                { "DateofLastService","varchar(50)" },
                //{"ServiceThreshhold","varchar(50)" }
            };
            Dictionary<string, string> TimeLogTableFields = new Dictionary<string, string>
            {
                { "Id", "integer primary key autoincrement" },
                { "DeviceID", "varchar(100)"},
                { "DeviceName","varchar(50)" },
                { "UserName", "varchar(50)" },
                { "DateUsed","varchar(50)" },
                {"TimeOn", "varchar(50)"},
                {"TimeOff","varchar(50)" },
                {"TimeUsed","varchar(50)" }
            };

            Dictionary<string, string> ErrorLogTableFields = new Dictionary<string, string>
            {
                {"Id", "integer primary key autoincrement" },
                {"DateTime", "varchar(50)" },
                {"Class", "varchar(100)" },
                { "Method", "varchar(100)" },
                { "ErrorType","varchar(100)"},
                {"ErrorMessage","varchar(100)" }
            };

            CreateTable("ErrorLog", ErrorLogTableFields);
            CreateTable("Instruments", InstrumentTableFields);
            CreateTable("TimeLog", TimeLogTableFields);
        }

        private void ConnectToDB(string dbname)
        {
            _dbMgr = new DBManager(dbname);
        }

        public void InsertRow(string tablename, Query data)
        {
            string fields = String.Join(",", GetTableInfo(tablename));
            SQLiteCommand query = new SQLiteCommand();
            var queryobj_props = data.GetType().GetProperties();
            List<string> _values = new List<string>();
            _values.Add("null");
            for (int i = 0; i < queryobj_props.Length; i++)
            {
                _values.Add("'" + data.GetType().GetProperty(queryobj_props[i].Name).GetValue(data, null).ToString() + "'");
            }
            string new_values = String.Join(",", _values);
            query.CommandText = "INSERT INTO " + tablename + " VALUES (" + new_values + ")";
            _dbMgr.ExecuteQuery(query);
        }

        public DataTable RetrieveData(string tablename)
        {
            SQLiteCommand query = new SQLiteCommand();
            query.CommandText = "SELECT * FROM " + tablename;
            DataTable returnedrows = _dbMgr.ExecuteRetrieveDataQuery(query);
            return returnedrows;
        }

        public DataTable RetrieveData(string tablename, string field, string condition)
        {
            SQLiteCommand query = new SQLiteCommand();
            query.CommandText = "SELECT * FROM " + tablename + " WHERE " + field + " = " + condition;
            DataTable returnedrows = _dbMgr.ExecuteRetrieveDataQuery(query);
            return returnedrows;
        }

        public void LogException(string errMsg)
        {
            SQLiteCommand errorlog = new SQLiteCommand();
            errorlog.CommandText = "INSERT INTO ErrorLog VALUES (null," + errMsg + ")";
            _dbMgr.ExecuteQuery(errorlog);
        }

        public void DeleteAll(string tablename)
        {
            SQLiteCommand query = new SQLiteCommand();
            query.CommandText = "DELETE FROM " + tablename;
            _dbMgr.ExecuteQuery(query);
        }

        private void CreateTable(string tablename, Dictionary<string, string> fieldsandtypes)
        {
            List<string> fieldswithtypes = new List<string>();

            SQLiteCommand query = new SQLiteCommand();
            query.CommandText = "CREATE TABLE IF NOT EXISTS (@fieldsandtypes)";
            foreach (var fieldname in fieldsandtypes.Keys)
            {
                fieldswithtypes.Add(fieldname + " " + fieldsandtypes[fieldname]);
            }
            string fields = String.Join(",", fieldswithtypes);
            query.CommandText = "CREATE TABLE IF NOT EXISTS " + tablename + " (" + fields + ")";
            _dbMgr.ExecuteQuery(query);
            //string createintstable = "CREATE TABLE IF NOT EXISTS Instruments (Id integer primary key autoincrement, GUID varchar(100), InstrumentName varchar(50), TimeUsed varchar(50), DateofLastService varchar(50))";
            //string createtimelog = "CREATE TABLE IF NOT EXISTS TimeLog (Id integer primary key autoincrement, DeviceID varchar(100),InstrumentName varchar(50), UserName varchar(50),DateUsed varchar(50),TimeOn varchar(50),TimeOff varchar(50),TimeUsed varchar(50))";        
        }

        public List<string> GetTableInfo(string tablename)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = "pragma table_info(" + tablename + ")";
            DataTable table_columns = _dbMgr.ExecuteRetrieveDataQuery(cmd);
            List<string> fields = new List<string>();
            foreach (DataColumn col in table_columns.Columns)
            {
                fields.Add(col.ToString());
            }
            return fields;
        }
    }
}
