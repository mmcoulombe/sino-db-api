using BC = BCrypt.Net.BCrypt;

using Core.Arango;

using Microsoft.IdentityModel.Tokens;

using SinoDbAPI.Extensions;
using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using SinoDbAPI.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SinoDbAPI.Services
{
    public interface IUsersService
    {
        Task<AuthenticationResponse> Authenticate(string username, string password);
        Task<IEnumerable<UserDetailResponse>> GetAll();
        Task<UserDetailResponse> GetById(string id);
        Task<string> Register(string username, string password);
    }

    public class UsersService : IUsersService
    {
        private readonly IArangoContext _arangoContext;
        private readonly IAuthenticationSettings _authenticationSettings;

        private readonly string _databaseName;
        private readonly string _userCollectionName;
        private readonly string _sessionCollectionName;

        public UsersService(ISinoDataBaseSettings settings, IAuthenticationSettings authenticationSettings, IArangoContext arangoContext)
        {
            _databaseName = settings.DatabaseName;
            _arangoContext = arangoContext;
            _userCollectionName = settings.UserCollectionName;
            _sessionCollectionName = "session_cache";
            _authenticationSettings = authenticationSettings;
        }

        public async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            var query = string.Format("FOR user IN {0} FILTER user.Username == '{1}' RETURN user", _userCollectionName, username);
            var queryResult = await _arangoContext.Query.ExecuteAsync<User>(_databaseName, query, null);
            var user = queryResult.FirstOrDefault();

            if (user == null || !BC.Verify(password, user.Password))
            {
                return null;
            }

            var hash = HashUser(user);
            var session = await GetSession(hash);
            if (session is null)
            {
                var token = generateToken(user);
                var newSession = user.ToSession(token, TimeSpan.FromHours(1));
                await SaveSessionToken(hash, newSession);

                return new AuthenticationResponse(user, token);
            }
            else if (session.ExpirationDate.CompareTo(DateTime.Now) < 0)
            {
                var token = generateToken(user);
                var newSession = user.ToSession(token, TimeSpan.FromHours(1));
                await UpdateSessionToken(hash, newSession);

                return new AuthenticationResponse(user, token);
            }

            return new AuthenticationResponse(user, session.SessionToken);
        }

        public async Task<string> Register(string username, string password)
        {
            var user = User.FromUsernamePassword(username, BC.HashPassword(password));
            var result = await _arangoContext.Document.CreateAsync(_databaseName, _userCollectionName, user);
            
            return result.Id;
        }

        public async Task<IEnumerable<UserDetailResponse>> GetAll()
        {
            var query = string.Format("FOR user IN {0} RETURN user", _userCollectionName );
            var queryResult = await _arangoContext.Query.ExecuteAsync<User>(_databaseName, query, null);
            var users = queryResult.ToList().Select(UserDetailResponse.FromUser);

            return users;
        }

        public async Task<UserDetailResponse> GetById(string id)
        {
            var query = string.Format("RETURN DOCUMENT('{0}/{1}')", _userCollectionName, id);
            var result = await _arangoContext.Query.ExecuteAsync<User>(_databaseName, query, null);

            return result.Select(UserDetailResponse.FromUser).FirstOrDefault();
        }

        private async Task<Jwt.Session> GetSession(string hash)
        {
            var query = string.Format("RETURN DOCUMENT('{0}/{1}').value", _sessionCollectionName, hash);
            var result = await _arangoContext.Query.ExecuteAsync<Jwt.Session>(_databaseName, query, null);

            return result.FirstOrDefault();
        }

        private Task SaveSessionToken(string hash, Jwt.Session session)
        {
            var jsonSession = JsonConvert.SerializeObject(session);
            var query = string.Format("INSERT {{ _key: '{0}', value: {1} }} INTO session_cache", hash, jsonSession);

            return _arangoContext.Query.ExecuteAsync<Jwt.Session>(_databaseName, query, null);
        }

        private Task UpdateSessionToken(string hash, Jwt.Session session)
        {
            var jsonSession = JsonConvert.SerializeObject(session);
            var query = string.Format("REPLACE '{0}' WITH {{ value: {1} }} IN session_cache", hash, jsonSession);

            return _arangoContext.Query.ExecuteAsync<Jwt.Session>(_databaseName, query, null);
        }

        private string HashUser(User user)
        {
            using (MD5 md5 = MD5.Create())
            {
                var input = string.Format("{0}{1}", user.Id.ToString(), user.Username.ToString());
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