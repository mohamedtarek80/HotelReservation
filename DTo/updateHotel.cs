using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class updateHotelDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]

        public string Description { get; set; }
        [Required]

        public string Address { get; set; }
        [Range(0, 5)]

        public decimal Rating { get; set; }
    }
}
