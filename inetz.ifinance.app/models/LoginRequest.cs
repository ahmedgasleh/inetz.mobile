﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.models
{
    public class LoginRequest
    {
        public string PhoneNumber { get; set; } = "";
        public string Password { get; set; } = "";
        public string DeviceId { get; set; } = "";
    }
}
