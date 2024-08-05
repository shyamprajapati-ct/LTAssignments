using System.Data;
using System.Data.SqlClient;

namespace LTAssignment.Models.DBConnection
{
    public class Common
    {
        
            private readonly string _connectionString;

            public Common(string connectionString)
            {
                _connectionString = connectionString;
            }
 
            public (DataSet dataSet, bool success, string message) ExecuteStoreProcedure(dynamic sqlParameters, string procedureName, int opCode)
            {
                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new(procedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 3000;
                cmd.Parameters.AddWithValue("@opCode", opCode);
                AddParameters(sqlParameters, cmd);

                using SqlDataAdapter sda = new(cmd);
                DataSet ds = new();
                try
                {
                    sda.Fill(ds);
                    return (ds, true, "Operation successful");
                }
                catch (Exception ex)
                {
                    return (ds, false, ex.Message);
                }

                static void AddParameters(dynamic SqlParameters, SqlCommand cmd)
                {
                    var SqlParametersPoperties = SqlParameters.GetType().GetProperties();

                    foreach (var SqlParameterProperty in SqlParametersPoperties)
                    {
                        cmd.Parameters.AddWithValue(string.Concat("@", SqlParameterProperty.Name), SqlParameterProperty.GetValue(SqlParameters));
                    }
                }
            }
        }
}
