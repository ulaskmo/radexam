using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rad302feCL2025
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        [ForeignKey("MovieGenre")]
        public int GenreId { get; set; }
        public DateOnly? ReleaseDate { get; set; }

        [JsonIgnore]
        public virtual Genre MovieGenre { get; set; }



    }
}
