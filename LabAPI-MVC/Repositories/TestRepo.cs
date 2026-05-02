using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class TestRepo(SqlConnection connection)
{
    public async Task<bool> IsTestExists(int id)
    {
        var command = new SqlCommand(SqlQueries.IsTestExists, connection);
        command.Parameters.AddWithValue("@id", id);
        var isTestExists = await command.ExecuteScalarAsync();
        
        if (isTestExists == null) return false;
        return (int)isTestExists > 0;
    }
}