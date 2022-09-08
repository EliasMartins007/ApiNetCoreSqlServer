using herois.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//add
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//serilog elias08/09
//remove default logging providers
builder.Logging.ClearProviders();
//SeriLog configure
var logger = new LoggerConfiguration()
     .WriteTo.Console()
    .CreateLogger();
//Registra Serilog
builder.Logging.AddSerilog(logger);
//Registra Serilog em File
builder.Logging.AddFile("Logs/minha-app-(Date).txt");


var app = builder.Build();
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

//hero id
app.MapGet("/superhero/{id}", async (DataContext context, int id) =>
 await context.Superheros.FindAsync(id) is SuperHero hero ?
 Results.Ok (hero) :
 Results.NotFound("Sorry :("));

//Post
app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.Superheros.Add(hero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));//Ok(hero);
});

//Put
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

//DELETE
app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.Superheros.FindAsync(id);
    if (dbHero == null)
        return Results.NotFound(" :(");
    context.Superheros.Remove(dbHero);
    return Results.Ok(await GetAllHeroes(context));
}); 


app.Run();
