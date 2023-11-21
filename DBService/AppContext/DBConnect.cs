using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace System
{
    // Encryption decryption unit
    public static class Encryption
    {
        // It encrypts string into md5 string 
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}

namespace Classes
{
    public class DBConnect : IDisposable
    {
        private string server;
        // private string database;
        // private string database_type;
        public string database;
        public DBType database_type;
        private string uid;
        private string password;
        public SqlConnection connection;
        private SqlDataAdapter adp;
        public MySqlConnection myconnection;
        private MySqlDataAdapter myadp;
        private DataSet dtset;
        string dbType;

        public string connectionString;

        /// <summary>
        /// TRUE when db created or checked, FALSE when not checked or created
        /// </summary>
        public static bool bIsDBCreated = false;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DBConnect> _logger; 
        //Constructor
        public DBConnect(IConfiguration configuration, ILogger<DBConnect> logger)
        {
            string strError = "";
            _configuration = configuration;
            _logger = logger;

            try
            {
                Initialize();

            }
            catch (Exception excp)
            {
                strError = excp.Message;
            }
        }

        //Initialize values
        private void Initialize()
        {
            var section = _configuration.GetSection("ConnDetails");

            server = section["DBServerIP"].ToString();//   "WIN-T04S48MMAPI"; //IP where Database is present

            //uid = "lipi";
            uid = section["UserId"].ToString();
            //password = "L!p!d@t@";
            password = section["Password"].ToString();

            database = section["Database"].ToString(); //"kmsdatabase";


            //database_type = ConfigurationManager.AppSettings["DataBaseType"].ToString();
            dbType = section["DataBaseType"].ToString();
            if (dbType == "mssql")
            {
                database_type = DBType.mssql;
            }
            else
            {
                database_type = DBType.mysql;
            }

            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";


            _logger.LogInformation(connectionString, "ServiceDBLogs");

            //MessageBox.Show("database initialize");
            if (database_type == DBType.mssql)
                connection = new SqlConnection(connectionString);
            else
                myconnection = new MySqlConnection(connectionString);




        }

        public string DatabaseName
        {
            get { return database; }
        }



        //open connection to database
        private bool OpenConnection(out string strError)
        {
            strError = "";
            try
            {
                if (database_type == DBType.mssql)
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        _logger.LogInformation("try to open connection", "ServiceDBLogs");
                        connection.Open();
                        _logger.LogInformation("Connection Opended ", "ServiceDBLogs");
                        //ThreadSafe.WriteFile("S", "Database connection :", "Connection Open", "MainFolder");
                    }
                    return true;
                }
                else
                {
                    if (myconnection.State == ConnectionState.Closed)
                    {
                        _logger.LogInformation("try to open connection", "ServiceDBLogs");
                        myconnection.Open();
                        _logger.LogInformation("Connection Opended ", "ServiceDBLogs");
                        //ThreadSafe.WriteFile("S", "Database connection :", "Connection Open", "MainFolder");
                    }
                    return true;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogInformation("error in connection open", "ServiceDBLogs");
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                //switch (ex.Number)
                //{
                //    case 0:
                //        MessageBox.Show("Cannot connect to server.  Contact administrator");
                //        //ThreadSafe.WriteFile("Cannot connect to server");

                //        break;

                //    case 1045:
                //        MessageBox.Show("Invalid username/password, please try again");
                //        //ThreadSafe.WriteFile("Invalid username/password for database connectivity");
                //        break;

                //    //added ankush for the server not present
                //    case 1042:
                //        MessageBox.Show("Server Not Present for connection");
                //        //ThreadSafe.WriteFile("Server Not Presented for connection");
                //        break;
                //    default:
                //        ThreadSafe.WriteFile("S", "Excp -> Database connection :", "Connection Open", "MainFolder");
                //        break;

                //}
                strError = ex.Message;
                return false;
            }
        }

        //Close connection
        private bool CloseConnection(out string strError)
        {
            strError = "";
            try
            {
                if (database_type == DBType.mssql)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        _logger.LogInformation("try to close connection", "ServiceDBLogs");
                        connection.Close();
                        //ThreadSafe.WriteFile("S", "Database connection :", "Connection Close", "MainFolder");
                    }
                    return true;
                }
                else
                {
                    if (myconnection.State == ConnectionState.Open)
                    {
                        _logger.LogInformation("try to close connection", "ServiceDBLogs");
                        myconnection.Close();
                        _logger.LogInformation(" connection Closed", "ServiceDBLogs");
                        //ThreadSafe.WriteFile("S", "Database connection :", "Connection Close", "MainFolder");
                    }
                    return true;
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.Message);
                //ThreadSafe.WriteFile("Exception DBConnect : CloseConnection()", "", "", "MainFolder");
                _logger.LogInformation("errer in close connection", "ServiceDBLogs");
                strError = ex.Message;
                return false;
            }
        }

        //Select statement
        public bool Select(string query, out DataSet ds, out string strError)
        {
            //string query = "SELECT * FROM tableinfo";
            ds = null;
            strError = "";

            try
            {
                if (this.OpenConnection(out strError))
                {
                    if (database_type == DBType.mssql)
                    {
                        adp = new SqlDataAdapter(query, connection);
                        //   adp.SelectCommand.CommandTimeout = 180;
                        dtset = new DataSet();
                        adp.Fill(dtset);
                        ds = dtset;


                        //ThreadSafe.WriteFile("S","Query :","Select Query :[" +query+ "]","MainFolder" );
                        if (this.CloseConnection(out strError))
                            return true;
                    }
                    else
                    {
                        myadp = new MySqlDataAdapter(query, myconnection);
                        //   adp.SelectCommand.CommandTimeout = 180;
                        dtset = new DataSet();
                        myadp.Fill(dtset);
                        ds = dtset;


                        //ThreadSafe.WriteFile("S","Query :","Select Query :[" +query+ "]","MainFolder" );
                        if (this.CloseConnection(out strError))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception excp)
            {
                //ThreadSafe.WriteFile("Exception in Select query" + excp.ToString(), "", "", "MainFolder");
                //string msg = excp.ToString();

                strError = excp.Message;
                return false;
            }
        }
        //Insert statement
        public bool Insert(string query, out string strError)
        {
            //string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";
            strError = "";
            try
            {
                //open connection
                if (this.OpenConnection(out strError))
                {
                    //create command and assign the query and connection from the constructor
                    int status;
                    if (database_type == DBType.mssql)
                    {
                        SqlCommand cmd = new SqlCommand(query, connection);
                        status = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand(query, myconnection);
                        status = cmd.ExecuteNonQuery();
                    }
                    //MySqlDataReader dataReader = cmd.ExecuteReader();


                    //Execute command

                    //if (status <= 0)
                    //ThreadSafe.WriteFile("S", "Query :", "Insert Query :Unsuccessful Execution [" + query + "]", "MainFolder");
                    //else
                    //     ThreadSafe.WriteFile("S", "Query :", "Insert Query :Unsuccessful Execution [" + query + "]", "MainFolder");


                    //close connection
                    if (this.CloseConnection(out strError) && status != 0)
                        return true;
                }
                return false;
            }
            catch (SqlException ex)
            {
                //ThreadSafe.WriteFile("Exception DBCOnnect : Insert()");
                //MessageBox.Show("insert dbconnect " + ex.Message);
                strError = ex.Message;
                return false;
            }

            //catch { return false; }
        }

        public bool MultiInsertRec(string[] query, out string strError)
        {
            //string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";
            strError = "";
            try
            {
                //open connection
                if (this.OpenConnection(out strError))
                {
                    //create command and assign the query and connection from the constructor
                    int status = 0;
                    if (database_type == DBType.mssql)
                    {
                        for (int i = 0; i < query.Length; i++)
                        {
                            SqlCommand cmd = new SqlCommand(query[i], connection);
                            status = cmd.ExecuteNonQuery();
                        }

                    }
                    else
                    {
                        for (int i = 0; i < query.Length; i++)
                        {
                            MySqlCommand cmd = new MySqlCommand(query[i], myconnection);
                            status = cmd.ExecuteNonQuery();
                        }

                    }
                    //MySqlDataReader dataReader = cmd.ExecuteReader();


                    //Execute command

                    //if (status <= 0)
                    //ThreadSafe.WriteFile("S", "Query :", "Insert Query :Unsuccessful Execution [" + query + "]", "MainFolder");
                    //else
                    //     ThreadSafe.WriteFile("S", "Query :", "Insert Query :Unsuccessful Execution [" + query + "]", "MainFolder");


                    //close connection
                    if (this.CloseConnection(out strError) && status != 0)
                        return true;
                }
                return false;
            }
            catch (SqlException ex)
            {
                //ThreadSafe.WriteFile("Exception DBCOnnect : Insert()");
                //MessageBox.Show("insert dbconnect " + ex.Message);
                strError = ex.Message;
                return false;
            }

            //catch { return false; }
        }

        //Update statement
        public bool Update(string query, out string strError)
        {
            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
            strError = "";
            try
            {
                //Open connection
                if (this.OpenConnection(out strError))
                {
                    if (database_type == DBType.mssql)
                    {
                        //create mysql command
                        SqlCommand cmd = new SqlCommand();
                        //Assign the query using CommandText
                        cmd.CommandText = query;
                        //Assign the connection using Connection
                        cmd.Connection = connection;

                        //Execute query
                        int status = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        //create mysql command
                        MySqlCommand cmd = new MySqlCommand();
                        //Assign the query using CommandText
                        cmd.CommandText = query;
                        //Assign the connection using Connection
                        cmd.Connection = myconnection;

                        //Execute query
                        int status = cmd.ExecuteNonQuery();
                    }
                    //ThreadSafe.WriteFile("Updated query executed successfully " + "[ " + query + " ]");
                    //if (status <= 0)
                    //ThreadSafe.WriteFile("S", "Query :", "Update Query :Executed with no changes [" + query + "]", "MainFolder");
                    //else
                    //ThreadSafe.WriteFile("S", "Query :", "Update Query :Executed with no changes [" + query + "]", "MainFolder");

                    //close connection
                    if (this.CloseConnection(out strError))
                        return true;
                }
                return false;
            }
            catch (Exception excp)
            {
                //ThreadSafe.WriteFile("Exception executing Update query" + excp.ToString(), "", "", "MainFolder");
                //string msg = excp.ToString();
                strError = excp.Message;
                return false;
            }
            //catch { return false; }
        }

        //Delete statement
        public bool Delete(string query, out string strError)
        {
            //string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            strError = "";
            try
            {
                if (this.OpenConnection(out strError))
                {
                    if (database_type == DBType.mssql)
                    {
                        SqlCommand cmd = new SqlCommand(query, connection);
                        int status = cmd.ExecuteNonQuery();
                        //ThreadSafe.WriteFile("Delete query executed successfully " + "[ " + query + " ]");
                        //if (status <= 0)
                        //ThreadSafe.WriteFile("S", "Query :", "Delete Query :Executed with no changes [" + query + "]", "MainFolder");
                        //else
                        //ThreadSafe.WriteFile("S", "Query :", "Delete Query :Executed with no changes [" + query + "]", "MainFolder");

                        if (this.CloseConnection(out strError))
                            return true;
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand(query, myconnection);
                        int status = cmd.ExecuteNonQuery();
                        if (this.CloseConnection(out strError))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                //ThreadSafe.WriteFile("Exception in Delete query " + ex.ToString(), "", "", "MainFolder");
                return false;
            }
        }

        public bool InsertParameterized(string query, List<string> data, List<byte[]> data1, out string strError)
        {
            try
            {
                strError = "";
                //open connection
                if (this.OpenConnection(out strError))
                {
                    //create command and assign the query and connection from the constructor
                    if (database_type == DBType.mssql)
                    {
                        int status;
                        SqlCommand cmd = new SqlCommand(query, connection);

                        for (int i = 0, j = 0; i < data.Count; i += 2)
                        {
                            cmd.Parameters.AddWithValue(data[j], data[i + 1]);
                            j = j + 2;
                        }


                        //for (int i = 0; i < data.Count; i++)
                        //{
                        //    cmd.Parameters.AddWithValue(data[i], data1[i]);
                        //}


                        adp = new SqlDataAdapter(query, connection);
                        adp.InsertCommand = cmd;
                        status = cmd.ExecuteNonQuery();

                        //close connection
                        if (this.CloseConnection(out strError) && status != 0)
                            return true;
                    }
                    else
                    {
                        int status;
                        MySqlCommand cmd = new MySqlCommand(query, myconnection);
                        //for (int i = 0, j = 0; i < data.Count; i += 2)
                        //{
                        //    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
                        //    j = j + 2;
                        //}

                        for (int i = 0; i < data.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(data[i], data1[i]);
                        }

                        myadp = new MySqlDataAdapter(query, myconnection);
                        myadp.InsertCommand = cmd;
                        status = cmd.ExecuteNonQuery();
                        if (this.CloseConnection(out strError) && status != 0)
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //ThreadSafe.WriteFile("Exception DBCOnnect : Insert()");
                //MessageBox.Show("insert dbconnect " + ex.Message);
                strError = ex.Message;
                return false;
            }

            //catch { return false; }
        }

        public bool UpdateParameterized(string query, List<string> data, List<byte[]> data1, out string strError)
        {
            strError = "";
            try
            {

                //Open connection
                if (this.OpenConnection(out strError))
                {

                    //create mysql command
                    if (database_type == DBType.mssql)
                    {
                        SqlCommand cmd = new SqlCommand(query, connection);

                        for (int i = 0, j = 0; i < data.Count; i += 2)
                        {
                            cmd.Parameters.AddWithValue(data[j], data[i + 1]);
                            j = j + 2;
                        }

                        adp = new SqlDataAdapter(query, connection);
                        adp.UpdateCommand = cmd;
                        int status = cmd.ExecuteNonQuery();
                        if (this.CloseConnection(out strError))
                            return true;
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand(query, myconnection);

                        //for (int i = 0, j = 0; i < data.Count; i += 2)
                        //{
                        //    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
                        //    j = j + 2;
                        //}

                        for (int i = 0; i < data.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(data[i], data1[i]);
                        }



                        myadp = new MySqlDataAdapter(query, myconnection);
                        myadp.UpdateCommand = cmd;
                        int status = cmd.ExecuteNonQuery();


                        //Execute query
                        //int status = cmd.ExecuteNonQuery();

                        //close connection
                        if (this.CloseConnection(out strError))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception excp)
            {
                return false;
            }
            //catch { return false; }
        }

        public bool Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
                return true;
            }
            catch (IOException ex)
            {
                //ThreadSafe.WriteFile("Exception DBConnect: Backup() ");
                //MessageBox.Show("Error , unable to backup!");
                return false;
            }
        }
        //public void MultiInsertRec(string[] query, out bool hasDataSaved, out string Error)
        //{
        //    //Error = "";
        //    //hasDataSaved = false;
        //    //if (this.OpenConnection(out Error))
        //    //    //this./*CloseConnection = this.OpenConnection();*/

        //    //    int rowsAffected = 0;
        //    //for (int i = 0; i < query.Length; i++)
        //    //{
        //    //    if (!string.IsNullOrEmpty(query[i]))
        //    //    {
        //    //        SqlCommand sqlCommand = new SqlCommand(query[i], sqlConnection);
        //    //        rowsAffected = sqlCommand.ExecuteNonQuery();
        //    //    }
        //    //}
        //    //if (rowsAffected > 0)
        //    //{
        //    //    hasDataSaved = true;
        //    //    Error = "";
        //    //}
        //    //else
        //    //{
        //    //    Error = "Records couldn't be saved...";
        //    //    hasDataSaved = false;
        //    //}

        //    //this.closeConnection();

        //    //try
        //    //{
        //    //    strError = "";
        //    //    //open connection
        //    //    if (this.OpenConnection(out strError))
        //    //    {
        //    //        //create command and assign the query and connection from the constructor
        //    //        if (database_type == DBType.mssql)
        //    //        {
        //    //            int status;
        //    //            SqlCommand cmd = new SqlCommand(query, connection);

        //    //            for (int i = 0, j = 0; i < data.Count; i += 2)
        //    //            {
        //    //                cmd.Parameters.AddWithValue(data[j], data[i + 1]);
        //    //                j = j + 2;
        //    //            }


        //    //            //for (int i = 0; i < data.Count; i++)
        //    //            //{
        //    //            //    cmd.Parameters.AddWithValue(data[i], data1[i]);
        //    //            //}


        //    //            adp = new SqlDataAdapter(query, connection);
        //    //            adp.InsertCommand = cmd;
        //    //            status = cmd.ExecuteNonQuery();

        //    //            //close connection
        //    //            if (this.CloseConnection(out strError) && status != 0)
        //    //                return true;
        //    //        }
        //    //        else
        //    //        {
        //    //            int status;
        //    //            MySqlCommand cmd = new MySqlCommand(query, myconnection);
        //    //            //for (int i = 0, j = 0; i < data.Count; i += 2)
        //    //            //{
        //    //            //    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
        //    //            //    j = j + 2;
        //    //            //}

        //    //            for (int i = 0; i < data.Count; i++)
        //    //            {
        //    //                cmd.Parameters.AddWithValue(data[i], data1[i]);
        //    //            }

        //    //            myadp = new MySqlDataAdapter(query, myconnection);
        //    //            myadp.InsertCommand = cmd;
        //    //            status = cmd.ExecuteNonQuery();
        //    //            if (this.CloseConnection(out strError) && status != 0)
        //    //                return true;
        //    //        }
        //    //    }
        //    //    return false;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    //ThreadSafe.WriteFile("Exception DBCOnnect : Insert()");
        //    //    //MessageBox.Show("insert dbconnect " + ex.Message);
        //    //    strError = ex.Message;
        //    //    return false;
        //    //}


        //}
        //Restore
        public bool Restore(string path)
        {
            try
            {
                //Read file from C:\
                //string path;
                //path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                return true;
            }
            catch (IOException ex)
            {
                //ThreadSafe.WriteFile("Exception DBConnect: Restore() " );
                //MessageBox.Show("Error , unable to Restore!");
                return false;
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            connection = null;
            myconnection = null;
            server = "";
            database = "";
            uid = "";
            password = "";
            adp = null;
            dtset = null;
            GC.Collect();

            //throw new NotImplementedException();
        }

        #endregion

    }

    public enum DBType
    {
        mssql = 1,
        mysql = 2,
        oracle = 3
    }
}