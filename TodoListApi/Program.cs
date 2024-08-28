using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TodoListApi.Data;
using TodoListApi.Services;
using TodoListApi.Validators;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;
using DotNetEnv;
using TodoListApi.Swagger.TodoExample;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

var sqlBuilder = new SqlConnectionStringBuilder()
{
    DataSource = Environment.GetEnvironmentVariable("SQLSERVER_HOST"),  // e.g. '127.0.0.1' ('cloudsql' when deploying to App Engine Flexible environment)
    UserID = Environment.GetEnvironmentVariable("DB_USER"),             // e.g. 'my-db-user'
    Password = Environment.GetEnvironmentVariable("DB_PASS"),           // e.g. 'my-db-password'
    InitialCatalog = Environment.GetEnvironmentVariable("DB_NAME"),     // e.g. 'my-database'
    IntegratedSecurity = Convert.ToBoolean(Environment.GetEnvironmentVariable("Integrated_Security")),
    TrustServerCertificate = true,
    Encrypt = false, // The Cloud SQL proxy provides encryption between the proxy and instance
};

// Register DbContext using SQL Server
builder.Services.AddDbContext<TodoListContext>(options =>
    options.UseSqlServer(sqlBuilder.ConnectionString));

// Register scoped services
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IValidator<TodoDto>, TodoDtoValidator>();

builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IValidator<PriorityDto>, PriorityDtoValidator>();

builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IValidator<StatusDto>, StatusDtoValidator>();

builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IValidator<TagDto>, TagDtoValidator>();

// Register controllers
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoListApi", Version = "v1" });
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<TodoCreateExample>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoListApi v1");
    c.RoutePrefix = ""; // Set to empty string to make Swagger UI available at the root
});

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
