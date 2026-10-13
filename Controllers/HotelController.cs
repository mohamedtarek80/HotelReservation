using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.model;

using Microsoft.EntityFrameworkCore;
using HotelReservation.DTo;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HotelController(AppDbContext context)
        {
            _context= context;
        }


        [HttpGet]


        [HttpGet]
        public async Task<ActionResult<PagedResultDto<HotelDto>>> GetHotels(
       [FromQuery] string? search,
       [FromQuery] decimal? minRating,
      [FromQuery] string? sortBy,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(
                    "PageNumber and PageSize must be greater than 0");
            }

            if (pageSize > 100)
            {
                return BadRequest(
                    "PageSize cannot be greater than 100");
            }

            var query = _context.Hotels.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h =>
                    h.Name.Contains(search) ||
                    h.Address.Contains(search));
            }

            
            if (minRating.HasValue)
            {
                query = query.Where(h =>
                    h.Rating >= minRating.Value);
            }

           
            var totalCount = await query.CountAsync();

          
            if (sortBy == "rating_desc")
            {
                query = query.OrderByDescending(h => h.Rating)
                             .ThenBy(h => h.Id);
            }
            else if (sortBy == "rating_asc")
            {
                query = query.OrderBy(h => h.Rating)
                             .ThenBy(h => h.Id);
            }
            else
            {
                query = query.OrderBy(h => h.Id);
            }

            
            var hotels = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new HotelDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Address = h.Address,
                    Rating = h.Rating
                })
                .ToListAsync();

            var result = new PagedResultDto<HotelDto>
            {
                Data = hotels,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber <
                    (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }







        [HttpGet("{id:int}")]
        public async Task <ActionResult<HotelDto>>GetHotel (int id)
        {
            var hotel = await _context.Hotels
       .Where(x => x.Id == id)
       .Select(x => new HotelDto
       {
           Id = x.Id,
           Name = x.Name,
           Description = x.Description,
           Address = x.Address,
           Rating = x.Rating
       })
       .FirstOrDefaultAsync();

            if (hotel == null)
                return NotFound();

            return Ok(hotel);


        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task <ActionResult<Hotel>>CreateHotel (CreateHotelDto dto)
        {
            var hotel = new Hotel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Rating = dto.Rating,

            };
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            var result = new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                Rating = hotel.Rating
            };

            return Ok(result);


          
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")] 
        public async Task <ActionResult<Hotel>> UpdateHotel(int id, updateHotelDto dto)
        {
            var existingHotel = await _context.Hotels.FirstOrDefaultAsync(x=>x.Id == id);

            if (existingHotel == null)
                return NotFound();
            existingHotel.Name= dto.Name;
            existingHotel.Description = dto.Description;
            existingHotel.Address= dto.Address;
            existingHotel.Rating= dto.Rating;

          
            await _context.SaveChangesAsync();
            var result = new HotelDto
            {
                Id = existingHotel.Id,
                Name = existingHotel.Name,
                Description = existingHotel.Description,
                Address = existingHotel.Address,
                Rating = existingHotel.Rating
            };

            return Ok(result);
          

           
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Hotel>>DeleteHotel(int id)
        {
            var hotel=await _context.Hotels.FirstOrDefaultAsync( x=>x.Id == id);
            if (hotel == null) return NotFound();

          _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
           return NoContent();
        }
    }
}
