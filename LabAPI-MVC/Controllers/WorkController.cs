using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/workitem")]
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
    
    [HttpGet]
    [Route("{id}")]
    public async Task<WorkItem?> GetWorkItem(WorkItemRepo workItemRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return null;
        
        return await workItemRepo.Get(itemId);
    }
    
    [HttpPost]
    [Route("{id}/processed")]
    public async Task<string> ProcessedWorkItem(WorkItemRepo workItemRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";
        
        return await workItemRepo.SetProcessed(itemId, DateTime.Now) ? "ok" : "not updated";
    }
    
    [HttpPost]
    [Route("{id}/canceled")]
    public async Task<string> CanceledWorkItem(WorkItemRepo workItemRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";
        
        return await workItemRepo.SetCanceled(itemId, DateTime.Now) ? "ok" : "not updated";
    }

    [HttpPost]
    [Route("{id}/result")]
    public async Task<string> ResultWorkItem(WorkItemRepo workItemRepo, string id, [FromBody] List<ResultDetail> resultDetails)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";

        foreach (var res in resultDetails)
        {
            // todo repo
        }

        throw new NotImplementedException();
    }
    
}