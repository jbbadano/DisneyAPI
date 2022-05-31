using System.ComponentModel.DataAnnotations;

namespace DisneyAPI.Models;
    public class Registro
    {
        [Required(ErrorMessage = "Crea tu nombre de usuario.")]
        [StringLength(16, ErrorMessage = "Crea un usuario con al menos 5 caracteres.", MinimumLength = 5)]
        public string Usuario { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingresa la contraseña")]
        [StringLength(16, ErrorMessage = "Crea una contraseña con al menos 5 caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
    }