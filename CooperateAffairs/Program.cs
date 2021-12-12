using CooperateAffairs.Configs;
using CooperateAffairs.Extensions;
using CooperateAffairs.Logic;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("appSettings.json");
builder.Services.Configure<DekoSettings>(builder.Configuration.GetSection(DekoSettings.SectionName));
builder.Services.AddScoped<IDekoSharp, DekoSharp>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPdf, PdfGenerator>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureDekoExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
