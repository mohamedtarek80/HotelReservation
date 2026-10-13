using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
