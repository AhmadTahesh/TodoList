using Supabase.Gotrue;
using TodoList.Requests;

namespace TodoList.Interfaces;

public interface IAuthService
{
    Task<Session> SignUp(SignUpRequest request);
    Task<Session> SignIn(SignInRequest request);
    Task<bool> SignOut(string token);
}
