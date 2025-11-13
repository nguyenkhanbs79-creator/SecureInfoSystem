using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SecureInfoSystem.BLL.Helpers;
using SecureInfoSystem.Entities;

namespace SecureInfoSystem.DAL.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Register(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                user.PasswordHash = SecurityHelper.HashPassword(user.PasswordHash);
                user.PhoneEncrypted = SecurityHelper.EncryptString(user.PhoneEncrypted);
                user.AddressEncrypted = SecurityHelper.EncryptString(user.AddressEncrypted);

                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, PhoneEncrypted, AddressEncrypted) 
                                            VALUES (@Username, @PasswordHash, @FullName, @Email, @PhoneEncrypted, @AddressEncrypted)";

                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = user.Username });
                    command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 88) { Value = user.PasswordHash });
                    command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = (object)user.FullName ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = (object)user.Email ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@PhoneEncrypted", SqlDbType.NVarChar) { Value = (object)user.PhoneEncrypted ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@AddressEncrypted", SqlDbType.NVarChar) { Value = (object)user.AddressEncrypted ?? DBNull.Value });

                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while registering the user.", ex);
            }
        }

        public bool LoginCheck(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT PasswordHash FROM dbo.Users WHERE Username = @Username";
                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username });

                    connection.Open();
                    var result = command.ExecuteScalar() as string;
                    if (string.IsNullOrEmpty(result))
                    {
                        return false;
                    }

                    return SecurityHelper.VerifyPassword(password, result);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while verifying login credentials.", ex);
            }
        }

        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM dbo.Users WHERE Username = @Username";
                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username });

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapUser(reader);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving the user by username.", ex);
            }
        }

        public User GetById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM dbo.Users WHERE UserId = @UserId";
                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = id });

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapUser(reader);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving the user by identifier.", ex);
            }
        }

        public bool UpdateUserInfo(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                user.PhoneEncrypted = SecurityHelper.EncryptString(user.PhoneEncrypted);
                user.AddressEncrypted = SecurityHelper.EncryptString(user.AddressEncrypted);

                using (var connection = new SqlConnection(_connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE dbo.Users 
                                            SET FullName = @FullName, 
                                                Email = @Email, 
                                                PhoneEncrypted = @PhoneEncrypted, 
                                                AddressEncrypted = @AddressEncrypted, 
                                                UpdatedAt = SYSUTCDATETIME()
                                            WHERE UserId = @UserId";

                    command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = (object)user.FullName ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = (object)user.Email ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@PhoneEncrypted", SqlDbType.NVarChar) { Value = (object)user.PhoneEncrypted ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@AddressEncrypted", SqlDbType.NVarChar) { Value = (object)user.AddressEncrypted ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId });

                    connection.Open();
                    var affected = command.ExecuteNonQuery();
                    return affected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the user information.", ex);
            }
        }

        private User MapUser(IDataRecord record)
        {
            return new User
            {
                UserId = record.GetInt32(record.GetOrdinal("UserId")),
                Username = record.GetString(record.GetOrdinal("Username")),
                PasswordHash = record.GetString(record.GetOrdinal("PasswordHash")),
                FullName = record["FullName"] as string,
                Email = record["Email"] as string,
                PhoneEncrypted = record["PhoneEncrypted"] as string,
                AddressEncrypted = record["AddressEncrypted"] as string,
                CreatedAt = record.GetDateTime(record.GetOrdinal("CreatedAt")),
                UpdatedAt = record["UpdatedAt"] == DBNull.Value ? (DateTime?)null : record.GetDateTime(record.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
