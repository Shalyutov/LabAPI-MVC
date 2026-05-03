using LabAPI_MVC.Entities;
using LabAPI_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI_MVC.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<Patient?> GetPatient(PatientRepo patientRepo, string id)
    {
        var patientId = Guid.Parse(id);
        return await patientRepo.Get(patientId);
    }

    [HttpPost]
    [Route("")]
    public async Task<bool> CreatePatient(PatientRepo patientRepo, [FromBody] Patient? patient)
    {
        patient ??= new Patient();
        patient.Id ??= Guid.NewGuid();
        return await patientRepo.Create(patient);
    }
    
    [HttpPut]
    [Route("{id}")]
    public async Task<bool> UpdatePatient(PatientRepo patientRepo, string id, [FromBody] Patient patient)
    {
        var patientId = Guid.Parse(id);
        if (!await patientRepo.IsExist(patientId))
            return false;
        
        patient.Id ??= patientId;
        return await patientRepo.Update(patient);
    }
    [HttpDelete]
    [Route("{id}")]
    public async Task<bool> DeletePatient(PatientRepo patientRepo, string id)
    {
        var patientId = Guid.Parse(id);
        if (!await patientRepo.IsExist(patientId))
            return false;
        
        return await patientRepo.Delete(patientId);
    }
}