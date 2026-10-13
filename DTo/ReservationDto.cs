using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class ReservationDto
    {
       
        public int Id { get; set; }

        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int RoomNumber { get; set; }

        public string HotelName { get; set; }

    }
}
