using BlazorTodo.Shared;

namespace BlazorTodo.Server.Services.DIexample
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
    }
}
