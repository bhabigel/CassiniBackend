using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.Config
{
    /// <summary>
    /// Beállítások a json web token számára
    /// Egy felhasználó ellenőrzéséhez szükség van a token kibocsájtója, felhasználója és aláírója ellenőrzésére
    /// </summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}