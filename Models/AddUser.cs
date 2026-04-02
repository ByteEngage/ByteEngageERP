namespace ByteEngageERP.Models;

public class AddUser
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int? AddedBy { get; set; }
}