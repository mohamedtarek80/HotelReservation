using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
       
        [Required]
        [Phone]
        public string phoneNumber { get; set; }
    }
}
