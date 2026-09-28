using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Entity Framework para PostgreSQL / Supabase
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Puerto Publico
    options.ListenAnyIP(8080); // Puerto Administrativo
});

var app = builder.Build();

// 1. Endpoint para Unidad 1: Ciclo de Vida y Entornos
app.MapGet("/api/status", (IConfiguration config, IHostEnvironment env) =>
{
    var response = new
    {
        AppName = config["AppSettings:AppName"],
        Environment = env.EnvironmentName,
        EnabledDetailedLogs = config.GetValue<bool>("AppSettings:FeatureFlag_EnableDetailedLogs"),
        databasetimeout = config["DATABASE_TIMEOUT"] ?? "No definida"
    };
    return Results.Ok(response);
});

// 2. Endpoint para Unidad 3 y 4: Persistencia y prueba de lectura
app.MapGet("/api/items", async (AppDbContext db) => await db.Items.ToListAsync());

// 3. Endpoint para probar escrituras / CORS desde clientes web
app.MapPost("/api/items", async (Item item, AppDbContext db) => {
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/items/{item.Id}", item);
});

app.MapGet("Api/kestrel-info", () => 
{
    //Obtencion de metricas del ThreadPool
    ThreadPool.GetAvailableThreads(out int workerAvailable, out int completionAvailable);
    ThreadPool.GetMaxThreads(out int workerMax, out int completionMax);

    //Obtencion de memoria utilizada por el proceso
    using var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
    double workingSetMb = Math.Round(currentProcess.WorkingSet64 / (1024.0 * 1024.0), 2);

    var response = new
    {
        ThreadPool = new
        {
            WorkerThreads = new 
            { 
                Available = workerAvailable, 
                Max = workerMax 
            },
            CompletionPortThreads = new 
            { 
                Available = completionAvailable, 
                Max = completionMax 
            }
        },
        Memory = new
        {
            WorkingSetMb = workingSetMb
        },
        System = new
        {
            LogicalProcessors = Environment.ProcessorCount
        }
    };
    return Results.Ok(response);
});

app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    //Si intentan ingresar al endpoint de diagnostico
    if (!string.IsNullOrEmpty(path) && path.Equals("/Api/kestrel-info", StringComparison.OrdinalIgnoreCase))
    {
        //Se verifica que la conexion provenga exclusivamente del puerto 8080
        if (context.Connection.LocalPort != 8080)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Acceso denegado. Este endpoint solo está disponible en el puerto administrativo (8080).");
            return; //Corta la ejecucion de la peticion aqui
        }
    }
    await next(); //Si pasa la validacion (o es otra ruta), continua a los endpoints
});

app.MapGet("/api/items", async (AppDbContext db) =>
{
var items = await db.Items.ToListAsync();
return Results.Ok(items);
});

app.MapPost("/api/items", async ([FromBody] string name, AppDbContext db) =>
{
if (string.IsNullOrWhiteSpace(name)){
return Results.BadRequest("El nombre del item no puede estar vacío.");
}
var newItem = new Item{Name = name, CreatedAt = DateTime.UtcNow};
db.Items.Add(newItem);
await db.SaveChangesAsync();
return Results.Created($"/api/items/{newItem.Id}", newItem);
});

app.Run();