using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace HotelReservation.model
{
    public class Reservation
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

    }
}
