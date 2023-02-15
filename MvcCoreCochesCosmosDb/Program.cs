using Microsoft.Azure.Cosmos;
using MvcCoreCochesCosmosDb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
string connectionString = builder.Configuration.GetConnectionString("AzureCosmos");
string database = builder.Configuration.GetValue<string>("CochesCosmosDb:Database");
string container = builder.Configuration.GetValue<string>("CochesCosmosDb:Container");
//CREAMOS NUESTRO CosmosClient CON LA CADENA DE CONEXION
CosmosClient cosmosClient = new CosmosClient(connectionString);
//RECUPERAMOS TAMBIEN EL CONTENEDOR A PARTIR DE LA BASE DE DATOS Y EL NOMBRE DEL CONTAINER
Container containerCosmos = cosmosClient.GetContainer(database, container);
//PONEMOS DENTRO DE NUESTRA APLICACION EL CLIENTE DE COSMOS
builder.Services.AddSingleton<CosmosClient>(x => cosmosClient);
//EL CONTENEDOR LO UTILIZAREMOS MULTIPLES VECES POR LO QUE LO INCLUIMOS COMO TRANSIENT
builder.Services.AddTransient<Container>(x => containerCosmos);
//INYECTAMOS NUESTRO SERVICE COSMOS PARA QUE LO PUEDAN UTILIZAR LOS CONTROLADORES
builder.Services.AddTransient<ServiceCochesCosmos>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
