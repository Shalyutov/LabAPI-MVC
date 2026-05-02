using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class PatientRepo(SqlConnection connection)
{
    public async Task<Patient?> Get(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.GetPatientBase, connection);
        command.Parameters.AddWithValue("@id", guid);
        var reader = await command.ExecuteReaderAsync();
        
        if (!reader.HasRows)
        {
            return null;
        }
        
        Patient? patient = null;
        while (reader.Read())
        {
            if (patient != null) continue;
            patient = new Patient
            {
                Id = reader.GetGuid(0),
                FullName = reader.GetString(1),
                BirthDate = DateOnly.FromDateTime(reader.GetDateTime(2)),
                Document = reader.GetString(3),
                Phone = reader.GetString(4),
                Email = reader.GetString(5)
            };
        }
        await reader.CloseAsync();

        return patient;
    }

    public async Task<bool> IsExist(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.IsPatientExists, connection);
        command.Parameters.AddWithValue("@id", guid);
        var isExists = await command.ExecuteScalarAsync();
        if (isExists == null) return false;
        return (int)isExists > 0;
    }
}