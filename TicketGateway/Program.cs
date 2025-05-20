using ExternalValidation.Interfaces;
using ExternalValidation.Pocos;
using ExternalValidation.Services;
using TicketGateway.Models;
using TicketGateway.Services;

var builder = WebApplication.CreateBuilder(args);

//Setting which connectionstrings for the pocos.
builder.Services.Configure<AzureServiceBusSettings>(builder.Configuration.GetSection("AzureServiceBus"));
builder.Services.Configure<EventApiSettings>(builder.Configuration.GetSection("EventApi"));
builder.Services.Configure<UserApiSettings>(builder.Configuration.GetSection("UserApi"));
builder.Services.Configure<InvoiceApiSettings>(builder.Configuration.GetSection("InvoiceApi"));

builder.Services.AddSingleton<TicketSBSender>();

builder.Services.AddHttpClient<IExternalEventCheck, ExternalEventCheck>();
builder.Services.AddHttpClient<IExternalInvoiceCheck, ExternalInvoiceCheck>();
builder.Services.AddHttpClient<IExternalUserCheck, ExternalUserCheck>();




builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();

app.Run();
