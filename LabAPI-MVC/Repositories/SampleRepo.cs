using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class SampleRepo(SqlConnection connection)
{
    public async Task<List<Sample>> GetByReferral(Guid referralId)
    {
        var command = new SqlCommand(SqlQueries.GetReferralSamples, connection);
        command.Parameters.AddWithValue("@id", referralId);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return [];
        }
        
        List<Sample> samples = [];
        while (reader.Read())
        {
            var sample = new Sample
            {
                Id = reader.GetGuid(0),
                IssuedAt = reader.GetDateTime(1),
                BioCase = new BioCase()
                {
                    Id = reader.GetInt32(2),
                    Name = reader.GetSqlString(3).IsNull ? null : reader.GetSqlString(3).Value,
                    Description = reader.GetSqlString(4).IsNull ? null : reader.GetSqlString(4).Value,
                    BioContainer = new BioContainer
                    {
                        Id = reader.GetInt32(5),
                        Name = reader.GetSqlString(6).IsNull ? null : reader.GetSqlString(6).Value,
                        Description = reader.GetSqlString(7).IsNull ? null : reader.GetSqlString(7).Value,
                        Biomaterial = new Biomaterial
                        {
                            Id = reader.GetInt32(8),
                            Name = reader.GetSqlString(9).IsNull ? null : reader.GetSqlString(9).Value,
                            Description = reader.GetSqlString(10).IsNull ? null : reader.GetSqlString(10).Value
                        }
                    },
                    Supplier = new Supplier
                    {
                        Id = reader.GetInt32(11),
                        Name = reader.GetSqlString(12).IsNull ? null : reader.GetSqlString(12).Value,
                        Description = reader.GetSqlString(13).IsNull ? null : reader.GetSqlString(13).Value,
                    }
                }
            };
            samples.Add(sample);
        }
        await reader.CloseAsync();
        
        return samples;
    }
}