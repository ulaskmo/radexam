using CsvHelper.Configuration;

namespace Rad302feCL2025.CSVFiles
{
    public class MapMovie : ClassMap<Movie>
    {
        public MapMovie()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Title).Name("Title");
            Map(m => m.ReleaseDate).Name("ReleaseDate");
            Map(m => m.GenreId).Name("Category");
        }
    }

    public class MapActor : ClassMap<Actor>
    {
        public MapActor()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.SecondName).Name("SecondName");

        }
    }

    public class MapGenre : ClassMap<Genre>
    {
        public MapGenre()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
        }
    }

    public class MapActedIn : ClassMap<ActedIn>
    {
        public MapActedIn()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.ActorId).Name("ActorId");
            Map(m => m.MovieId).Name("MovieId");
        }
    }
}