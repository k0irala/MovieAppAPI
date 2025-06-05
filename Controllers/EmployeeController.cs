using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using MovieApplicationApi.Repository;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeController(IEmployeeRepository empRepository) : ControllerBase
{
    [HttpGet]
    public List<Employee> GetAll()
    {
        List<Employee> employees = empRepository.GetAllEmployees();
        return employees;
    }
    [HttpPost]
    public IActionResult Create(AddEmployeeDTO addEmployee)
    {
        if (!Request.Headers.TryGetValue("X-Signature", out var signatureHeader))
        {
            return BadRequest("Missing X-Signature header.");
        }
        var apiSignature = signatureHeader.ToString();
        int result = empRepository.AddEmployee(addEmployee, apiSignature);
        if (result == -1) return BadRequest("Invalid employee data provided.");
        if (result == 401) return Unauthorized("API signature is not valid");
        return Ok("Employee added successfully");
    }
    [HttpPost("AddEmployeeSignature")]
    public string SignatureForCreate(AddEmployeeDTO addEmployee)
    {
        string apiSignature = empRepository.GetAddEmployeeSignature(addEmployee);
        return apiSignature;
    }

    [HttpPost("UpdateEmployeeSignature")]
    public string SignatureforUpdate(AddEmployeeDTO updateEmployee)
    {
        string apiSignature = empRepository.GetUpdateEmployeeSignature(updateEmployee);
        return apiSignature;
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        int result = empRepository.DeleteEmployee(id);
        if (result == -1) return NotFound("The requested employee cannot be found");
        else if (result == 0) return BadRequest("The employee does not exist or has already been deleted");
        return Ok("Employee deleted successfully");
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Employee employee = empRepository.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound("The requested employee cannot be found");
        }
        return Ok(employee);
    }
    [HttpPut("{id}")]
    public IActionResult Update(int id, AddEmployeeDTO updateEmployee)
    {
        if (!Request.Headers.TryGetValue("X-Signature", out var signatureHeader))
        {
            return BadRequest("Missing X-Signature header.");
        }
        var apiSignature = signatureHeader.ToString();
        int result = empRepository.UpdateEmployee(id, updateEmployee,apiSignature);
        if (result == 0) return Conflict("The email of the user is already registered!!");
        if (result == -1) return NotFound("The requested employee cannot be found");
        else if(result == 401) return BadRequest("Invalid employee data provided. Please check the input and try again.");
        return Ok("Employee Updated Successfully");
    }
}
