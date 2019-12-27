using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wei.Repository;

namespace WebApi.Data
{
    public class User : Entity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
    }
}
