using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger for documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CRUD Endpoints
app.MapPost("/movies", async (MovieContext db, Movie movie) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/movies/{movie.Id}", movie);
});

app.MapGet("/movies", async (MovieContext db) =>
{
    return await db.Movies.ToListAsync();
});

app.MapGet("/movies/{id:int}", async (MovieContext db, int id) =>
{
    var movie = await db.Movies.FindAsync(id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});

app.MapPut("/movies/{id:int}", async (MovieContext db, int id, Movie inputMovie) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();

    movie.Name = inputMovie.Name;
    movie.Genre = inputMovie.Genre;
    movie.DateReleased = inputMovie.DateReleased;
    movie.Director = inputMovie.Director;
    movie.Rating = inputMovie.Rating;

    await db.SaveChangesAsync();
    return Results.Ok(movie);
});

app.MapDelete("/movies/{id:int}", async (MovieContext db, int id) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();

    db.Movies.Remove(movie);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
