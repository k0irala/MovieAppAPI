namespace MovieApplicationApi.Models
{
    public class UpdateEmployeeDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }
        public decimal Salary { get; set; }
    }
}
