using HotelReservation.DTo;
using HotelReservation.DTo.CheckAvailabilityDto;
using HotelReservation.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReservationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<ReservationDto>> CreateReservation(
     CreateReservationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid User ID in token");

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

            if (room == null)
                return NotFound("Room not found");

            if (dto.CheckIn >= dto.CheckOut)
                return BadRequest("Check-out must be after check-in");
            if (dto.CheckIn < DateTime.UtcNow)
                return BadRequest("Check-in date cannot be in the past");

            var hasOverlap = await _context.Reservations
                .AnyAsync(r =>
                    r.RoomId == dto.RoomId &&
                    r.Status != "Cancelled" &&
                    dto.CheckIn < r.CheckOut &&
                    dto.CheckOut > r.CheckIn
                );

            if (hasOverlap)
                return BadRequest("Room is already reserved for these dates");

            var numberOfNights =
                (dto.CheckOut - dto.CheckIn).Days;

            var totalPrice =
                numberOfNights * room.PricePerNight;

            var reservation = new Reservation
            {
                UserId = userId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalPrice = totalPrice,
                Status = "Pending"
            };

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();

            return Ok(new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                RoomId = reservation.RoomId,
                RoomNumber = room.RoomNumber,
                HotelName = room.Hotel.Name,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status
            });
        }

        [HttpGet("myresevation")]
        [Authorize(Roles = "customer")]

        public async Task<ActionResult<List<ReservationDto>>> GetMyReservations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");
            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid User ID in token");
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    RoomId = r.RoomId,
                    RoomNumber = r.Room.RoomNumber,
                    HotelName = r.Room.Hotel.Name,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync();
            return Ok(reservations);
        }
        [HttpPut("{id:int}/cancel")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult> CancelReservation(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");
            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid User ID in token");
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (reservation == null)
                return NotFound("Reservation not found");
            if (reservation.Status == "Cancelled")
                return BadRequest("Reservation is already cancelled");
            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return Ok("Reservation cancelled successfully");
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReservationDto>>> GetAllReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    RoomId = r.RoomId,
                    RoomNumber = r.Room.RoomNumber,
                    HotelName = r.Room.Hotel.Name,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync();
            return Ok(reservations);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReservationDto>> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
                .Where(r => r.Id == id)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    RoomId = r.RoomId,
                    RoomNumber = r.Room.RoomNumber,
                    HotelName = r.Room.Hotel.Name,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .FirstOrDefaultAsync();
            if (reservation == null)
                return NotFound("Reservation not found");
            return Ok(reservation);
        }

        [HttpPut("{id:int}/update-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateReservationStatus(int id, UpdateReservationDto dto  )
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound("Reservation not found");

            if (dto.Status != "Pending" &&
                dto.Status != "Confirmed" &&
                dto.Status != "Cancelled")
            {
                return BadRequest(
                    "Invalid status. Use Pending, Confirmed or Cancelled");
            }

            reservation.Status = dto.Status;
            await _context.SaveChangesAsync();
            return Ok("Reservation status updated successfully");
        }
        [HttpPost("Check_Availability")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckAvailability(CheckAvailabilityDto dto)
        {
            if (dto.CheckIn < DateTime.UtcNow)
                return BadRequest("Check-in date cannot be in the past");
            var roomExists = await _context.Rooms
    .AnyAsync(r => r.Id == dto.RoomId);

            if (!roomExists)
                return NotFound("Room not found");
            if (dto.CheckIn >= dto.CheckOut)
                return BadRequest("Check-out must be after check-in");
            var hasOverlap = await _context.Reservations
                .AnyAsync(r =>
                    r.RoomId == dto.RoomId &&
                    r.Status != "Cancelled" &&
                    dto.CheckIn < r.CheckOut &&
                    dto.CheckOut > r.CheckIn
                );
            return Ok(!hasOverlap);
        }








    }
}
