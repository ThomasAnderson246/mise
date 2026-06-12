using Microsoft.EntityFrameworkCore;
using Mise.API.Extensions;
using Mise.API.Middleware;
using Mise.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);


var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI();

// commented out for railway redirection for now
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
