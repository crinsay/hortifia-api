using Hortifia.API.Extensions;
using Hortifia.API.Handlers;
using Hortifia.Application.Extensions;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Host);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGroup("api/identity")
    .WithTags("Identity")
    .MapHortifiaIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();