using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservation.model
{
    public class Room
    {
        public int Id { get; set; }

        public int RoomNumber { get; set; }

        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public List <Reservation> Reservations { get; set; }


    }
}
