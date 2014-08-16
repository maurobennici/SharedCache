using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMock
{
    [Serializable]
    public class Customer
    {
        public string Name { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
