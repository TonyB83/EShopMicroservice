using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<DiscountContext>(opts =>
        opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DiscountContext>();

    // 1. On récupère le chemin du fichier depuis la configuration
    var dbPath = db.Database.GetDbConnection().DataSource;

    // 2. On extrait le nom du dossier parent
    var directory = Path.GetDirectoryName(dbPath);

    // 3. Si le dossier n'est pas vide et n'existe pas, on le crée !
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }

    // 4. Maintenant on peut créer la base et les tables sereinement
    db.Database.Migrate(); // (ou db.Database.EnsureCreated(); si vous n'utilisez pas les migrations)
}

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.UseMigration();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
