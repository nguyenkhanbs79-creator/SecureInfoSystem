using System;
using SecureInfoSystem.Entities.Entities;
using SecureInfoSystem.DAL.Repositories;
using SecureInfoSystem.BLL.Helpers;

namespace SecureInfoSystem.BLL.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is required.", nameof(connectionString));
            }

            _userRepository = new UserRepository(connectionString);
        }

        public bool Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            return _userRepository.LoginCheck(username, password);
        }

        public User LoginAndGetUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            bool isValid = _userRepository.LoginCheck(username, password);
            if (!isValid)
            {
                return null;
            }

            return _userRepository.GetByUsername(username);
        }

        public bool Register(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException("Username is required.", nameof(user.Username));
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Password is required.", nameof(user.PasswordHash));
            }

            return _userRepository.Register(user);
        }
    }
}
