using System;
using System.Collections.Generic;
using System.Text;

namespace Wei.Repository.Test
{
    public class TestTable1 : Entity<long>
    {
        public string TestMethodName { get; set; }
        public string TestResult { get; set; }
    }
}
