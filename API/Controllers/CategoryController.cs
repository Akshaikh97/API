using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Context _context;

        public CategoryController(Context context)
        {
            _context = context;
        }
        // If your method primarily returns a specific type but may also need to return different status codes, ActionResult<T>
        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = await _context.Category.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("GetCategory")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
                return NotFound(new { Message = $"Category with ID {id} not found." });

            return Ok(category);
        }

        // [HttpPost("AddCategory")]
        // public async Task<ActionResult<Category>> CreateCategory(Category category)
        // {
        //     _context.Category.Add(category);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        // }

        // [HttpPut("UpdateCategory")]
        // public async Task<IActionResult> UpdateCategory(int id, Category category)
        // {
        //     if (id != category.Id)
        //         return BadRequest(new { Message = "Category ID mismatch." });

        //     _context.Entry(category).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!CategoryExists(id))
        //             return NotFound(new { Message = $"Category with ID {id} not found." });
        //         else
        //             throw;
        //     }

        //     return NoContent();
        // }
        
        // POST: api/categories/UpsertCategory
        [HttpPost("UpsertCategory")]
        public async Task<ActionResult<Category>> UpsertCategory(Category category)
        {
            var existingCategory = await _context.Category.FindAsync(category.Id);

            if (existingCategory == null)
            {
                // Insert new category
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category); // 201 Created
            }
            else
            {
                existingCategory.CategoryName = category.CategoryName;
              
                _context.Entry(existingCategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(existingCategory); // 200 OK
            }
        }
        //If your method may return different response types based on different conditions (e.g., success, failure, not found), 
        //IActionResult is suitable.
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
                return NotFound(new { Message = $"Category with ID {id} not found." });

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
