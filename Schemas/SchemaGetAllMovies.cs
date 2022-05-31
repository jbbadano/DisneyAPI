using System;
using System.ComponentModel.DataAnnotations;

namespace DisneyAPI.Schemas
{
    public class SchemaGetAllMovies
    {
        public string TituloPelicula { get; set; }
        public DateTime FechaDeCreacion { get; set; }
        public string ImagenPelicula { get; set; }

    }
}