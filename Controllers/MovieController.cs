using LiteDB;
using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Database;

namespace MovieDatabaseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : Controller
    {
        private readonly ILiteDatabase _liteDb;

        public MovieController(ILiteDatabase liteDb)
        {
            _liteDb = liteDb;
        }

        // GET: movie
        // Retrieves all movies
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> Get([FromQuery] string? name = null, 
            [FromQuery] string? genre = null,
            [FromQuery] string? director = null,
            [FromQuery] string? distributor = null,
            [FromQuery] int? releaseYear = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20
            )
        {
            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            // Filter movies based on the provided parameters
            var query = collection.FindAll().AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(m => m.Name!.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(m => m.Genre!.Contains(genre, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(director))
            {
                query = query.Where(m => m.Director!.Contains(director, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(distributor))
            {
                query = query.Where(m => m.DistributedBy!.Contains(distributor, StringComparison.OrdinalIgnoreCase));
            }

            if (releaseYear.HasValue)
            {
                query = query.Where(m => m.ReleaseDate!.Value.Year == releaseYear.Value);
            }

            // Apply pagination
            int skip = (page - 1) * pageSize;
            var paginatedMovies = query.Skip(skip).Take(pageSize).ToList();

            // Check if there are any results
            if (paginatedMovies.Count == 0)
            {
                return NotFound("No movies found with the specified criteria.");
            }

            // Total number of records for client information
            int totalRecords = query.Count();

            // Return the paginated results along with total record count for client-side use
            return Ok(new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                Movies = paginatedMovies
            });
        }

        // GET: movie/{id}
        [HttpGet("id/{id:int}")]
        public ActionResult<Movie> GetById(int id)
        {
            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            var movie = collection.FindById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // GET: movie/{ids}
        [HttpGet("GetByIds")]
        public ActionResult<IEnumerable<Movie>> GetByIds([FromQuery] int[] ids)
        {
            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            // Find all movies with IDs matching those in the input array
            var movies = collection.Find(m => ids.Contains(m.Id)).ToList();

            // If no movies are found, return 404
            if (movies == null || !movies.Any())
            {
                return NotFound("No movies found with the given IDs.");
            }

            // Return the list of movies
            return Ok(movies);
        }

        // GET: movie/{name}
        [HttpGet("name/{name}")]
        public ActionResult<IEnumerable<Movie>> GetByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return NotFound(new { Message = $"Requested name cannot be null or empty" });
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            var movies = collection.Find(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (movies.Count == 0)
            {
                return NotFound();
            }

            return Ok(movies);
        }
        
        // POST: movie
        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            // Validate the movie object
            if (movie == null)
            {
                return BadRequest("Movie data is required.");
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            // Check for existing movie with the same ID
            var existingMovie = collection.FindById(movie.Id);
            if (existingMovie != null)
            {
                return Conflict("A movie with this ID already exists."); // Return 409 Conflict
            }

            // Insert the new movie
            collection.Insert(movie);

            return CreatedAtAction(nameof(GetById), new { Id = movie.Id }, movie);
        }

        // POST: movie
        [HttpPost("BulkInsert")]
        public IActionResult PostBulk([FromBody] Movie[] movies)
        {
            // Validate the movie object
            if (movies == null)
            {
                return BadRequest("Movie data is required.");
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            foreach (var movie in movies)
            {
                if (movie == null)
                    continue;

                // Check for existing movie with the same ID
                var existingMovie = collection.FindById(movie.Id);
                if (existingMovie != null)
                    continue;

                // Insert the new movie
                collection.Insert(movie);
            }

            return Ok("Bulk insert completed successfully.");
        }

        // PUT movie
        [HttpPut]
        public IActionResult Put([FromBody] Movie updatedMovie)
        {
            // Validate the movie object
            if (updatedMovie == null)
            {
                return BadRequest("Movie data is required.");
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            // Find the existing movie by ID
            var existingMovie = collection.FindOne(m => m.Id == updatedMovie.Id);
            if (existingMovie == null)
            {
                return NotFound("Movie not found.");
            }

            // Update the movie
            if (!string.IsNullOrEmpty(updatedMovie.Name))
                existingMovie.Name = updatedMovie.Name;

            if(!string.IsNullOrEmpty(updatedMovie.Description))
                existingMovie.Description = updatedMovie.Description;

            if (!string.IsNullOrEmpty(updatedMovie.Genre))
                existingMovie.Genre = updatedMovie.Genre;

            if(updatedMovie.ReleaseDate.HasValue)
                existingMovie.ReleaseDate = updatedMovie.ReleaseDate.Value;

            if (!string.IsNullOrEmpty(updatedMovie.Director))
                existingMovie.Director = updatedMovie.Director;

            if (!string.IsNullOrEmpty(updatedMovie.DistributedBy))
                existingMovie.DistributedBy = updatedMovie.DistributedBy;

            if(updatedMovie.Budget > 0)
                existingMovie.Budget = updatedMovie.Budget;

            if (!string.IsNullOrEmpty(updatedMovie.CoverUrl))
                existingMovie.CoverUrl = updatedMovie.CoverUrl;

            // Save the changes
            collection.Update(existingMovie);

            return NoContent();
        }

        // DELETE: movie/{id}
        [HttpDelete]
        public IActionResult Delete([FromQuery] int[] ids)
        {
            if(ids == null)
            {
                return BadRequest("id is null.");
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            foreach (var id in ids)
            {
                collection.Delete(id);
            }

            return NoContent();
        }

        // DELETE: movie/{name}
        [HttpDelete("name/{name}")]
        public IActionResult Delete(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return BadRequest("name is null.");
            }

            var collection = _liteDb.GetCollection<Movie>(Collection.MOVIES);

            var movies = collection.Find(m => m.Name == name);

            if (!movies.Any())
            {
                return StatusCode(500, new { Message = $"No movies found with the name '{name}'." });
            }

            int deleteNum = collection.DeleteMany(m => m.Name == name);

            return StatusCode(202, new { Message = $"{deleteNum} movies were deleted." });
        }
    }
}
