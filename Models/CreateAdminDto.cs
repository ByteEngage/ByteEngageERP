using System.ComponentModel.DataAnnotations;

public class CreateAdminDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    [Required]
    public int OrganizationId { get; set; }
    public int AddedBy { get; set; }
    // 🔥 important
}
