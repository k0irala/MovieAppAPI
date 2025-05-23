namespace WebApplication1.Models
{
    public class AddEmployeeDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }
        public decimal Salary { get; set; }
    }
}
