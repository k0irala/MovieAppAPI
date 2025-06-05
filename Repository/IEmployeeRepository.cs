using MovieApplicationApi.Models;
using MovieApplicationApi.Models.Entities;

namespace MovieApplicationApi.Repository
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAllEmployees();
        int AddEmployee(AddEmployeeDTO addEmployee,string clientSignature);      
        int UpdateEmployee(int id, AddEmployeeDTO updateEmployee,string clientSignature);
        int DeleteEmployee(int id);
        Employee GetEmployeeById(int id);
        string GetAddEmployeeSignature(AddEmployeeDTO addEmployee);

        string GetUpdateEmployeeSignature(AddEmployeeDTO updateEmployee);
    }
}
