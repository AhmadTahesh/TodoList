using Supabase;

namespace TodoList.Interfaces;

public interface ISupabaseService
{
    Client GetClient();
}
