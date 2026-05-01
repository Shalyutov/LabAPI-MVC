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
                Sex = reader.GetSqlInt32(5).IsNull ? null : reader.GetSqlInt32(5).Value
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
}
