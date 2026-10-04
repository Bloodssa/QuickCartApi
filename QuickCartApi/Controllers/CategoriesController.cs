using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCartApi.DTOs.Category;
using QuickCartApi.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuickCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await categoryService.GetAllAsync());

        // GET api/<CategoryController>/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            return category is null ? NotFound(new { message = $"Category with id {id} not found"}) : Ok(category);
        }

        // POST api/<CategoryController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCategoryRequest request)
        {
            var created = await categoryService.CreateAsync(request);

            return Created($"/api/category/{created.Id}", created);
        }

        // PUT api/<CategoryController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoryRequest request)
        {
            var updated = await categoryService.UpdateAsync(id, request);

            if (updated is null) return NotFound(new { message = "Category not found."});

            return Ok(updated);
        }

        // DELETE api/<CategoryController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await categoryService.DeleteAsync(id);
            if (!success)
                return error == "Category not found." ? NotFound(new { message = "Category doesn't exists" } ) : BadRequest(new { message = error });

            return NoContent();
        }
    }
}
