using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseMock;
using System.Diagnostics;

namespace ScharedCacheResearch
{
    class Program
    {
        static void Main(string[] args)
        {
            // first launch SharedCache...: SharedCache.WinService.exe /local

            Console.WriteLine("List of registered customers:");

            IEnumerable<Customer> registeredCustomers = null;

            Stopwatch stopwatchReadCache = Stopwatch.StartNew();
            // get data from SharedCache
            registeredCustomers = SharedCacheWrapper.Get<IEnumerable<Customer>>("all customers");
            stopwatchReadCache.Stop();

            Stopwatch stopwatchDatabase = new Stopwatch();
            Stopwatch stopwatchWriteCache = new Stopwatch();
            // cache contains no data
            if (registeredCustomers == null)
            {
                stopwatchDatabase.Start();
                // load data from mocked repository
                registeredCustomers = CustomersRepository.GetAllCustomers();
                stopwatchDatabase.Stop();
                stopwatchWriteCache.Start();
                // push loaded data to a cache
                SharedCacheWrapper.Add("all customers", registeredCustomers);
                stopwatchWriteCache.Stop();
            }
            else
            {
                Console.WriteLine("Cache HIT - data taken from cache.");
            }

            foreach (var customer in registeredCustomers)
            {
                Console.WriteLine("- {0} [{1}]", customer.Name, customer.RegisterDate);
            }
            Console.WriteLine();
            Console.WriteLine("Gathering data from mocked database: {0} [ms]", stopwatchDatabase.ElapsedMilliseconds);
            Console.WriteLine("Gathering data from cache: {0} [ms]", stopwatchReadCache.ElapsedMilliseconds);
            Console.WriteLine("Sending data to cache: {0} [ms]", stopwatchWriteCache.ElapsedMilliseconds);
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
