﻿using Newtonsoft.Json;
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
    public class Users
    {
        public static Users Instance { get; } = new Users();

        public User SessionUser { get; set; }

        // TODO: Better solution for this.
        public bool Changed { get; set; }

        public List<User> UserList { get; set; }

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Users()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

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

        public async Task<bool> CreateUser(User user)
        {
            var postBody = JsonConvert.SerializeObject(user);
            var response = await _client
                .PostAsync("users", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<User[]> GetUsers()
        {
            var json = await _client.GetStringAsync("users").ConfigureAwait(false);
            var users = JsonConvert.DeserializeObject<User[]>(json);
            Instance.UserList = users.ToList();
            return users;
        }
    }
}
