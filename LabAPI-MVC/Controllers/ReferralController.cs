using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/referrals")]
public class ReferralController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<Referral?> GetReferral(ReferralRepo referralRepo, PatientRepo patientRepo, SampleRepo sampleRepo, string id)
    {
        var guid = Guid.Parse(id);
        
        var referral = await referralRepo.Get(guid);
        if (referral == null) return null;

        if (referral.Patient is { Id: not null })
        {
            referral.Patient = await patientRepo.Get(referral.Patient.Id.Value);
        }
        referral.Tests = await referralRepo.GetTests(guid);
        referral.Samples = await sampleRepo.GetByReferral(guid);
        
        return referral;
    }

    [HttpPost]
    [Route("")]
    public async Task<Referral?> CreateReferral(PatientRepo patientRepo, ReferralRepo referralRepo, [FromBody] Referral? referral)
    {
        referral ??= new Referral();
        
        if (referral.Patient?.Id != null)
        {
            var isExists = await patientRepo.IsExist(referral.Patient!.Id.Value);
            if (!isExists) return null;
        }

        referral.Id ??= Guid.NewGuid();
        referral.IssuedAt ??= DateTime.Now;
        
        var result = await referralRepo.Create(referral);
        return result ? referral : null;
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