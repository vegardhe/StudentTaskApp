using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Uwp.App.DataSource
{
    /// <summary>
    /// Handles database interaction for users.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Users Instance { get; } = new Users();

        /// <summary>
        /// Gets or sets the session user.
        /// </summary>
        /// <value>
        /// The session user.
        /// </value>
        public User SessionUser { get; set; }

        // TODO: Better solution for this.
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Users"/> is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        public bool Changed { get; set; }

        /// <summary>
        /// Gets or sets the user list.
        /// </summary>
        /// <value>
        /// The user list.
        /// </value>
        public List<User> UserList { get; set; }

        /// <summary>
        /// The base URI
        /// </summary>
        private const string BaseUri = "http://localhost:52988/api/";

        /// <summary>
        /// The client
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// Prevents a default instance of the <see cref="Users"/> class from being created.
        /// </summary>
        private Users()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        /// <summary>
        /// Logs the on.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="WebException"></exception>
        /// <exception cref="InvalidDataException">User is invalid!</exception>
        public async Task<User> LogOn(User user)
        {
            var postBody = JsonConvert.SerializeObject(user);
            var response = await _client
                .PostAsync("users/login", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var sessionUser = JsonConvert.DeserializeObject<User>(responseBody);
            if (!response.IsSuccessStatusCode)
            {
                if(!NetworkInterface.GetIsNetworkAvailable())
                    throw new WebException();

                if(sessionUser is null || !sessionUser.IsValid)
                    throw new InvalidDataException("User is invalid!");
            }
            SessionUser = sessionUser;
            return sessionUser;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<bool> PostUser(User user)
        {
            var postBody = JsonConvert.SerializeObject(user);
            var response = await _client
                .PostAsync("users", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        public async Task<User[]> GetUsers()
        {
            var json = await _client.GetStringAsync("users").ConfigureAwait(false);
            var users = JsonConvert.DeserializeObject<User[]>(json);
            Instance.UserList = users.ToList();
            return users;
        }
    }
}
