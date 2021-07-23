using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SinoDbAPI.Extensions;
using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using SinoDbAPI.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Text.Json;

namespace SinoDbAPI.Services
{
    public interface IUsersService
    {
        Task<AuthenticationResponse> Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Register(string username, string password);
    }

    public class UsersService : IUsersService
    {
        private readonly IRedisClient _redisClient;
        private readonly IMongoCollection<User> _results;
        private readonly IAuthenticationSettings _authenticationSettings;

        public UsersService(ISinoDataBaseSettings settings, IAuthenticationSettings authenticationSettings, IMongoClient client, IRedisClientsManager redisClientsManager)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _results = database.GetCollection<User>(settings.UserCollectionName);
            _authenticationSettings = authenticationSettings;
            _redisClient = redisClientsManager.GetClient();
        }

        public async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            var queryResult = await _results.FindAsync(x => x.Username == username);
            var user = queryResult.FirstOrDefault();

            if (user == null || !BC.Verify(password, user.Password))
            {
                return null;
            }

            var hash = HashUser(user);
            var session = GetSession(hash);
            if (session is null)
            {
                var token = generateToken(user);
                var newSession = user.ToSession(token, TimeSpan.FromHours(1));
                SaveSessionToken(hash, newSession);

                return new AuthenticationResponse(user, token);
            }
            else if (session.ExpirationDate.CompareTo(DateTime.Now) < 0)
            {
                var token = generateToken(user);
                var newSession = user.ToSession(token, TimeSpan.FromHours(1));
                _redisClient.Replace(hash, newSession);
                _redisClient.Save();

                return new AuthenticationResponse(user, token);
            }

            return new AuthenticationResponse(user, session.SessionToken);
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

        private Jwt.Session GetSession(string hash)
        {
            return _redisClient.Get<Jwt.Session>(hash);
        }

        private void SaveSessionToken(string hash, Jwt.Session session)
        {
            _redisClient.Add(hash, session);
            _redisClient.Save();
        }

        private void UpdateSessionToken(string hash, Jwt.Session session)
        {
            _redisClient.Replace(hash, session);
            _redisClient.Save();
        }

        private string HashUser(User user)
        {
            using (MD5 md5 = MD5.Create())
            {
                var input = String.Format("{0}{1}", user.Id.ToString(), user.Username.ToString());
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
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