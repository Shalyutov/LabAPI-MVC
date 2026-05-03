using Microsoft.AspNetCore.Mvc;
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
            if (!await patientRepo.IsExist(referral.Patient!.Id.Value)) 
                return null;
        }

        referral.Id ??= Guid.NewGuid();
        referral.IssuedAt ??= DateTime.Now;
        
        return await referralRepo.Create(referral) ? referral : null;
    }

    [HttpPost]
    [Route("{id}/patient")]
    public async Task<string> SetPatient(ReferralRepo referralRepo, PatientRepo patientRepo, string id, [FromBody] Patient patient)
    {
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exist referral";
        
        if (patient.Id != null && !await patientRepo.IsExist(patient.Id.Value)) 
            return "not exist patient";

        return await referralRepo.SetPatient(referralId, patient.Id) ? "ok" : "error";
    }

    [HttpPost]
    [Route("{id}/tests")]
    public async Task<string> SetTest(ReferralRepo referralRepo, TestRepo testRepo, string id, [FromBody] Test test)
    {
        if (test.Id == null)
            return "no test specified";
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exist referral";
        if (!await testRepo.IsTestExists(test.Id.Value)) 
            return "not exist test";
        
        return await referralRepo.LinkTest(referralId, test.Id.Value) ? "ok" : "exist";
    }

    [HttpDelete]
    [Route("{id}/tests")]
    public async Task<string> DeleteTest(ReferralRepo referralRepo, TestRepo testRepo, string id, [FromBody] Test test)
    {
        if (test.Id == null)
            return "no test specified";
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exist referral";
        if (!await testRepo.IsTestExists(test.Id.Value)) 
            return "not exist test";
        
        return await referralRepo.UnlinkTest(referralId, test.Id.Value) ? "ok" : "nothing to delete";
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<string> UpdateReferral(ReferralRepo referralRepo, string id, [FromBody] Referral referral)
    {
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exist referral";
        
        referral.Id ??= referralId;
        
        return await referralRepo.UpdateReferral(referral) ? "ok" : "not updated";
    }

    [HttpPost]
    [Route("{id}/samples")]
    public async Task<string> CreateSample(ReferralRepo referralRepo, SampleRepo sampleRepo, BioCaseRepo bioCaseRepo, string id, [FromBody] Sample sample)
    {
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exist referral";
        if (!await bioCaseRepo.IsExists(sample.BioCase!.Id!.Value))
            return "not exist bio case";

        sample.Id ??= Guid.NewGuid();
        sample.IssuedAt ??= DateTime.Now;
        sample.Referral ??= new Referral{Id = referralId};
        
        return await sampleRepo.Create(sample) ? "ok" : "not updated";
    }

    [HttpGet]
    [Route("{id}/samples")]
    public async Task<List<Sample>?> GetSamples(ReferralRepo referralRepo, SampleRepo sampleRepo, string id)
    {
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return null;
        
        return await sampleRepo.GetByReferral(referralId);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<string> Delete(ReferralRepo referralRepo, string id)
    {
        var referralId = Guid.Parse(id);
        if (!await referralRepo.IsExists(referralId)) 
            return "not exists";
        
        return await referralRepo.Delete(referralId) ? "ok" : "not deleted";
    }
}