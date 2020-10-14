﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Prima.Services
{
    public class PasswordGenerator
    {
        private readonly HttpClient _http;

        public PasswordGenerator(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> Get(ulong uid)
        {
            using var req = new StringContent(uid.ToString());
            var res = await _http.PostAsync(new Uri("http://localhost:9000/"), req);
            return await res.Content.ReadAsStringAsync();
        }
    }
}