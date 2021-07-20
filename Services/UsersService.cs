using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using SinoDbAPI.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SinoDbAPI.Services
{
    public interface IUsersService
    {
        AuthenticationResponse Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Register(string username, string password);
    }

    public class UsersService : IUsersService
    {
        private readonly IMongoCollection<User> _results;
        private readonly IAuthenticationSettings _authenticationSettings;

        public UsersService(ISinoDataBaseSettings settings, IAuthenticationSettings authenticationSettings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _results = database.GetCollection<User>(settings.UserCollectionName);
            _authenticationSettings = authenticationSettings;
        }

        public AuthenticationResponse Authenticate(string username, string password)
        {
            // todo: use bcrypt
            var user = _results.Find(x => x.Username == username).FirstOrDefault();
            if (user == null || !BC.Verify(password, user.Password))
            {
                return null;
            }

            var token = generateToken(user);
            return new AuthenticationResponse(user, token);
        }

        public User Register(string username, string password)
        {
            var user = User.FromUsernamePassword(username, BC.HashPassword(password));
            _results.InsertOne(user);
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _results.Find(x => true).ToList();
        }

        public User GetById(string id)
        {
            return _results.Find(x => x.Id == id).FirstOrDefault();
        }

        private string generateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authenticationSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}