namespace HotelReservation.model
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string phoneNumber { get; set; }

        public string Role { get; set; }

        public List <Reservation> Reservations { get; set; }
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }







    }
}
