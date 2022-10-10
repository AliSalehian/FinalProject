using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;

namespace jf_FinalProject.Logic
{
    class DataBaseConnection
    {
        Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"connection", "Server=localhost;Integrated security=SSPI;database=master"},
            {"create database","CREATE DATABASE IF NOT EXISITS dynamomtere-testdata" },
            {"create test table", "CREATE TABLE if not exists test (  test_number INT NOT NULL AUTO_INCREMENT,  date VARCHAR(45) NULL, name VARCHAR(45) NULL,  PRIMARY KEY (test_number));" },
            {"create result table", "CREATE TABLE if is not exists dynamometer.ressult (" +
                              "test_number INT NULL,Mmax VARCHAR(45) NULL," +
                              "Mmin VARCHAR(45) NULL," +
                              "µmax VARCHAR(45) NULL,µmin VARCHAR(45) NULL" +
                              ",µ VARCHAR(45) NULL" +
                              ",t3 VARCHAR(45) NULL" +
                              ",t3b VARCHAR(45) NULL," +
                              "Fmax VARCHAR(45) NULL," +
                              "F VARCHAR(45) NULL," +
                              "Sv VARCHAR(45) NULL," +
                              "Sc VARCHAR(45) NULL," +
                              "s VARCHAR(45) NULL," +
                              "a(-)MFDD` VARCHAR(45) NULL," +
                              "Ws VARCHAR(45) NULL," +
                              "Wj VARCHAR(45) NULL," +
                              "W % VARCHAR(45) NULL," +
                              "CONSTRAINT `test_number`" +
                              "FOREIGN KEY(`test_number`)" +
                              "REFERENCES `dynamometer`.`test` (`test_number`)" +
                              "ON DELETE NO CASCADE" +
                              "ON UPDATE NO CASCADE)"},
            {"show tests", "SELECT * FROM dynamometer.test ORDER BY test_number DEC LIMIT 10" },
            {"show result of test ", "write it here"},
        };
        public DataBaseConnection()
        {
            SqlDataReader result;
            int rowAffected;
            SqlConnection myConn = new SqlConnection(queries["connection"]);
            //if database does not exists this command will make it
            SqlCommand myCommand = new SqlCommand(queries["mcreate database"], myConn);
            try
            {
                myConn.Open();
                rowAffected = myCommand.ExecuteNonQuery();
                result = myCommand.ExecuteReader();
                MessageBox.Show(result.Read().ToString(), "DYNAMOMETER");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DYNAMOMETER");
            }

            // making information of test table if is not exists
            myCommand = new SqlCommand(queries["create test table"], myConn);
            try
            {
                myConn.Open();
                rowAffected = myCommand.ExecuteNonQuery();
                result = myCommand.ExecuteReader();
                MessageBox.Show(result.Read().ToString(), "DYNAMOMETER");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DYNAMOMETER");
            }
            //making test result table if is not exists
            myCommand = new SqlCommand(queries["create result table"], myConn);
            try
            {
                myConn.Open();
                rowAffected = myCommand.ExecuteNonQuery();
                result = myCommand.ExecuteReader();
                result.Read();
                MessageBox.Show(result.ToString(), "DYNAMOMETER");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DYNAMOMETER");
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }
        public void dataBaseInstertion(string query)
        {
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            try
            {
                SqlConnection myConn = new SqlConnection(queries["connection"]);
                myConn.Open();
                sqlAdapter.InsertCommand = new SqlCommand(queries[query], myConn);
                sqlAdapter.InsertCommand.ExecuteNonQuery();
                MessageBox.Show("Row inserted !! ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public SqlDataReader dataBaseQuery(string query)

        {
            SqlDataReader result;
            SqlConnection myConn = new SqlConnection(queries["connection"]);
            if (queries.ContainsKey(query))
            {
                query = queries[query];
            }
            SqlCommand myCommand = new SqlCommand(query, myConn);
            myConn.Open();
            result = myCommand.ExecuteReader();
            return result;
        }
    }
}
