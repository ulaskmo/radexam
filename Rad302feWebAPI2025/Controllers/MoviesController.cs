using Microsoft.AspNetCore.Mvc;
using Rad302feCL2025;
using Rad302feCL2025.Repositories;
using System.Collections.Generic;

namespace Rad302feWebAPI2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IRepositoryFactory repositoryFactory)
        {
            _movieRepository = repositoryFactory.CreateMovieRepository();
        }

        // GET: api/Movies
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetMovies()
        {
            return Ok(_movieRepository.GetAllMovies());
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public ActionResult<Movie> GetMovie(int id)
        {
            var movie = _movieRepository.GetMovieById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // GET: api/Movies/genre/5
        [HttpGet("genre/{genreId}")]
        public ActionResult<IEnumerable<Movie>> GetMoviesByGenre(int genreId)
        {
            return Ok(_movieRepository.GetMoviesByGenre(genreId));
        }

        // POST: api/Movies
        [HttpPost]
        public ActionResult<Movie> PostMovie(Movie movie)
        {
            _movieRepository.AddMovie(movie);
            _movieRepository.SaveChanges();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public IActionResult PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _movieRepository.UpdateMovie(movie);
            _movieRepository.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            _movieRepository.DeleteMovie(id);
            _movieRepository.SaveChanges();

            return NoContent();
        }
    }
}
