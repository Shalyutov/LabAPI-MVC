using System.Data.Common;
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
                                          r.issued,
                                          r.weight,
                                          r.height,
                                          r.sex
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
                IssuedAt = reader.GetDateTime(2),
                Weight = reader.GetSqlDecimal(3).IsNull ?  null : reader.GetSqlDecimal(3).Value,
                Height = reader.GetSqlDecimal(4).IsNull ?  null : reader.GetSqlDecimal(4).Value,
                Sex = reader.GetSqlInt32(5).IsNull ?  null : reader.GetSqlInt32(5).Value
            };
            patientId = reader.GetSqlGuid(1).IsNull ? Guid.Empty : reader.GetSqlGuid(1).Value;
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
        if (reader.HasRows)
        {
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
            }
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
        List<Test> tests = [];
        if (reader.HasRows)
        {
            
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
            referral.Tests = tests;
        }
        await reader.CloseAsync();
        
        
        // get referral samples
        if (patient != null)
        {
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
                                                b.description,
                                                sp.supplier_id,
                                                sp.name,
                                                sp.description
                                           from prelab.sample s
                                           join prelab.bio_case bc
                                             on bc.bio_case_id = s.bio_case_id
                                           join prelab.bio_container bcn
                                             on bcn.bio_container_id = bc.bio_container_id
                                           join prelab.biomaterial b
                                             on b.biomaterial_id = bcn.biomaterial_id
                                           join prelab.supplier sp
                                             on sp.supplier_id = bc.supplier_id
                                          where s.referral_id = @id
                                            and s.patient_id = @patientId
                                      """;
            command = new SqlCommand(sqlSamples, connection);
            command.Parameters.AddWithValue("@id", referral.Id);
            command.Parameters.AddWithValue("@patientId", referral.Patient?.Id);
            reader = await command.ExecuteReaderAsync();
            List<Sample> samples = [];
            if (reader.HasRows)
            {

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

            }

            await reader.CloseAsync();
            referral.Samples = samples;
        }

        return referral;
    }

    [HttpPost]
    [Route("")]
    public async Task<Referral?> CreateReferral(SqlConnection connection, [FromBody] Referral? referral)
    {
        SqlCommand command;
        referral ??= new Referral();
        
        if (referral.Patient?.Id != null)
        {
            const string sqlPatient = """
                                      select count(1)
                                        from prelab.patient p
                                       where p.patient_id = @id
                                      """;
            command = new SqlCommand(sqlPatient, connection);
            command.Parameters.AddWithValue("@id", referral.Patient?.Id);
            var isExists = await command.ExecuteScalarAsync();
            if (isExists == null) return null;
            if ((int)isExists == 0) return null;
        }

        referral.Id ??= Guid.NewGuid();
        referral.IssuedAt ??= DateTime.Now;
        
        const string sql = """
                           insert into prelab.referral (referral_id, patient_id, issued, weight, height, sex)
                           values (@id, @patient, @issued, @weight, @height, @sex);
                           """;
        command = new SqlCommand(sql, connection);
 
        command.Parameters.AddWithValue("@id", referral.Id);
        command.Parameters.AddWithValue("@issued", referral.IssuedAt);
        command.Parameters.AddWithValue("@patient",  referral.Patient?.Id ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@weight", referral.Weight ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@height", referral.Height ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sex", referral.Sex ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        if (affected > 0) return referral;
        return null;
    }

    [HttpPost]
    [Route("{id}/patient")]
    public async Task<string> SetPatient(SqlConnection connection, string id, [FromBody] Patient patient)
    {
        const string sqlReferral= """
                                  select count(1)
                                    from prelab.referral r
                                   where r.referral_id = @id
                                  """;
        var command = new SqlCommand(sqlReferral, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        var isReferralExists = await command.ExecuteScalarAsync();
        if (isReferralExists == null) return "null exist referral";
        if ((int)isReferralExists == 0) return "not exist referral";
        
        if (patient.Id != null)
        {
            const string sqlPatient = """
                                      select count(1)
                                        from prelab.patient p
                                       where p.patient_id = @id
                                      """;
            command = new SqlCommand(sqlPatient, connection);
            command.Parameters.AddWithValue("@id", patient.Id);
            var isPatientExists = await command.ExecuteScalarAsync();
            if (isPatientExists == null) return "null exist patient";
            if ((int)isPatientExists == 0) return "not exist patient";
        }
        
        const string sql = """
                           update prelab.referral
                              set patient_id = @patient
                            where referral_id = @id
                           """;
        command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        command.Parameters.AddWithValue("@patient",  patient.Id ?? (object)DBNull.Value);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0 ? "ok" : "error";
    }

    [HttpPost]
    [Route("{id}/tests")]
    public async Task<string> SetTest(SqlConnection connection, string id, [FromBody] Test test)
    {
        const string sqlReferral= """
                                  select count(1)
                                    from prelab.referral r
                                   where r.referral_id = @id
                                  """;
        var command = new SqlCommand(sqlReferral, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        var isReferralExists = await command.ExecuteScalarAsync();
        if (isReferralExists == null) return "null exist referral";
        if ((int)isReferralExists == 0) return "not exist referral";
        
        if (test.Id != null)
        {
            const string sqlPatient = """
                                      select count(1)
                                        from prelab.test t
                                       where t.test_id = @id
                                      """;
            command = new SqlCommand(sqlPatient, connection);
            command.Parameters.AddWithValue("@id", test.Id);
            var isTestExists = await command.ExecuteScalarAsync();
            if (isTestExists == null) return "null exist test";
            if ((int)isTestExists == 0) return "not exist test";
        }
        
        const string sql = """
                           if not exists (select 1 
                            from prelab.referral_test 
                           where referral_id = @id 
                             and test_id = @test)
                             begin
                           insert into prelab.referral_test (referral_id, test_id)
                           values (@id, @test);
                           end;
                           """;
        command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        command.Parameters.AddWithValue("@test", test.Id);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0 ? "ok" : "exist";
    }

    [HttpDelete]
    [Route("{id}/tests")]
    public async Task<string> DeleteTest(SqlConnection connection, string id, [FromBody] Test test)
    {
        const string sqlReferral= """
                                  select count(1)
                                    from prelab.referral r
                                   where r.referral_id = @id
                                  """;
        var command = new SqlCommand(sqlReferral, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        var isReferralExists = await command.ExecuteScalarAsync();
        if (isReferralExists == null) return "null exist referral";
        if ((int)isReferralExists == 0) return "not exist referral";
        
        if (test.Id != null)
        {
            const string sqlPatient = """
                                      select count(1)
                                        from prelab.test t
                                       where t.test_id = @id
                                      """;
            command = new SqlCommand(sqlPatient, connection);
            command.Parameters.AddWithValue("@id", test.Id);
            var isTestExists = await command.ExecuteScalarAsync();
            if (isTestExists == null) return "null exist test";
            if ((int)isTestExists == 0) return "not exist test";
        }
        
        const string sql = """
                           delete from prelab.referral_test
                            where referral_id = @id
                              and test_id = @test;
                           """;
        command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        command.Parameters.AddWithValue("@test", test.Id);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0 ? "ok" : "nothing to delete";
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<string> UpdateReferral(SqlConnection connection, string id, [FromBody] Referral test)
    {
        const string sqlReferral= """
                                  select count(1)
                                    from prelab.referral r
                                   where r.referral_id = @id
                                  """;
        var command = new SqlCommand(sqlReferral, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        var isReferralExists = await command.ExecuteScalarAsync();
        if (isReferralExists == null) return "null exist referral";
        if ((int)isReferralExists == 0) return "not exist referral";
        
        const string sql = """
                           update prelab.referral
                              set weight = @weight,
                                  height = @height,
                                  sex = @sex
                            where referral_id = @id;
                           """;
        command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", Guid.Parse(id));
        command.Parameters.AddWithValue("@weight", test.Weight ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@height", test.Height ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sex", test.Sex ?? (object)DBNull.Value);
        
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0 ? "ok" : "nothing to delete";
    }
}