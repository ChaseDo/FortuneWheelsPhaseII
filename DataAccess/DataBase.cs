using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace DataAccess
{
    public class DataBase
    {
        private string connectString = null;

        private string GetConnectionString()
        {
            return WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        public  DataTable ExcuteSqlReturnDataTable(string sql, SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (SqlParameter sqlParameter in sqlParameters)
                    {
                        if (sqlParameter.Value != null)
                        {
                            if (sqlParameter.Value is string)
                            {
                                if (string.IsNullOrEmpty((string)sqlParameter.Value))
                                {
                                    sqlParameter.Value = null;
                                }
                            }
                        }
                        if (sqlParameter.Value == null)
                        {
                            sqlParameter.Value = (object)DBNull.Value;
                        }
                        command.Parameters.Add(sqlParameter);
                    }
                    SqlDataAdapter sa = new SqlDataAdapter();
                    sa.SelectCommand = command;
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }

        public DataTable ExcuteSqlReturnDataTableTransaction(string sql, SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                    {
                        foreach (SqlParameter sqlParameter in sqlParameters)
                        {
                            if (sqlParameter.Value != null)
                            {
                                if (sqlParameter.Value is string)
                                {
                                    if (string.IsNullOrEmpty((string)sqlParameter.Value))
                                    {
                                        sqlParameter.Value = null;
                                    }
                                }
                            }
                            if (sqlParameter.Value == null)
                            {
                                sqlParameter.Value = (object)DBNull.Value;
                            }
                            command.Parameters.Add(sqlParameter);
                        }
                        SqlDataAdapter sa = new SqlDataAdapter();
                        sa.SelectCommand = command;
                        DataSet ds = new DataSet();
                        sa.Fill(ds);
                        transaction.Commit();
                        return ds.Tables[0];
                    }
                }
                catch
                {
                    transaction.Rollback();
                    return new DataTable();
                }
            }
        }

        public  DataTable ExcuteSqlReturnDataTable(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        SqlDataAdapter sa = new SqlDataAdapter();
                        sa.SelectCommand = command;
                        DataSet ds = new DataSet();
                        sa.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// excute sql with parameters and returns the number of affected rows 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns>returns the number of affected rows </returns>
        public  int ExcuteSqlReturnInt(string sql, SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (SqlParameter sqlParameter in sqlParameters)
                    {
                        if (sqlParameter.Value != null)
                        {
                            if (sqlParameter.Value is string)
                            {
                                if (string.IsNullOrEmpty((string)sqlParameter.Value))
                                {
                                    sqlParameter.Value = null;
                                }
                            }
                        }
                        if (sqlParameter.Value == null)
                        {
                            sqlParameter.Value = (object)DBNull.Value;
                        }
                        command.Parameters.Add(sqlParameter);
                    }
                    int result = command.ExecuteNonQuery();
                    return result;
                }
            }
        }

        /// <summary>
        /// excute sql without parameters and returns the number of affected rows 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>returns the number of affected rows </returns>
        public  int ExcuteSqlReturnInt(string sql)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    int result = command.ExecuteNonQuery();
                    return result;
                }
            }
        }

    }
}
