using DisneyAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DisneyAPI.Schemas
{
    public class SchemaGetMovie
    {
        public string TituloPelicula { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaDeCreacion { get; set; }

        public int Calificacion { get; set; }
        public string ImagenPelicula { get; set; }

        public int IdGenero { get; set; }

        public List<Personaje> Personajes { get; set; }
    }
}