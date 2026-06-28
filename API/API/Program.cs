using API.Repository;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de servicios de negocio
builder.Services.AddTransient<ApiServices>();
builder.Services.AddTransient<IPdfService, PdfService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ITipoCambioService, TipoCambioService>();

// HttpClient para TipoCambioService (consulta BCCR)
builder.Services.AddHttpClient<ITipoCambioService, TipoCambioService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddDbContext<DbContextBeach>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalString"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
