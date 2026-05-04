using System.Data;
using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class WorkItemRepo(SqlConnection connection)
{
    public async Task<WorkItem?> Create(WorkItem workItem)
    {
        var command = new SqlCommand(SqlQueries.CreateWorkItem, connection);
 
        command.Parameters.AddWithValue("@referral",  workItem.Referral.Id);
        command.Parameters.AddWithValue("@test", workItem.Test.Id);
        command.Parameters.AddWithValue("@sample",  workItem.Sample.Id);
        command.Parameters.AddWithValue("@equipment", workItem.Equipment.Id);
        command.Parameters.AddWithValue("@created_at",  workItem.Created);
        
        SqlParameter idParam = new SqlParameter
        {
            ParameterName = "@id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(idParam);
        
        var affected = await command.ExecuteNonQueryAsync();
        
        if (affected == 0)
            return null;
        
        workItem.Id = (int)idParam.Value;
        return workItem;
    }
}