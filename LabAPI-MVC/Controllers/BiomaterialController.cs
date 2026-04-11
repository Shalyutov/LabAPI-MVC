using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LabAPI_MVC.Controllers;

public class Biomaterial
{
    public int? Id { get; set; }
    public string? Name  { get; set; }
    public string? Description  { get; set; }
}

[ApiController]
public class BiomaterialController : ControllerBase
{
    [HttpGet]
    [Route("api/biomaterial")]
    public async Task<IEnumerable<Biomaterial>> Get(SqlConnection connection)
    {
        var sql = @"
        select b.biomaterial_id, 
               b.name,
               b.description
          from prelab.biomaterial b";
    
        var biomaterials = new List<Biomaterial>();
    
        SqlCommand command = new SqlCommand(sql, connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) 
            return biomaterials.ToArray();
    
        while (await reader.ReadAsync())
        {
            var biomaterial = new Biomaterial
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetSqlString(2).ToString()
            };
            biomaterials.Add(biomaterial);
        }

        await reader.CloseAsync();
            
        return biomaterials.ToArray();
    }
    
    [HttpGet]
    [Route("api/biomaterial/{id}")]
    public async Task<Biomaterial?> GetBiomaterial(SqlConnection connection, int id)
    {
        var sql = @"
        select b.biomaterial_id, 
               b.name,
               b.description
          from prelab.biomaterial b
         where b.biomaterial_id = @id";

        Biomaterial? biomaterial = null;
    
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        
        SqlDataReader reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) 
            return null;
    
        while (reader.Read())
        {
            if (biomaterial != null) continue;
            biomaterial = new Biomaterial
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetSqlString(2).IsNull ? null : reader.GetSqlString(2).Value
            };
            break;
        }

        await reader.CloseAsync();
            
        return biomaterial;
    }

    [HttpPost]
    [Route("api/biomaterial")]
    public async Task<Biomaterial?> Post(SqlConnection connection, [FromBody] Biomaterial biomaterial)
    {
        var sql = @"
            INSERT INTO prelab.biomaterial (name, description) 
            VALUES (@name, @description); 
            SET @id=SCOPE_IDENTITY()";
        SqlCommand command = new SqlCommand(sql, connection);
 
        command.Parameters.AddWithValue("@name",  biomaterial.Name ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@description", biomaterial.Description ?? (object)DBNull.Value);
        
        SqlParameter idParam = new SqlParameter
        {
            ParameterName = "@id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(idParam);
        
        var affected = await command.ExecuteNonQueryAsync();

        if (affected > 0)
        {
            biomaterial.Id = (int)idParam.Value;
            return biomaterial;
        }

        return null;
    }
    
    [HttpDelete]
    [Route("api/biomaterial/{id}")]
    public async Task<string> Delete(SqlConnection connection, int id)
    {
        var sql = "delete from prelab.biomaterial where biomaterial_id = @id";
        SqlCommand command = new SqlCommand(sql, connection);
 
        command.Parameters.AddWithValue("@id", id);
        
        var affected = await command.ExecuteNonQueryAsync();

        if (affected > 0)
        {
            return "ok";
        }

        return "error";
    }
}

