#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsedAPI.Data;
using UsedAPI.Dto;
using UsedAPI.Models;

namespace UsedAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly DataContext _context;

        public ItemController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }
        
        [Authorize]
        [HttpGet("seller")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsBySeller()
        {
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (currentUserId == null) return BadRequest();

            if (!Int32.TryParse(currentUserId.Value, out int currentUserIdInt)) return BadRequest();

            var items = _context.Items.Where(item => item.SellerId == currentUserIdInt);
            
            return await items.ToListAsync();
        }

        // PUT: api/Item/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemDto itemDto)
        {   
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (currentUserId == null) return BadRequest();

            if (!Int32.TryParse(currentUserId.Value, out int currentUserIdInt)) return BadRequest();
            
            var item = await _context.Items.FindAsync(id);

            if (item == null) return NotFound();

            if (item.SellerId != currentUserIdInt) return Unauthorized();

            item.Name = itemDto.Name;
            item.Price = itemDto.Price;
            item.Category = itemDto.Category;
            item.Description = itemDto.Description;
            item.PicUrl = itemDto.PicUrl;
            item.Location = itemDto.Location;
            item.Condition = itemDto.Condition;
            
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Item
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(ItemDto itemDto)
        {
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (currentUserId == null) return BadRequest();

            if (!Int32.TryParse(currentUserId.Value, out int currentUserIdInt)) return BadRequest();

            Item item = new Item
            {
                Name = itemDto.Name,
                Condition = itemDto.Condition,
                Category = itemDto.Category,
                Date = DateTime.Now,
                Price = itemDto.Price,
                Description = itemDto.Description,
                Location = itemDto.Location,
                PicUrl = itemDto.PicUrl,
                SellerId = currentUserIdInt
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }
        
        // DELETE: api/Item/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (currentUserId == null) return BadRequest();

            if (!Int32.TryParse(currentUserId.Value, out int currentUserIdInt)) return BadRequest();
            
            var item = await _context.Items.FindAsync(id);

            if (item == null) return NotFound();

            if (item.SellerId != currentUserIdInt) return Unauthorized();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
