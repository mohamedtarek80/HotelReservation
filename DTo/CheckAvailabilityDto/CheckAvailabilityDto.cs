namespace HotelReservation.DTo.CheckAvailabilityDto
{
    public class CheckAvailabilityDto
    {
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

    }
}
