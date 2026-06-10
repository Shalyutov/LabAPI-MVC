using LabAPI_MVC.Entities;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Repositories;

public class ResultRepo (SqlConnection connection)
{
    public async Task<bool> CreateResult(int id)
    {
        var command = new SqlCommand(SqlQueries.CreateResult, connection);
 
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@issued_at", DateTime.Now);
        
        var affected = await command.ExecuteNonQueryAsync();
        
        return affected != 0;
    }
    
    public async Task<bool> CreateResultDetail(int id, ResultDetail detail)
    {
        var command = new SqlCommand(SqlQueries.CreateResultDetail, connection);
 
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@indicator_id", detail.Indicator);
        command.Parameters.AddWithValue("@bool_value", detail.BoolValue);
        command.Parameters.AddWithValue("@decimal_value", detail.DecimalValue);
        command.Parameters.AddWithValue("@str_value", detail.StringValue);
        
        var affected = await command.ExecuteNonQueryAsync();
        
        return affected != 0;
    }
    
    public async Task<bool> ConfirmResult(int id)
    {
        var command = new SqlCommand(SqlQueries.UpdateConfirmedResult, connection);
 
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@confirmed_at", DateTime.Now);
        
        var affected = await command.ExecuteNonQueryAsync();
        
        return affected != 0;
    }
    
    public async Task<bool> DeleteResult(int id)
    {
        var command = new SqlCommand(SqlQueries.DeleteResult, connection);
 
        command.Parameters.AddWithValue("@id", id);
        
        var affected = await command.ExecuteNonQueryAsync();

        if (affected == 0) return false;
        
        command = new SqlCommand(SqlQueries.DeleteResultDetails, connection);
 
        command.Parameters.AddWithValue("@id", id);
        
        affected = await command.ExecuteNonQueryAsync();
        
        return affected != 0;
    }
    
    public async Task<LabResult?> GetResult(int id)
    {
        var command = new SqlCommand(SqlQueries.GetResult, connection);
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return null;
        }

        LabResult? result = null;
        while (reader.Read())
        {
            if (result != null) continue;
            result = new LabResult
            {
                WorkItemId = id,
                IssuedAt =  reader.GetDateTime(1),
                ConfirmedAt =   reader.GetSqlDateTime(2).IsNull ? null : reader.GetSqlDateTime(2).Value
            };
        }
        
        command = new SqlCommand(SqlQueries.GetResultDetails, connection);
        command.Parameters.AddWithValue("@id", id);
        reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            return result;
        }
        var results = new List<ResultDetail>();
        while (reader.Read())
        {
            var detail = new ResultDetail
            {
                Indicator = reader.GetInt32(0),
                BoolValue = reader.GetSqlBoolean(1).IsNull ? null : reader.GetSqlBoolean(1).Value,
                DecimalValue = reader.GetSqlDecimal(2).IsNull ? null : reader.GetSqlDecimal(2).Value,
                StringValue = reader.GetSqlString(3).IsNull ? null : reader.GetSqlString(3).Value
            };
            results.Add(detail);
        }
        await reader.CloseAsync();
        result?.ResultDetails = results;
        return result;
    }
}