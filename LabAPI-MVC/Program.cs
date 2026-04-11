using System.Data;
using Microsoft.Data.SqlClient;

var connectionString = "Server=localhost;Database=lis;User Id=sa;Password=shaly799-=;Encrypt=False;";
var connection = new SqlConnection(connectionString);
try
{
    connection.Open();
    Console.WriteLine("Подключение открыто");
    Console.WriteLine("Свойства подключения:");
    Console.WriteLine($"\tБаза данных: {connection.Database}");
    Console.WriteLine($"\tСервер: {connection.DataSource}");
    Console.WriteLine($"\tВерсия сервера: {connection.ServerVersion}");
    Console.WriteLine($"\tСостояние: {connection.State}");
    Console.WriteLine($"\tWorkstationld: {connection.WorkstationId}");
}
catch (SqlException ex)
{
    Console.WriteLine(ex.Message);
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(connection);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

if (connection.State == ConnectionState.Open)
{
    connection.Close();
    Console.WriteLine("Подключение закрыто...");
}