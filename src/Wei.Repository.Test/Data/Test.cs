using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Wei.Repository.Test
{
    public class Test
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
