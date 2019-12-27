using System;
using System.Collections.Generic;
using System.Text;

namespace Wei.Repository.Test
{
    public class User : Entity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
    }
}
