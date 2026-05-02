using System.Data;
using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class ReferralRepo(SqlConnection connection)
{
    public async Task<Referral?> Get(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.GetReferralBase, connection);
        command.Parameters.AddWithValue("@id", guid);
        var reader = await command.ExecuteReaderAsync();
        
        if (!reader.HasRows)
        {
            return null;
        }
        
        Referral? referral = null;
        while (reader.Read())
        {
            if (referral != null) continue;
            referral = new Referral
            {
                Id = reader.GetGuid(0),
                Patient = new Patient{Id = reader.GetSqlGuid(1).IsNull ? Guid.Empty : reader.GetSqlGuid(1).Value},
                IssuedAt = reader.GetDateTime(2),
                Weight = reader.GetSqlDecimal(3).IsNull ? null : reader.GetSqlDecimal(3).Value,
                Height = reader.GetSqlDecimal(4).IsNull ? null : reader.GetSqlDecimal(4).Value,
                Sex = reader.GetSqlInt32(5).IsNull ? null : reader.GetSqlInt32(5).Value,
                Tests = [],
                Samples = []
            };
        }
        await reader.CloseAsync();
        
        return referral;
    }

    public async Task<List<Test>> GetTests(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.GetReferralTests, connection);
        command.Parameters.AddWithValue("@id", guid);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return [];
        }
        
        List<Test> tests = [];
        while (reader.Read())
        {
            var test = new Test
            {
                Id = reader.GetInt32(0),
                Name = reader.GetSqlString(1).IsNull ? null : reader.GetSqlString(1).Value,
                Description = reader.GetSqlString(2).IsNull ? null : reader.GetSqlString(2).Value,
                Biomaterial = new Biomaterial
                {
                    Id = reader.GetInt32(3),
                    Name = reader.GetSqlString(4).IsNull ? null : reader.GetSqlString(4).Value,
                    Description = reader.GetSqlString(5).IsNull ? null : reader.GetSqlString(5).Value
                }
            };
            tests.Add(test);
        }
        await reader.CloseAsync();
        
        return tests;
    }

    public async Task<bool> Create(Referral referral)
    {
        var command = new SqlCommand(SqlQueries.CreatePatient, connection);
 
        command.Parameters.AddWithValue("@id", referral.Id);
        command.Parameters.AddWithValue("@issued", referral.IssuedAt);
        command.Parameters.AddWithValue("@patient",  referral.Patient?.Id ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@weight", referral.Weight ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@height", referral.Height ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sex", referral.Sex ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> IsExists(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.IsReferralExists, connection);
        command.Parameters.AddWithValue("@id", guid);
        var isReferralExists = await command.ExecuteScalarAsync();
        
        if (isReferralExists == null) return false;
        return (int)isReferralExists == 0;
    }

    public async Task<bool> SetPatient(Guid referral, Guid? patient)
    {
        var command = new SqlCommand(SqlQueries.SetPatientReferral, connection);
        command.Parameters.AddWithValue("@id", referral);
        command.Parameters.AddWithValue("@patient",  patient ?? (object)DBNull.Value);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> LinkTest(Guid referral, int test)
    {
        try
        {
            var command = new SqlCommand(SqlQueries.LinkReferralTest, connection);
            command.Parameters.AddWithValue("@id", referral);
            command.Parameters.AddWithValue("@test", test);

            var affected = await command.ExecuteNonQueryAsync();
            return affected > 0;
        }
        catch (ConstraintException)
        {
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UnlinkTest(Guid referral, int test)
    {
        var command = new SqlCommand(SqlQueries.UnlinkReferralTest, connection);
        command.Parameters.AddWithValue("@id", referral);
        command.Parameters.AddWithValue("@test", test);

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> UpdateReferral(Referral referral)
    {
        var command = new SqlCommand(SqlQueries.UpdateReferral, connection);
        command.Parameters.AddWithValue("@id", referral.Id);
        command.Parameters.AddWithValue("@weight", referral.Weight ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@height", referral.Height ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sex", referral.Sex ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
}
