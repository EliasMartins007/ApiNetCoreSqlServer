using herois.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//add elias 13/09 log
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//serilog elias08/09
//remove default logging providers
builder.Logging.ClearProviders();

///teste log banco de dados 13/09/2022
var _serialogLogger = new LoggerConfiguration()
.WriteTo.MSSqlServer(
    configuration.GetConnectionString("DefaultConnection"),
    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
    {
        AutoCreateSqlTable = true,
        TableName = "Serilogs"
    })
.CreateLogger();

var app = builder.Build();
app.UsePathBase("/superhero");
//add
async Task<List<SuperHero>> GetAllHeroes(DataContext context) =>
       await context.Superheros.ToListAsync();

app.UseSwagger();
app.UseSwaggerUI();
//add
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");
app.MapGet("/superhero", async (DataContext context) => 
    await context.Superheros.ToListAsync());

//rota hero id
app.MapGet("/superhero/{id}", async (DataContext context, int id) =>
 await context.Superheros.FindAsync(id) is SuperHero hero ?
 Results.Ok (hero) :
 Results.NotFound("Sorry :("));

app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.Superheros.Add(hero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));//Ok(hero);
});

app.MapPut("/superhero/{id}", async (DataContext context, SuperHero hero, int id) =>
{
    var dbHero = await context.Superheros.FindAsync(id);
    if (dbHero == null)
        return Results.NotFound("No hero found. :(");

    dbHero.Firstname = hero.Firstname;
    dbHero.Lastname = hero.Lastname;
    dbHero.Heroname = hero.Heroname;
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeroes(context));
});

app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.Superheros.FindAsync(id);
    if (dbHero == null)
        return Results.NotFound(" :(");
    context.Superheros.Remove(dbHero);
    return Results.Ok(await GetAllHeroes(context));
}); 


app.Run();
