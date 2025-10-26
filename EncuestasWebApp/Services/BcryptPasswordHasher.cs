﻿
namespace EncuestasWebApp.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }

    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) 
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);
        public bool Verify(string password, string passwordHash) 
            => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
