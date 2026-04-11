using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LabAPI_MVC.Entities;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/referrals")]
public class ReferralController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<Referral?> GetReferral(SqlConnection connection, string id)
    {
        var guid = Guid.Parse(id);
        Referral? referral = null;
        Patient? patient = null;
        Guid patientId = Guid.Empty;
        
        // get referral
        const string sql = """
                                   select r.referral_id,
                                          r.patient_id,
                                          r.issued
                                     from prelab.referral r
                                    where r.referral_id = @id
                           """;
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", guid);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows) 
            return null;
        while (reader.Read())
        {
            if (referral != null) continue;
            referral = new Referral
            {
                Id = reader.GetGuid(0),
                IssuedAt = reader.GetDateTime(2)
            };
            patientId = reader.GetGuid(1);
            break;
        }
        await reader.CloseAsync();

        if (referral == null) return null;
        
        // get patient
        const string sqlPatient = """
                                   select p.patient_id,
                                          p.full_name,
                                          p.birth_date,
                                          p.document,
                                          p.phone,
                                          p.email
                                     from prelab.patient p
                                    where p.patient_id = @id
                           """;
        command = new SqlCommand(sqlPatient, connection);
        command.Parameters.AddWithValue("@id", patientId);
        reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows) 
            return null;
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
                Email = reader.GetString(5),
            };
            break;
        }
        await reader.CloseAsync();
        
        referral.Patient = patient;
        
        // get referral tests
        
        
        const string sqlTests = """
                                          select rt.test_id,
                                                 t.name,
                                                 t.description,
                                                 b.biomaterial_id,
                                                 b.name,
                                                 b.description
                                            from prelab.referral_test rt
                                            join prelab.test t
                                              on rt.test_id = t.test_id
                                            join prelab.biomaterial b
                                              on b.biomaterial_id = t.biomaterial_id
                                           where rt.referral_id = @id
                                  """;
        command = new SqlCommand(sqlTests, connection);
        command.Parameters.AddWithValue("@id", referral.Id);
        reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
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
                        Name = reader.GetSqlString(4).IsNull ? null :  reader.GetSqlString(4).Value,
                        Description = reader.GetSqlString(5).IsNull ? null : reader.GetSqlString(5).Value
                    }
                };
                tests.Add(test);
            }
            await reader.CloseAsync();
            referral.Tests = tests;
        }
        
        // get referral samples
        const string sqlSamples = """
                                  select s.sample_id,
                                        s.issued,
                                        bc.bio_case_id,
                                        bc.name,
                                        bc.description,
                                        bcn.bio_container_id,
                                        bcn.name,
                                        bcn.description,
                                        b.biomaterial_id,
                                        b.name,
                                        b.description
                                   from prelab.sample s
                                   join prelab.bio_case bc
                                     on bc.bio_case_id = s.bio_case_id
                                   join prelab.bio_container bcn
                                     on bcn.bio_container_id = bc.bio_container_id
                                   join prelab.biomaterial b
                                     on b.biomaterial_id = bcn.biomaterial_id
                                  where s.referral_id = @id
                                    and s.patient_id = @patientId
                                  """;
        command = new SqlCommand(sqlSamples, connection);
        command.Parameters.AddWithValue("@id", referral.Id);
        command.Parameters.AddWithValue("@patientId", referral.Patient?.Id);
        reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            List<Sample> samples = [];
            while (reader.Read())
            {
                var sample = new Sample
                {
                    Id = reader.GetGuid(0),
                    IssuedAt =  reader.GetDateTime(1),
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
                                Id =  reader.GetInt32(8),
                                Name = reader.GetSqlString(9).IsNull ? null : reader.GetSqlString(9).Value,
                                Description = reader.GetSqlString(10).IsNull ? null :  reader.GetSqlString(10).Value
                            }
                        }
                    }
                };
                samples.Add(sample);
            }
            await reader.CloseAsync();
            referral.Samples = samples;
        }
        
        return referral;
    }
}