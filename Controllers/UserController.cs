using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MovieApplicationApi.Repository;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserRepository userRepository): ControllerBase
    {
        [HttpGet]
        public List<User> GetAll()
        {
            var users = userRepository.GetAllUsers();
            return users;
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid user data provided.");
            }
            
            int result = userRepository.AddUser(user);
            if (result <= 0)
            {
                return BadRequest("Failed to add user.");
            }
            
            return Ok("User added successfully");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            int result = userRepository.DeleteUser(id);
            if (result == -1) return NotFound("The requested user cannot be found");
            else if (result == 0) return BadRequest("The user does not exist or has already been deleted");
            return Ok("User deleted successfully");
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                User user = userRepository.GetUserById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("The requested user cannot be found");
            }
        }  
        [HttpPut("{id}")]
        public IActionResult Update(int id,User user)
        {
            var result = userRepository.UpdateUser(id,user);
            if (result == -1) return NotFound("The requested user cannot be found");
            else if (result == 401) return BadRequest("Invalid user data provided. Please check the input and try again.");
            return Ok("User Updated Successfully");
        }
    }
}
