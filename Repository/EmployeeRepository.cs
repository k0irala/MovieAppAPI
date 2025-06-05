using Dapper;
using FluentValidation;
using FluentValidation.Results;
using MovieApplicationApi.Helpers;
using MovieApplicationApi.Signatures;
using Newtonsoft.Json;
using System.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Repository;

public class EmployeeRepository(IDapperRepository repository, IValidator<AddEmployeeDTO> validator, EncryptionHelper encryption, ApiSignature signature) : IEmployeeRepository
{
    private readonly string apiKey = "thisisasecretapikeynotpublic";
    public int AddEmployee(AddEmployeeDTO addEmployee, string clientSignature)
    {
        var requestJson = JsonConvert.SerializeObject(addEmployee);
        bool isValid = signature.IsValidSignature(apiKey, requestJson, clientSignature);
        if (!isValid) return 401;

        ValidationResult validationResult = validator.Validate(addEmployee);
        if (!validationResult.IsValid)
            return -1;
        string encryptedEmail = encryption.Encrypt(addEmployee.Email);
        string encryptedAddress = encryption.Encrypt(addEmployee.Address ?? string.Empty);
        DynamicParameters parameters = new();
        parameters.Add("@name", addEmployee.Name);
        parameters.Add("@email", encryptedEmail);
        parameters.Add("@address", encryptedAddress);
        parameters.Add("@salary", addEmployee.Salary);
        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
        repository.Execute("InsertEmployeeData", parameters);

        int result = parameters.Get<int>("@Result");

        return result;
    }
    public string GetAddEmployeeSignature(AddEmployeeDTO addEmployee)
    {
        ValidationResult validationResult = validator.Validate(addEmployee);
        if (!validationResult.IsValid)
            return "Validation Error Occured";
        var payload = JsonConvert.SerializeObject(addEmployee);
        string apiSignature = signature.ComputeSignature(apiKey, payload, out _);
        return apiSignature;
    }

    public string GetUpdateEmployeeSignature(AddEmployeeDTO updateEmployee)
    {
        ValidationResult validationResult = validator.Validate(updateEmployee);
        if (!validationResult.IsValid)
            return "One or more validation has failed";
        var payload = JsonConvert.SerializeObject(updateEmployee);
        var apiSignature = signature.ComputeSignature(apiKey, payload, out _);
        return apiSignature;
    }

    public int DeleteEmployee(int id)
    {
        DynamicParameters parameters = new();
        parameters.Add("@empId", id);
        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        repository.Execute("DeleteEmployeeData", parameters);

        var result = parameters.Get<int>("@Result");

        return result;
    }
    public List<Employee> GetAllEmployees()
    {
        DynamicParameters dynamicParameters = new();
        var allEmployees = repository.Query<Employee>("GetAllEmployees", dynamicParameters);

        foreach (var employee in allEmployees)
        {
            if (employee.Id >= 14 || employee.Id == 4)
            {
                // Ensure Address is not null before decrypting
                if (!string.IsNullOrEmpty(employee.Address))
                {
                    employee.Address = encryption.Decrypt(employee.Address);
                }
                employee.Email = encryption.Decrypt(employee.Email);
            }
        }

        return [.. allEmployees];
    }

    public Employee GetEmployeeById(int id) // Use nullable reference type for the return type
    {
        DynamicParameters parameters = new();
        parameters.Add("userID", id);
        var employees = repository.QuerySingleOrDefault<Employee>("GetEmployeeById", parameters);

        if (employees == null)
        {
            return new Employee()
            {
                Id = 0,
                Name = "Not Found",
                Email = "Not Found",
                Address = "Not Found",
                Salary = 0
            };
        }

        if (employees.Id >= 14)
        {
            // Ensure Address is not null before decrypting
            if (!string.IsNullOrEmpty(employees.Address))
            {
                employees.Address = encryption.Decrypt(employees.Address);
            }
            employees.Email = encryption.Decrypt(employees.Email);
        }

        return employees;
    }
    public int UpdateEmployee(int id, AddEmployeeDTO updateEmployee, string clientSignature)
    {
        ValidationResult validationResult = validator.Validate(updateEmployee);
        if (!validationResult.IsValid)
            return 401;

        var requestJson = JsonConvert.SerializeObject(updateEmployee);
        bool isValid = signature.IsValidSignature(apiKey, requestJson, clientSignature);
        if (!isValid) return 401;

        string encryptedEmail = encryption.Encrypt(updateEmployee.Email);
        string encryptedAddress = encryption.Encrypt(updateEmployee.Address ?? string.Empty);

        DynamicParameters parameters = new();
        parameters.Add("@empId", id);
        parameters.Add("@name", updateEmployee.Name);
        parameters.Add("@email", encryptedEmail);
        parameters.Add("@salary", updateEmployee.Salary);
        parameters.Add("@address", encryptedAddress);
        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        repository.Execute("UpdateEmployeeData", parameters);

        var result = parameters.Get<int>("@Result");
        return result;
    }
}
