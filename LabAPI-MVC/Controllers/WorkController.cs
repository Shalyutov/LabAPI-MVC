using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/work_item")]
public class WorkController: ControllerBase
{
    [HttpPost]
    [Route("")]
    public async Task<string> CreateWorkItem(TestRepo testRepo, SampleRepo sampleRepo, ReferralRepo referralRepo, WorkItemRepo workItemRepo, [FromBody] WorkItem workItem)
    {
        if (!await referralRepo.IsExists(workItem.Referral.Id!.Value)) 
            return "not exist referral";
        if (!await sampleRepo.IsExists(workItem.Sample.Id!.Value)) 
            return "not exist referral";
        if (!await testRepo.IsTestExists(workItem.Test.Id!.Value))
            return "not exist test";
        
        workItem.Created ??= DateTime.Now;
        
        return await workItemRepo.Create(workItem) != null ? workItem.Id!.Value.ToString() : "not inserted";
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<string> DeleteWorkItem(WorkItemRepo workItemRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";
        
        return await workItemRepo.Delete(itemId) ? "ok" : "not deleted";
    }
    
}