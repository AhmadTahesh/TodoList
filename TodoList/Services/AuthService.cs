using Supabase.Gotrue;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Services;

public class AuthService : IAuthService
{
    private readonly ISupabaseService _supabaseService;
    private readonly IConfiguration _configuration;

    public AuthService(ISupabaseService supabaseService, IConfiguration configuration)
    {
        _supabaseService = supabaseService;
        _configuration = configuration;
    }

    public async Task<Session> SignUp(SignUpRequest request)
    {
        try
        {
            var client = _supabaseService.GetClient();
            var options = new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "display_name", request.DisplayName },
                    { "phone", request.PhoneNumber }
                }
            };

            var response = await client.Auth.SignUp(request.Email, request.Password, options);
            
            if (response?.User == null)
                throw new Exception("User already registered");

            return response;
        }
        catch (Supabase.Gotrue.Exceptions.GotrueException ex) when (ex.Message.Contains("User already registered") || ex.Message.Contains("duplicate key"))
        {
            throw new Exception("User already registered");
        }
    }

    public async Task<Session> SignIn(SignInRequest request)
    {
        var client = _supabaseService.GetClient();
        var response = await client.Auth.SignIn(request.Email, request.Password);
        return response;
    }
    

    public async Task<bool> SignOut(string token)
    {
        var client = _supabaseService.GetClient();
        await client.Auth.SignOut();
        return true;
    }
}