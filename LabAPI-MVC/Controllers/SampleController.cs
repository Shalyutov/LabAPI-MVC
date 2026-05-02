using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/samples")]
public class SampleController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<Sample?> Get(SampleRepo sampleRepo, string id)
    {
        var sampleId = Guid.Parse(id);
        return await sampleRepo.GetById(sampleId);
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<bool> Delete(SampleRepo sampleRepo, string id)
    {
        var sampleId = Guid.Parse(id);
        return await sampleRepo.Delete(sampleId);
    }
    
    [HttpPost]
    [Route("")]
    public async Task<string> CreateSample(ReferralRepo referralRepo, SampleRepo sampleRepo, BioCaseRepo bioCaseRepo, [FromBody] Sample sample)
    {
        if (!await referralRepo.IsExists(sample.Referral!.Id!.Value)) 
            return "not exist referral";
        if (!await bioCaseRepo.IsExists(sample.BioCase!.Id!.Value))
            return "not exist bio case";

        sample.Id ??= Guid.NewGuid();
        sample.IssuedAt ??= DateTime.Now;
        
        return await sampleRepo.Create(sample) ? "ok" : "not inserted";
    }
}