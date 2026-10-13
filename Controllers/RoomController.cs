using HotelReservation.DTo;
using HotelReservation.model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<RoomDto>>> GetRoom()
        {

            var Rooms = await _context.Rooms.Select(x => new RoomDto

            {
                Id = x.Id,
                RoomNumber = x.RoomNumber,
                IsAvailable = x.IsAvailable,
                HotelId = x.HotelId,
                PricePerNight = x.PricePerNight,

            }).ToListAsync();

            return Ok(Rooms);


        }
        [HttpGet("{id:int}")]

        public async Task <ActionResult<RoomDto>>GetRoom(int id)
        {
            var Room = await _context.Rooms.Where(x => x.Id == id).Select(x => new RoomDto
            {
                Id = x.Id,
                RoomNumber = x.RoomNumber,
                IsAvailable = x.IsAvailable,
                HotelId = x.HotelId,
                PricePerNight = x.PricePerNight,


            }).FirstOrDefaultAsync();
            if (Room == null) 
                return NotFound();
            return Ok(Room);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task <ActionResult<RoomDto>>createRoom (CreateRoomDto dto)
        {
            var Room = new Room()
            {
                
                RoomNumber = dto.RoomNumber,
                IsAvailable = dto.IsAvailable,
                PricePerNight = dto.PricePerNight,
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
            };
            _context.Rooms.Add(Room);
            await _context.SaveChangesAsync();
            var result = new RoomDto
            {
                Id =Room.Id,
                RoomNumber = Room.RoomNumber,
                IsAvailable = Room.IsAvailable,
                PricePerNight = Room.PricePerNight,
                HotelId = Room.HotelId,
                RoomTypeId=Room.RoomTypeId,
            };
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut ("{id:int}")]
        public async Task<ActionResult<RoomDto>> UpdateRoom (int id,UpdateRoomDto dto)
        {
            var existingRoom =await _context.Rooms.FirstOrDefaultAsync( r => r.Id== id);
            if (existingRoom == null) return NotFound();
            existingRoom.RoomNumber = dto.RoomNumber;   
            existingRoom.IsAvailable = dto.IsAvailable;
            existingRoom .PricePerNight = dto.PricePerNight;
            existingRoom.HotelId = dto.HotelId;
            existingRoom.RoomTypeId = dto.RoomTypeId;
            await _context.SaveChangesAsync();
            var result = new RoomDto
            {
                Id = existingRoom.Id,
                RoomNumber = existingRoom.RoomNumber,
                IsAvailable = existingRoom.IsAvailable,
                PricePerNight = existingRoom.PricePerNight,
                HotelId = existingRoom.HotelId,
                RoomTypeId = existingRoom.RoomTypeId,
            };

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRoom (int id)
        {
            var Room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (Room == null) return NotFound();

            _context .Rooms.Remove(Room);
            await _context.SaveChangesAsync();
            return NoContent();
               

        }

    }
}
