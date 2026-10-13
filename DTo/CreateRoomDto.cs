using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class CreateRoomDto
    {
        [Required]
        public int RoomNumber { get; set; }

        [Required]
        [Range(1, 10000)]

        public decimal PricePerNight { get; set; }
        [Range(1,int.MaxValue)]
        public int HotelId { get; set; }
        
        public bool IsAvailable { get; set; }
        [Range(1,int.MaxValue)]
        public int RoomTypeId { get; set; }
    }
}
