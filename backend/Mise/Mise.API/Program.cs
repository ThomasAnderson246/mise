using Microsoft.EntityFrameworkCore;
using Mise.API.Middleware;
using Mise.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MiseDbContext>(options => 
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
     )
   );

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMIddleware>();

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
