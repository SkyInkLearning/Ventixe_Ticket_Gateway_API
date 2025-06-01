using ExternalValidation.ApiSettings;
using ExternalValidation.Interfaces;
using ExternalValidation.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TicketGateway.Models;
using TicketGateway.Services;


var builder = WebApplication.CreateBuilder(args);

//Setting which connectionstrings for the pocos.
builder.Services.Configure<AzureServiceBusSettings>(builder.Configuration.GetSection("AzureServiceBus"));
builder.Services.Configure<EventApiSettings>(builder.Configuration.GetSection("EventApi"));
builder.Services.Configure<UserApiSettings>(builder.Configuration.GetSection("UserApi"));
builder.Services.Configure<InvoiceApiSettings>(builder.Configuration.GetSection("InvoiceApi"));
builder.Services.Configure<TicketServiceApiSettings>(builder.Configuration.GetSection("TicketServiceApi"));

builder.Services.AddSingleton<TicketSBSender>();

builder.Services.AddHttpClient<IExternalEventCheck, ExternalEventCheck>();
builder.Services.AddHttpClient<IExternalInvoiceCheck, ExternalInvoiceCheck>();
builder.Services.AddHttpClient<IExternalUserCheck, ExternalUserCheck>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v.1.0",
        Title = "Event TicketGateway API Documentation",
        Description = "Documentation for the TicketGateway API."
    });
    o.EnableAnnotations();
    o.ExampleFilters();

    var apiScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Description = "API KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        Reference = new OpenApiReference
        {
            Id = "ApiKey",
            Type = ReferenceType.SecurityScheme,
        }
    };

    o.AddSecurityDefinition("ApiKey", apiScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { apiScheme, new List<string>() }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();
app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event TicketGateway API v.1.0");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();

app.Run();
