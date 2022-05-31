using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisneyAPI.Models;
    public class Pelicula
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdPelicula { get; set; }

        public string ImagenPelicula { get; set; }

        [Required(ErrorMessage = "Por favor rellene este campo.")]
        [StringLength(100)]
        public string TituloPelicula { get; set; }

        public DateTime FechaDeCreacion { get; set; }

        [Required(ErrorMessage = "Por favor rellene este campo.")]
        [Range(0, 5)]
        public int Calificacion { get; set; }

        public int IdGenero { get; set; }

        [ForeignKey("IdGenero")]
        public Genero genero { get; set; }
    }