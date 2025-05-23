using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using Dapper;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(ApplicationDbContext dbContext,IDapperRepository repository,WebApplication app) : ControllerBase
    {
        [HttpGet]
        public static void  GetAllEmployees(WebApplication app)
        {
            app.MapGet("api/Employee", (IDapperRepository repository) => { 
            DynamicParameters dynamicParameters = new DynamicParameters();
            var allEmployees = repository.Query<Employee>("GetAllEmployees", dynamicParameters);
            return allEmployees.ToList();
            });
        }
        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeDTO addEmployee)
        {
            var employees = new Employee()
            {
                Email = addEmployee.Email,
                Name = addEmployee.Name,
                Salary = addEmployee.Salary,
                Address = addEmployee.Address
            };
            dbContext.Employees.Add(employees);
            dbContext.SaveChanges();
            return Ok(employees);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var existingEmployee = dbContext.Employees.SingleOrDefault(x => x.Id == id);
            if (existingEmployee == null)
            {
                return BadRequest();
            }
            dbContext.Employees.Remove(existingEmployee);
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("{id}")]

        public IActionResult GetEmployeeById(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("userID", id);
            var employees = repository.Query<Employee>("GetEmployeeById", parameters);
            //var employees = dbContext.Employees.SingleOrDefault(x => x.Id == id);
            if (employees == null)
            {
                return BadRequest();
            }
            return Ok(employees);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id,AddEmployeeDTO updateEmployee)
        {
            var existingEmloyee = dbContext.Employees.SingleOrDefault(x => x.Id == id);
            if(existingEmloyee == null)
            {
                return BadRequest();
            }
            existingEmloyee.Name = updateEmployee.Name;
            existingEmloyee.Salary = updateEmployee.Salary;
            existingEmloyee.Address = updateEmployee.Address;
            existingEmloyee.Email = updateEmployee.Email;

            dbContext.Employees.Update(existingEmloyee);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
