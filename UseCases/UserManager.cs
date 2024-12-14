using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;

namespace UseCases
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var storedUsers = await _userRepository.GetAllAsync();
            var foundUser = storedUsers.FirstOrDefault(_ => _.Username == username);
            return foundUser;
        }

        public Task AddAsync(User user)
        {
            return _userRepository.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            return _userRepository.UpdateAsync(user);
        }

        public async Task SuspendAsync(int id)
        {
            var foundUser = await _userRepository.GetByIdAsync(id);
            if (foundUser != null)
            {
                foundUser.Status = Entities.Enums.EntityStatus.Suspended;
                await _userRepository.UpdateAsync(foundUser);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var foundUser = await _userRepository.GetByIdAsync(id);
            if (foundUser != null)
            {
                foundUser.Status = Entities.Enums.EntityStatus.Active;
                await _userRepository.UpdateAsync(foundUser);
            }
        }
    }
}
