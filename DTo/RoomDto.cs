namespace HotelReservation.DTo
{
    public class RoomDto
    {
        public int Id { get; set; }

        public int RoomNumber { get; set; }

        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
    }
}
