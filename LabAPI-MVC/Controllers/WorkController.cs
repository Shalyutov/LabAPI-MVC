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
    
    [HttpGet]
    [Route("")]
    public async Task<List<WorkItem>> GetWorkItemByEquipment(WorkItemRepo workItemRepo, [FromQuery] string equipmentId)
    {
        var id = int.Parse(equipmentId);
        
        return await workItemRepo.GetByEquipment(id);
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
    public async Task<string> PostResultWorkItem(WorkItemRepo workItemRepo, ResultRepo resultRepo, string id, [FromBody] List<ResultDetail> resultDetails)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";

        var r = await resultRepo.CreateResult(itemId);

        if (!r)
        {
            return "not created";
        }

        foreach (var res in resultDetails)
        {
            var v = await resultRepo.CreateResultDetail(itemId, res);
            if (!v)
            {
                return $"item_id: {itemId} not inserted detail: {res.Indicator}";
            }
        }

        return "ok";
    }

    [HttpGet]
    [Route("{id}/result")]
    public async Task<LabResult?> GetResultWorkItem(WorkItemRepo workItemRepo, ResultRepo resultRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return null;
        
        var result = await resultRepo.GetResult(itemId);
        return result;
    }
    
    [HttpDelete]
    [Route("{id}/result")]
    public async Task<string> DeleteResultWorkItem(WorkItemRepo workItemRepo, ResultRepo resultRepo, string id)
    {
        var itemId = int.Parse(id);

        if (!await workItemRepo.IsExists(itemId))
            return "not exists";
        
        var result = await resultRepo.DeleteResult(itemId);
        return result ? "ok" : "not deleted";
    }
}