namespace HotelReservation.model
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public decimal Rating { get; set; }

        public ICollection <Room> Rooms { get; set; }
    }
}
