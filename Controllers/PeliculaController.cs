using System;
using System.Collections.Generic;
using System.Linq;
using DisneyAPI.Data;
using DisneyAPI.Models;
using DisneyAPI.Response;
using DisneyAPI.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DisneyAPI.Controllers;
    [Route("[controller]")]
    [ApiController]
    public class PeliculaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PeliculaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Eliminar película (delete):
        [HttpDelete("{id}")]
        public ActionResult<Pelicula> Delete(int id)
        {
            var pelicula = _context.Pelicula.Find(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            _context.Pelicula.Remove(pelicula);
            _context.SaveChanges();

            return pelicula;
        }

        // Listado de películas (read/retrieve):
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<ResponseApi> Get()
        {
            var respuesta = new ResponseApi();

            try
            {
                var peliculas = _context.Pelicula.ToList();
                if (peliculas.Count == 0)
                {
                    throw new Exception("La base de datos no contiene ninguna película aún.");
                }
                var listado = new List<SchemaGetAllMovies>();

                foreach (var pelicula in peliculas)
                {
                    listado.Add(new SchemaGetAllMovies()
                    {
                        TituloPelicula = pelicula.TituloPelicula,
                        FechaDeCreacion = pelicula.FechaDeCreacion,
                        ImagenPelicula = pelicula.ImagenPelicula
                    });

                }

                respuesta.Ok = true;
                respuesta.Return = listado;
                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "Error: No encontrado" + error.Message;

                return respuesta;
            }
        }

        // Película y género:
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<ResponseApi> Get(int id)
        {
            var respuesta = new ResponseApi();

            try
            {
                var pelicula = _context.Pelicula.Find(id);

                if (pelicula == null)
                {
                    throw new Exception("Id: " + id.ToString());
                }

                var movie = new SchemaGetMovie()
                {
                    TituloPelicula = pelicula.TituloPelicula,
                    ImagenPelicula = pelicula.ImagenPelicula,
                    Calificacion = pelicula.Calificacion,
                    FechaDeCreacion = pelicula.FechaDeCreacion,
                    IdGenero = pelicula.IdGenero
                };
                pelicula.Personajes = _context.Personajes.Where(x => x.IdPelicula == pelicula.IdPelicula).ToList();

                respuesta.Ok = true;
                respuesta.Return = movie;
                return respuesta;

            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "Esta película no existe en nuestra base de datos." + error.Message;

                return respuesta;
            }
        }

        // Obtener película por género.
        [HttpGet]
        [Route("genero")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetByGenero([FromQuery] int genero)
        {
            var respuesta = new ResponseApi();
            try
            {
                var peliculas = _context.Pelicula.Where(k => k.genero.IdGenero == genero).ToList();

                if (peliculas.Count == 0)
                {
                    throw new Exception("idGenero: " + genero.ToString());
                }

                foreach (var pelicula in peliculas)
                {
                    _context.Entry(pelicula).Reference(x => x.genero).Load();
                }
                respuesta.Ok = true;
                respuesta.Return = peliculas;

                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "Este género no existe en nuestra base de datos." + error.Message;

                return respuesta;
            }
        }

        // Obtener película por nombre.
        [HttpGet]
        [Route("name")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetByName([FromQuery] string name)
        {
            var respuesta = new ResponseApi();
            try
            {
                var peliculas = _context.Pelicula.Where(k => k.TituloPelicula == name).ToList();
                if (peliculas.Count == 0)
                {
                    throw new Exception("Nombre: " + name);
                }

                foreach (var pelicula in peliculas)
                {
                    _context.Entry(pelicula).Reference(x => x.genero).Load();
                }
                respuesta.Ok = true;
                respuesta.Return = peliculas;
                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "El título de la película no existe." + error.Message;

                return respuesta;
            }
        }

        [HttpGet]
        [Route("order")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetWithOrder([FromQuery] string order)
        {
            var respuesta = new ResponseApi();
            try
            {
                List<Pelicula> movies = new List<Pelicula>();

                if (order.ToLower() == "asc")
                {
                    peliculas = _context.Pelicula.OrderBy(x => x.FechaDeCreacion).ToList();
                }
                else if (order.ToLower() == "desc")
                {
                    movies = _context.Pelicula.OrderByDescending(x => x.FechaDeCreacion).ToList();
                }
                else
                {
                    throw new Exception("Debe utilizar ASC o DESC.");
                }

                foreach (var pelicula in peliculas)
                {
                    _context.Entry(pelicula).Reference(x => x.genero).Load();
                }
                respuesta.Ok = true;
                respuesta.Return = peliculas;

                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "Error al ordenar." + error.Message;

                return respuesta;
            }
        }

        // Añadir una nueva película (create):
        [HttpPost("create")]
        public ActionResult<ResponseApi> Post([FromBody] SchemaCreateMovie movie)
        {
            var respuesta = new ResponseApi();
            var pelicula = new Pelicula();
            pelicula.TituloPelicula = movie.TituloPelicula;
            pelicula.FechaDeCreacion = movie.FechaDeCreacion;
            pelicula.Calificacion = movie.Calificacion;
            pelicula.ImagenPelicula = movie.ImagenPelicula;
            pelicula.IdGenero = movie.IdGenero;

            _context.Pelicula.Add(pelicula);
            _context.SaveChanges();
            respuesta.Ok = true;
            respuesta.Return = pelicula;
            _context.Entry(pelicula).Reference(x => x.genero).Load();
            return respuesta;
        }

        // Modificar una película (Update):
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] SchemaEditMovie movie)
        {
            var pelicula = _context.Pelicula.Find(id);
            if (pelicula is null)
            {
                return BadRequest();
            }
            pelicula.TituloPelicula = movie.TituloPelicula;
            pelicula.FechaDeCreacion = movie.FechaDeCreacion;
            pelicula.Calificacion = movie.Calificacion;
            pelicula.ImagenPelicula = movie.ImagenPelicula;
            pelicula.IdGenero = movie.IdGenero;

            _context.Entry(pelicula).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }
    }