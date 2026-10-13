using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class UpdateRoomDto
    {

        [Required]

        public int RoomNumber { get; set; }
        [Required]
        [Range(1, 10000)]

        public decimal PricePerNight { get; set; }
        [Required]

        public int HotelId { get; set; }
        [Required]


        public bool IsAvailable { get; set; }
        [Required]
        public int RoomTypeId { get; set; }
    }
}
