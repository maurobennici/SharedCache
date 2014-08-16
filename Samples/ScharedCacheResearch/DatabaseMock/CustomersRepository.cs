using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DatabaseMock
{
    public static class CustomersRepository
    {
        public static IEnumerable<Customer> GetAllCustomers()
        {
            Thread.Sleep(5000);
            return
                new List<Customer> {
                    new Customer { Name = "Customer 1", RegisterDate = DateTime.Now.AddMonths(-4) },
                    new Customer { Name = "Customer 2", RegisterDate = DateTime.Now.AddDays(-3) },
                };
        }
    }
}
