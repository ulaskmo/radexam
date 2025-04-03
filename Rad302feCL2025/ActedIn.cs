using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rad302feCL2025
{
    public  class ActedIn
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public int MovieId { get; set; }

        public virtual ICollection<Actor> Actors { get; set; }
        public virtual ICollection<Movie> Movies { get; set; }
    }
}
