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
        
        var idParam = new SqlParameter
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
    
    public async Task<bool> IsExists(int id)
    {
        var command = new SqlCommand(SqlQueries.IsWorkItemExists, connection);
        command.Parameters.AddWithValue("@id", id);
        var isReferralExists = await command.ExecuteScalarAsync();
        
        if (isReferralExists == null) return false;
        return (int)isReferralExists > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var command = new SqlCommand(SqlQueries.DeleteWorkItem, connection);
        command.Parameters.AddWithValue("@referral", id);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
}