using System.ComponentModel.DataAnnotations;

namespace DisneyAPI.Models;
    public class Login
    {
        [Required(ErrorMessage = "Ingresa tu nombre de usuario.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Ingresa tu contraseña.")]
        public string Contraseña { get; set; }
    }