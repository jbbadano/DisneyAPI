using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyAPI.Schemas
{
    public class SchemaEditMovie
    {
        public string TituloPelicula { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaDeCreacion { get; set; }

        public int Calificacion { get; set; }

        public string ImagenPelicula { get; set; }

        public int IdGenero { get; set; }
    }
}