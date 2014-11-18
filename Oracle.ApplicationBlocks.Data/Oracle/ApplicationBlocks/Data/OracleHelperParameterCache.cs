namespace Oracle.ApplicationBlocks.Data
{
    using System;
    using System.Collections;
    using System.Data;
    using Oracle.DataAccess.Client;

    public sealed class OracleHelperParameterCache
    {
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        private OracleHelperParameterCache()
        {
        }

        public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            string str = connectionString + ":" + commandText;
            paramCache[str] = commandParameters;
        }

        private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            OracleParameter[] parameterArray = new OracleParameter[originalParameters.Length];

            int index = 0;
            int length = originalParameters.Length;

            while (index < length)
            {
                parameterArray[index] = (OracleParameter) originalParameters[index];

                index++;
            }
            return parameterArray;
        }

        private static OracleParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            OracleParameter[] parameterArray2;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(spName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    OracleCommandBuilder.DeriveParameters(command);
                    if (!includeReturnValueParameter && (ParameterDirection.ReturnValue == command.Parameters[0].Direction))
                    {
                        command.Parameters.RemoveAt(0);
                    }
                    OracleParameter[] array = new OracleParameter[command.Parameters.Count];
                    command.Parameters.CopyTo(array, 0);
                    parameterArray2 = array;
                }
            }
            return parameterArray2;
        }

        public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string str = connectionString + ":" + commandText;
            OracleParameter[] originalParameters = (OracleParameter[]) paramCache[str];
            if (originalParameters == null)
            {
                return null;
            }
            return CloneParameters(originalParameters);
        }

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string str = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
            OracleParameter[] originalParameters = (OracleParameter[]) paramCache[str];
            if (originalParameters == null)
            {
                object obj2;
                paramCache[str] = obj2 = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter);
                originalParameters = (OracleParameter[]) obj2;
            }
            return CloneParameters(originalParameters);
        }
    }
}

