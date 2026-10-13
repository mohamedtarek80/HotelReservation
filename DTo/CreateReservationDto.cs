using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class CreateReservationDto
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
       


    }
}
