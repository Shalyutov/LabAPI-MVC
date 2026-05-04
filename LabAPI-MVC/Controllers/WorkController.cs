using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/work")]
public class WorkController: ControllerBase
{
    [HttpPost]
    [Route("")]
    public async Task<bool> CreateWorkItem(WorkItemRepo workItemRepo, [FromBody] WorkItem workItem)
    {
        throw new NotImplementedException();
    }
    
}