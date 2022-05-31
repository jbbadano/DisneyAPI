using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisneyAPI.Models;
    public class Personaje
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdPersonaje { get; set; }

        public string ImagenPersonaje { get; set; }

        [Required(ErrorMessage = "Por favor rellene este campo.")]
        [StringLength(30)]
        public string NombrePersonaje { get; set; }

        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; }

        public int IdPelicula { get; set; }

        [ForeignKey("IdPelicula")]
        public Pelicula Pelicula { get; set; }
    }