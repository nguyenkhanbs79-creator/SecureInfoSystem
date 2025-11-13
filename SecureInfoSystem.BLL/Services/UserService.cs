using System;
using SecureInfoSystem.Entities.Entities;
using SecureInfoSystem.DAL.Repositories;
using SecureInfoSystem.BLL.Helpers;

namespace SecureInfoSystem.BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is required.", nameof(connectionString));
            }

            _userRepository = new UserRepository(connectionString);
        }

        public User GetById(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            }

            return _userRepository.GetById(userId);
        }

        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            return _userRepository.GetByUsername(username);
        }

        public User GetDecryptedUserInfo(int userId)
        {
            var user = GetById(userId);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(user.PhoneEncrypted))
            {
                user.PhoneEncrypted = SecurityHelper.DecryptString(user.PhoneEncrypted);
            }

            if (!string.IsNullOrEmpty(user.AddressEncrypted))
            {
                user.AddressEncrypted = SecurityHelper.DecryptString(user.AddressEncrypted);
            }

            return user;
        }

        public bool UpdateUserInfo(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.UserId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero.", nameof(user.UserId));
            }

            return _userRepository.UpdateUserInfo(user);
        }
    }
}
