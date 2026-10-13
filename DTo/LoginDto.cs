using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
