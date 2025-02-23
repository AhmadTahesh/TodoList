using Supabase;
using TodoList.Interfaces;


namespace TodoList.Services;

public class SupabaseService : ISupabaseService
{
    private readonly Client _client;

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:Key"];
        
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        _client = new Client(url, key, options);
    }

    public Client GetClient() => _client;
}
