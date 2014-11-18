namespace Oracle.ApplicationBlocks.Data
{
    using System;
    using System.Data;
    using Oracle.DataAccess.Client;

    public sealed class OracleHelper
    {
        private OracleHelper()
        {
        }

        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters != null) && (parameterValues != null))
            {
                if (commandParameters.Length != parameterValues.Length)
                {
                    throw new ArgumentException("Parameter count does not match Parameter Value count.");
                }
                int index = 0;
                int length = commandParameters.Length;
                while (index < length)
                {
                    commandParameters[index].Value = parameterValues[index];
                    index++;
                }
            }
        }

        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter parameter in commandParameters)
            {
                //if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
                //{
                //    parameter.Value = DBNull.Value;
                //}

                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }

        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, null);
        }

        //public static DataSet ExecuteDataset(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        //}

        //public static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    return ExecuteDataset(transaction, commandType, commandText, null);
        //}

        //public static DataSet ExecuteDataset(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        //}

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, null);
        }

        //public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        //}

        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand command = new OracleCommand();

            using (connection)
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParameters);

                OracleDataAdapter adapter = new OracleDataAdapter(command);

                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet);

                return dataSet;
            }
        }

        //public static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    OracleCommand command = new OracleCommand();

        //    PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);

        //    OracleDataAdapter adapter = new OracleDataAdapter(command);

        //    DataSet dataSet = new DataSet();

        //    adapter.Fill(dataSet);

        //    return dataSet;
        //}

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);

            connection.Open();

            return ExecuteDataset(connection, commandType, commandText, commandParameters);
        }

        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }

        //public static int ExecuteNonQuery(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        //}

        //public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    return ExecuteNonQuery(transaction, commandType, commandText, null);
        //}

        //public static int ExecuteNonQuery(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        //}

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        //public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
        //}

        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand command = new OracleCommand();

            using (connection)
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParameters);

                return command.ExecuteNonQuery();
            }
        }

        //public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    OracleCommand command = new OracleCommand();
        //    PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
        //    return command.ExecuteNonQuery();
        //}

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);
           
            connection.Open();

            return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
        }

        //public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText)
        //{
        //    return ExecuteReader(connection, commandType, commandText, null);
        //}

        //public static OracleDataReader ExecuteReader(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteReader(connection, CommandType.StoredProcedure, spName);
        //}

        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    return ExecuteReader(transaction, commandType, commandText, null);
        //}

        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        //}

        //public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        //{
        //    return ExecuteReader(connectionString, commandType, commandText, null);
        //}

        //public static OracleDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
        //}

        //public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    return ExecuteReader(connection, null, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        //}

        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        //}

        //public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    OracleConnection connection = new OracleConnection(connectionString);

        //    connection.Open();

        //    OracleDataReader reader = ExecuteReader(connection, null, commandType, commandText, commandParameters, OracleConnectionOwnership.Internal);
 
        //    return reader;
        //}

        //private static OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, OracleConnectionOwnership connectionOwnership)
        //{
        //    OracleCommand command = new OracleCommand();

        //    PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);

        //    if (connectionOwnership == OracleConnectionOwnership.External)
        //    {
        //        return command.ExecuteReader();
        //    }

        //    return command.ExecuteReader(CommandBehavior.CloseConnection);
        //}

        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, null);
        }

        //public static object ExecuteScalar(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        //}

        //public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    return ExecuteScalar(transaction, commandType, commandText, null);
        //}

        //public static object ExecuteScalar(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
        //    }
        //    return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        //}

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, null);
        }

        //public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        //{
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] spParameterSet = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
        //        AssignParameterValues(spParameterSet, parameterValues);
        //        return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
        //    }

        //    return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
        //}

        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            using (connection)
            {

                OracleCommand command = new OracleCommand();

                PrepareCommand(command, connection, null, commandType, commandText, commandParameters);

                return command.ExecuteScalar();
            }
        }

        //public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    OracleCommand command = new OracleCommand();
        //    PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
        //    return command.ExecuteScalar();
        //}

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);

            connection.Open();

            return ExecuteScalar(connection, commandType, commandText, commandParameters);
        }

        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private enum OracleConnectionOwnership
        {
            Internal,
            External
        }
    }
}

