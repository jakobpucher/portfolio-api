using Dapper;
using PortfolioAPI.Models;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/skills", async  () =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");

    using var connection = new SqlConnection(connectionString);
    var skills = await connection.QueryAsync<Skill>("SELECT id, name, icon FROM skills");

    return skills;
});


app.MapPost("/skills", async (Skill skill) =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??  builder.Configuration.GetConnectionString("DefaultConnection");

    using var connection = new SqlConnection(connectionString);
    var result = await connection.ExecuteAsync("INSERT INTO skills (name, icon) VALUES (@name, @icon);", new { skill.name, skill.icon });

    return result > 0 ? Results.Ok("Skill added successfully") : Results.Problem("Failed to add skill.");
});

app.Run();
