using BlazorTodo.Shared;

namespace BlazorTodo.Server.Services.DIexample
{
    public class UserRepository : IUserRepository
    {
        public async Task<List<User>> GetAll()
        {
            return new List<User>()
            {
                new User()
                {
                    Name = "James",
                    DateOfBirth = new DateTime(2007, 1, 3)
                },
                new User()
                {
                    Name = "Luther",
                    DateOfBirth = new DateTime(2001, 3, 19)
                },
                new User()
                {
                    Name = "Kelly",
                    DateOfBirth = new DateTime(2010, 9, 1)
                },
                new User()
                {
                    Name = "Benjamin",
                    DateOfBirth = new DateTime(2005, 8, 25)
                }
            };
        }
    }
}
