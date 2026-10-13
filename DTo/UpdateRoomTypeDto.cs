using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class UpdateRoomTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]

        public string Description { get; set; }
    }
}
