using System.Data;
using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class WorkItemRepo(SqlConnection connection)
{
    public async Task<WorkItem?> Create(WorkItem workItem)
    {
        var command = new SqlCommand(SqlQueries.CreateWorkItem, connection);
 
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
        command.Parameters.AddWithValue("@id", id);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    public async Task<WorkItem?> Get(int id)
    {
        var command = new SqlCommand(SqlQueries.GetWorkItem, connection);
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return null;
        }

        WorkItem? workItem = null;
        while (reader.Read())
        {
            if (workItem != null) continue;
            workItem = new WorkItem
            {
                Id = id,
                Test = new Test{Id = reader.GetInt32(1)},
                Sample = new Sample{Id = reader.GetGuid(2)},
                Equipment = new Equipment{Id = reader.GetInt32(3)},
                Created = reader.GetDateTime(4),
                Processed = reader.GetSqlDateTime(5).IsNull ? null : reader.GetSqlDateTime(5).Value,
                Canceled =  reader.GetSqlDateTime(6).IsNull ? null : reader.GetSqlDateTime(6).Value
            };
        }
        await reader.CloseAsync();
        
        return workItem;
    }
    
    public async Task<List<WorkItem>> GetByEquipment(int id)
    {
        var command = new SqlCommand(SqlQueries.GetWorkItemsByEquipment, connection);
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return [];
        }

        var workItems = new List<WorkItem>();
        while (reader.Read())
        {
            var workItem = new WorkItem
            {
                Id = id,
                Test = new Test{Id = reader.GetInt32(1)},
                Sample = new Sample{Id = reader.GetGuid(2)},
                Equipment = new Equipment{Id = reader.GetInt32(3)},
                Created = reader.GetDateTime(4),
                Processed = reader.GetSqlDateTime(5).IsNull ? null : reader.GetSqlDateTime(5).Value,
                Canceled =  reader.GetSqlDateTime(6).IsNull ? null : reader.GetSqlDateTime(6).Value
            };
            workItems.Add(workItem);
        }
        await reader.CloseAsync();
        
        return workItems;
    }
    
    public async Task<bool> SetProcessed(int id, DateTime processedAt)
    {
        var command = new SqlCommand(SqlQueries.UpdateProcessedWorkItem, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@processed_at", processedAt);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    public async Task<bool> SetCanceled(int id, DateTime canceledAt)
    {
        var command = new SqlCommand(SqlQueries.UpdateCanceledWorkItem, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@canceled_at", canceledAt);
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }
    
    
}