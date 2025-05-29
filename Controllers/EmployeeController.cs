using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using Dapper;
using WebApplication1.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using FluentValidation;
using FluentValidation.Results;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController(ApplicationDbContext dbContext,IDapperRepository repository,IValidator<AddEmployeeDTO> validator) : ControllerBase
    {
        [HttpGet]
        public List<Employee>  GetAllEmployees()
        { 
            DynamicParameters dynamicParameters = new DynamicParameters();
            var allEmployees = repository.Query<Employee>("GetAllEmployees", dynamicParameters);
            return allEmployees.ToList();
        }
        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeDTO addEmployee)
        {
           ValidationResult validationResult = validator.Validate(addEmployee);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(s => new { field = s.PropertyName, error = s.ErrorMessage }));

            DynamicParameters parameters = new();
            parameters.Add("@name", addEmployee.Name);
            parameters.Add("@email", addEmployee.Email);
            parameters.Add("@address", addEmployee.Address);
            parameters.Add("@salary", addEmployee.Salary);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            repository.Execute("InsertEmployeeData", parameters);

            int result = parameters.Get<int>("@Result");
            if (result == -1)
            {
                return BadRequest("Error in inserting employee to database");
            }


            //var employees = new Employee()
            //{
            //    Email = addEmployee.Email,
            //    Name = addEmployee.Name,
            //    Salary = addEmployee.Salary,
            //    Address = addEmployee.Address
            //};
            //dbContext.Employees.Add(employees);
            //dbContext.SaveChanges();
            return Ok("Successfully Inserted Employee Data in the database");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("@empId", id);
            parameters.Add("@Result",dbType: DbType.Int32,direction:ParameterDirection.Output);

            repository.Execute("DeleteEmployeeData", parameters);

            var result = parameters.Get<int>("@Result");

            if (result == -1) {

                return NotFound("The requested Employee cannot be found");

            }
            //var existingEmployee = dbContext.Employees.SingleOrDefault(x => x.Id == id);
            //if (existingEmployee == null)
            //{
            //    return BadRequest();
            //}
            //dbContext.Employees.Remove(existingEmployee);
            //dbContext.SaveChanges();
            return Ok("The employee has been deleted successfully");
        }

        [HttpGet("{id}")]

        public IActionResult GetEmployeeById(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("userID", id);
            var employees = repository.QuerySingleOrDefault<Employee>("GetEmployeeById", parameters);

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
            DynamicParameters parameters = new();
            parameters.Add("@empId", id);
            parameters.Add("@name", updateEmployee.Name);
            parameters.Add("@email", updateEmployee.Email);
            parameters.Add("@salary", updateEmployee.Salary);
            parameters.Add("@address", updateEmployee.Address);
            parameters.Add("@Result", dbType:DbType.Int32,direction:ParameterDirection.Output);

            repository.Execute("UpdateEmployeeData", parameters);

            var result = parameters.Get<int>("@Result");
            if (result == -1) {
                return NotFound("The requested employee cannot be found");
            }

            //var existingEmloyee = dbContext.Employees.SingleOrDefault(x => x.Id == id);
            //if(existingEmloyee == null)
            //{
            //    return BadRequest();
            //}
            //existingEmloyee.Name = updateEmployee.Name;
            //existingEmloyee.Salary = updateEmployee.Salary;
            //existingEmloyee.Address = updateEmployee.Address;
            //existingEmloyee.Email = updateEmployee.Email;

            //dbContext.Employees.Update(existingEmloyee);
            //dbContext.SaveChanges();
            return Ok("Employee Updated Successfully");
        }
    }
}
