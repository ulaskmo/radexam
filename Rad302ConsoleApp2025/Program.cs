using Microsoft.EntityFrameworkCore;
using Rad302feCL2025;
using Rad302feCL2025.CSVFiles;
using System.Globalization;
using Tracker.WebAPIClient;

namespace Rad302feConsoleApp2025 // Fixed namespace to match project name
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ActivityAPIClient.Track(StudentID: "S219971", StudentName: "ulas", activityName: "Rad302 fe March 2025", Task: "Q1e Implementing Console queries");

            // Create and ensure the database is created
            using (var context = new MovieContext())
            {
                // Make sure we're using SQLite
                Console.WriteLine("Database provider: " + context.Database.ProviderName);
                
                // Ensure database is created
                context.Database.EnsureCreated();
                
                // Seed the database with data from CSV files
                SeedDatabase(context);
                
                // Query and display the seeded data
                DisplayDatabaseContents(context);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        static void SeedDatabase(MovieContext context)
        {
            Console.WriteLine("Seeding database with data from CSV files...");
            
            // Check if database is already seeded
            if (context.Genres.Any() || context.Movies.Any() || context.Actors.Any() || context.ActedIns.Any())
            {
                Console.WriteLine("Database already contains data. Skipping seeding.");
                return;
            }
            
            try
            {
                // Get the base directory for the CSV files
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string csvDirectory = Path.Combine(baseDirectory, "..", "..", "..", "..", "Rad302feCL2025", "CSVFiles");
                
                // Ensure the directory path is absolute
                csvDirectory = Path.GetFullPath(csvDirectory);
                Console.WriteLine($"Looking for CSV files in: {csvDirectory}");
                
                // Seed Genres
                string genreCsvPath = Path.Combine(csvDirectory, "Genre.csv");
                if (File.Exists(genreCsvPath))
                {
                    var genres = DBHelper.GetFile<Genre, MapGenre>(genreCsvPath);
                    context.Genres.AddRange(genres);
                    context.SaveChanges();
                    Console.WriteLine($"Added {genres.Count} genres to the database.");
                }
                else
                {
                    Console.WriteLine($"Warning: Genre CSV file not found at {genreCsvPath}");
                }
                
                // Seed Movies
                string moviesCsvPath = Path.Combine(csvDirectory, "Movies.csv");
                if (File.Exists(moviesCsvPath))
                {
                    var movies = DBHelper.GetFile<Movie, MapMovie>(moviesCsvPath);
                    context.Movies.AddRange(movies);
                    context.SaveChanges();
                    Console.WriteLine($"Added {movies.Count} movies to the database.");
                }
                else
                {
                    Console.WriteLine($"Warning: Movies CSV file not found at {moviesCsvPath}");
                }
                
                // Seed Actors
                string actorsCsvPath = Path.Combine(csvDirectory, "Actors.csv");
                if (File.Exists(actorsCsvPath))
                {
                    var actors = DBHelper.GetFile<Actor, MapActor>(actorsCsvPath);
                    context.Actors.AddRange(actors);
                    context.SaveChanges();
                    Console.WriteLine($"Added {actors.Count} actors to the database.");
                }
                else
                {
                    Console.WriteLine($"Warning: Actors CSV file not found at {actorsCsvPath}");
                }
                
                // Seed ActedIn relationships
                string actedInCsvPath = Path.Combine(csvDirectory, "ActedIn.csv");
                if (File.Exists(actedInCsvPath))
                {
                    var actedIns = DBHelper.GetFile<ActedIn, MapActedIn>(actedInCsvPath);
                    
                    // Initialize collections for each ActedIn record
                    foreach (var actedIn in actedIns)
                    {
                        actedIn.Actors = new List<Actor>();
                        actedIn.Movies = new List<Movie>();
                        
                        // Find the actor and movie by ID
                        var actor = context.Actors.Find(actedIn.ActorId);
                        var movie = context.Movies.Find(actedIn.MovieId);
                        
                        if (actor != null)
                        {
                            actedIn.Actors.Add(actor);
                        }
                        
                        if (movie != null)
                        {
                            actedIn.Movies.Add(movie);
                        }
                    }
                    
                    context.ActedIns.AddRange(actedIns);
                    context.SaveChanges();
                    Console.WriteLine($"Added {actedIns.Count} acting relationships to the database.");
                }
                else
                {
                    Console.WriteLine($"Warning: ActedIn CSV file not found at {actedInCsvPath}");
                }
                
                Console.WriteLine("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        
        static void DisplayDatabaseContents(MovieContext context)
        {
            // Query example: Get all movies
            var movies = context.Movies.ToList();
            Console.WriteLine($"\nFound {movies.Count} movies in the database:");
            
            foreach (var movie in movies)
            {
                Console.WriteLine($"- {movie.Title} ({movie.ReleaseDate})");
            }
            
            // Query example: Get movies with their genres
            var moviesWithGenres = context.Movies
                .Include(m => m.MovieGenre)
                .ToList();
            
            Console.WriteLine("\nMovies with genres:");
            foreach (var movie in moviesWithGenres)
            {
                var genreName = movie.MovieGenre?.Name ?? "No genre";
                Console.WriteLine($"- {movie.Title}: {genreName}");
            }
            
            // Query example: Get actors
            var actors = context.Actors.ToList();
            
            Console.WriteLine("\nActors:");
            foreach (var actor in actors)
            {
                Console.WriteLine($"- {actor.FirstName} {actor.SecondName}");
            }
            
            // Query example: Get acted in relationships
            var actedInRelationships = context.ActedIns
                .Include(ai => ai.Actors)
                .Include(ai => ai.Movies)
                .ToList();
            
            Console.WriteLine("\nActing relationships:");
            foreach (var actedIn in actedInRelationships)
            {
                var actorNames = string.Join(", ", actedIn.Actors.Select(a => $"{a.FirstName} {a.SecondName}"));
                var movieTitles = string.Join(", ", actedIn.Movies.Select(m => m.Title));
                
                Console.WriteLine($"- Actors: {actorNames} in Movies: {movieTitles}");
            }
        }
    }
} 