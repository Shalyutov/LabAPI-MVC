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

    public async Task<bool> Create(Patient patient)
    {
        var command = new SqlCommand(SqlQueries.CreatePatient, connection);
        command.Parameters.AddWithValue("@id", patient.Id);
        command.Parameters.AddWithValue("@fullname", patient.FullName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@birthdate", patient.BirthDate ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@document", patient.Document ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@email", patient.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@phone", patient.Phone ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    public async Task<bool> Update(Patient patient)
    {
        var command = new SqlCommand(SqlQueries.UpdatePatient, connection);
        command.Parameters.AddWithValue("@id", patient.Id);
        command.Parameters.AddWithValue("@fullname", patient.FullName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@birthdate", patient.BirthDate ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@document", patient.Document ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@email", patient.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@phone", patient.Phone ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    public async Task<bool> Delete(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.DeletePatient, connection);
        command.Parameters.AddWithValue("@id", guid);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
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