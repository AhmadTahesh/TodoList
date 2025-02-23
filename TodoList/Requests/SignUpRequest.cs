namespace TodoList.Requests;

public class SignUpRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    public string PhoneNumber { get; set; }
}