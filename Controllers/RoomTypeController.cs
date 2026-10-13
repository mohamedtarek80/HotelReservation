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
    public class RoomTypeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoomTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoomTypeDto>>> GetRoomTypes()
        {
            var roomTypes = await _context.RoomTypes.Select(x => new RoomTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();
            return Ok(roomTypes);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomTypeDto>> GetRoomType(int id)
        {
            var roomType = await _context.RoomTypes.Where(x => x.Id == id).Select(x => new RoomTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).FirstOrDefaultAsync();
            if (roomType == null)
                return NotFound();
            return Ok(roomType);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<RoomTypeDto>> CreateRoomType(CreateRoomTypeDto dto)
        {
            var roomType = new RoomType()
            {
                Name = dto.Name,
                Description = dto.Description
            };
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
            return Ok(new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoomTypeDto>> UpdateRoomType(int id, UpdateRoomTypeDto dto)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
                return NotFound();
            roomType.Name = dto.Name;
            roomType.Description = dto.Description;
            await _context.SaveChangesAsync();
            return Ok(new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteRoomType(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
                return NotFound();
            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
