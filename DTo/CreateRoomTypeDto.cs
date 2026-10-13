using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class CreateRoomTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
       
    }
}
