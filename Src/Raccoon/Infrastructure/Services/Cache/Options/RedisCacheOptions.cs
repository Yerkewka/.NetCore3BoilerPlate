﻿namespace Infrastructure.Services.Cache.Options
{
    public class RedisCacheOptions
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
    }
}
