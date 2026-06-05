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
                Referral = new Referral{Id = reader.GetGuid(14)},
                BioCase = new BioCase
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
    
    public async Task<Sample?> GetById(Guid id)
    {
        var command = new SqlCommand(SqlQueries.GetSample, connection);
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return null;
        }
        
        Sample? sample = null;
        while (reader.Read())
        {
            if (sample != null) continue;
            sample = new Sample
            {
                Id = reader.GetGuid(0),
                IssuedAt = reader.GetDateTime(1),
                Referral = new Referral{Id = reader.GetGuid(14)},
                BioCase = new BioCase
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
        }
        await reader.CloseAsync();
        
        return sample;
    }

    public async Task<bool> Create(Sample sample)
    {
        var command = new SqlCommand(SqlQueries.CreateSample, connection);
 
        command.Parameters.AddWithValue("@sample", sample.Id);
        command.Parameters.AddWithValue("@issued", sample.IssuedAt);
        command.Parameters.AddWithValue("@referral", sample.Referral?.Id);
        command.Parameters.AddWithValue("@bio_case_id", sample.BioCase?.Id);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    public async Task<bool> Delete(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.DeleteSample, connection);
 
        command.Parameters.AddWithValue("@sample", guid);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    public async Task<bool> IsExists(Guid guid)
    {
        var command = new SqlCommand(SqlQueries.IsSampleExists, connection);
        command.Parameters.AddWithValue("@id", guid);
        var isSampleExists = await command.ExecuteScalarAsync();
        
        if (isSampleExists == null) return false;
        return (int)isSampleExists > 0;
    }
}