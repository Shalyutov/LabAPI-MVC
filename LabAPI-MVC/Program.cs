using System.Data;
using LabAPI_MVC.Repositories;
using Microsoft.Data.SqlClient;

var connection = new SqlConnection();
var patientRepo = new PatientRepo(connection);
var referralRepo = new ReferralRepo(connection);
var sampleRepo = new SampleRepo(connection);
var testRepo = new TestRepo(connection);
var bioCaseRepo = new BioCaseRepo(connection);
var workItemRepo = new WorkItemRepo(connection);
var resultsRepo = new ResultRepo(connection);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(connection);
builder.Services.AddSingleton(patientRepo);
builder.Services.AddSingleton(referralRepo);
builder.Services.AddSingleton(sampleRepo);
builder.Services.AddSingleton(testRepo);
builder.Services.AddSingleton(bioCaseRepo);
builder.Services.AddSingleton(workItemRepo);
builder.Services.AddSingleton(resultsRepo);
builder.Services.AddSingleton(resultsRepo);

var app = builder.Build();

var connectionString = app.Configuration["DBConnectionString"];
connection.ConnectionString = connectionString!;
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

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");

app.Run();

if (connection.State == ConnectionState.Open)
{
    connection.Close();
    Console.WriteLine("Подключение закрыто...");
}