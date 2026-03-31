using Microsoft.AspNetCore.Mvc;

namespace ByteEngageERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>();

        // Static constructor to generate data
        static EmployeeController()
        {
            for (int i = 1; i <= 500; i++)
            {
                employees.Add(new Employee
                {
                    Id = i,
                    Name = "Employee " + i,
                    Email = $"employee{i}@company.com",
                    Department = i % 2 == 0 ? "IT" : "HR",
                    Salary = 20000 + (i * 100),
                    City = i % 3 == 0 ? "Delhi" : "Noida"
                });
            }
        }

        // 🔹 GET: api/demo
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(employees);
        }

        // 🔹 GET: api/demo/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null)
                return NotFound("Employee not found");

            return Ok(emp);
        }

        // 🔹 Filtering + Pagination
        // api/demo/filter?department=IT&page=1&pageSize=10
        [HttpGet("filter")]
        public IActionResult Filter(string? department, int page = 1, int pageSize = 10)
        {
            var query = employees.AsQueryable();

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department == department);
            }

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                TotalRecords = total,
                Page = page,
                PageSize = pageSize,
                Data = data
            });
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public int Salary { get; set; }
        public string City { get; set; }
    }
}