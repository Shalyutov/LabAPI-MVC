using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class BioCaseRepo(SqlConnection connection)
{
    public async Task<bool> IsExists(int id)
    {
        var command = new SqlCommand(SqlQueries.IsBioCaseExists, connection);
        command.Parameters.AddWithValue("@bio_case_id", id);
        var isExists = await command.ExecuteScalarAsync();
        
        if (isExists == null) return false;
        return (int)isExists == 0;
    }
}