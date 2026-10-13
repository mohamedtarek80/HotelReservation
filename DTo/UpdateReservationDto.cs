using System.ComponentModel.DataAnnotations;

namespace HotelReservation.DTo
{
    public class UpdateReservationDto
    {
        [Required]

        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [Required]

        public decimal TotalPrice { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
