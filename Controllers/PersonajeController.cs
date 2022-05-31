using DisneyAPI.Data;
using DisneyAPI.Models;
using DisneyAPI.Response;
using DisneyAPI.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisneyAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonajeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonajeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Eliminar un personaje:
        [HttpDelete("{id}")]
        public ActionResult<Personaje> Delete(int id)
        {
            var personaje = _context.Personajes.Find(id);
            if (personaje == null)
            {
                return NotFound();
            }
            _context.Personajes.Remove(personaje);
            _context.SaveChanges();

            return personaje;
        }

        // Lista de personajes:
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<ResponseApi> Get()
        {
            var respuesta = new ResponseApi();
            try
            {
                var personajes = _context.Personajes.ToList();

                if (personajes.Count == 0)
                {
                    throw new Exception("No hay personajes para mostrar");
                }

                var personajesList = new List<SchemaGetPersonajes>();

                foreach (var personaje in personajes)
                {
                    personajesList.Add(new SchemaGetPersonajes()
                    {
                        ImagenPersonaje = personaje.ImagenPersonaje,
                        NombrePersonaje = personaje.NombrePersonaje
                    });
                }

                respuesta.Ok = true;
                respuesta.Return = personajesList;
                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "404 - " + error.Message;

                return respuesta;
            }
        }

        // Detalle del personaje:
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<Personaje> Get(int id)
        {
            var personaje = _context.Personajes.Find(id);
            if (personaje == null)
            {
                return NotFound();
            }
            _context.Entry(personaje).Reference(x => x.Pelicula).Load();
            _context.Entry(personaje.Pelicula).Reference(x => x.genero).Load();

            return personaje;
        }

        // Personaje por edad:
        [HttpGet]
        [Route("age")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetByAge([FromQuery] int age)
        {
            var respuesta = new ResponseApi();
            try
            {
                var personajes = _context.Personajes.Where(k => k.Edad == age).ToList();
                if (personajes.Count == 0)
                {
                    throw new Exception("Edad: " + age.ToString());
                }

                foreach (var personaje in personajes)
                {
                    _context.Entry(personaje).Reference(x => x.Pelicula).Load();
                    _context.Entry(personaje.Pelicula).Reference(x => x.genero).Load();
                }
                respuesta.Ok = true;
                respuesta.Return = personajes;
                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "404 - El personaje no existe: " + error.Message;

                return respuesta;
            }
        }

        // Personaje por película:
        [HttpGet]
        [Route("movie")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetByIdMovie([FromQuery] int idMovie)
        {
            var respuesta = new ResponseApi();
            try
            {
                var personajes = _context.Personajes.Where(k => k.IdPelicula == idMovie).ToList();

                if (personajes.Count == 0)
                {
                    throw new Exception("idMovie: " + idMovie.ToString());
                }

                foreach (var personaje in personajes)
                {
                    _context.Entry(personaje).Reference(x => x.Pelicula).Load();

                    _context.Entry(personaje.Pelicula).Reference(x => x.genero).Load();
                }
                respuesta.Ok = true;
                respuesta.Return = personajes;

                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "404 - El personaje no existe: " + error.Message;

                return respuesta;
            }
        }

        // Personaje por nombre:
        [HttpGet]
        [Route("name")]
        [AllowAnonymous]
        public ActionResult<ResponseApi> GetByName([FromQuery] string name)
        {
            var respuesta = new ResponseApi();
            try
            {
                var personajes = _context.Personajes.Where(k => k.NombrePersonaje.Contains(name)).ToList();
                if (personajes.Count == 0)
                {
                    throw new Exception("Name: " + name);
                }

                foreach (var personaje in personajes)
                {
                    _context.Entry(personaje).Reference(x => x.Pelicula).Load();
                    _context.Entry(personaje.Pelicula).Reference(x => x.genero).Load();
                }

                respuesta.Ok = true;
                respuesta.Return = personajes;
                return respuesta;
            }
            catch (Exception error)
            {
                respuesta.Ok = false;
                respuesta.Error = "404 - El personaje no existe: " + error.Message;

                return respuesta;
            }
        }

        [HttpPost("create")]
        public ActionResult<ResponseApi> Post([FromBody] SchemaCreatePersonaje personaje)
        {
            var respuesta = new ResponseApi();
            var personaje = new Personaje();
            personaje.NombrePersonaje = personaje.NombrePersonaje;
            personaje.ImagenPersonaje = personaje.ImagenPersonaje;
            personaje.Edad = personaje.Edad;
            personaje.Peso = personaje.Peso;
            personaje.Historia = personaje.Historia;
            personaje.IdPelicula = personaje.IdPelicula;

            _context.Personajes.Add(personaje);
            _context.SaveChanges();
            respuesta.Ok = true;
            respuesta.Return = personaje;
            _context.Entry(personaje).Reference(x => x.Pelicula).Load();
            return respuesta;
        }

        // Características del personaje:
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] SchemaEditPersonaje personaje)
        {
            var personaje = _context.Personajes.Find(id);

            if (personaje is null)
            {
                return BadRequest();
            }

            personaje.NombrePersonaje = personaje.NombrePersonaje;
            personaje.ImagenPersonaje = personaje.ImagenPersonaje;
            personaje.Edad = personaje.Edad;
            personaje.Peso = personaje.Peso;
            personaje.Historia = personaje.Historia;
            personaje.IdPelicula = personaje.IdPelicula;


            _context.Entry(personaje).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }
    }
}