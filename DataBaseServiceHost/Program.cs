using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var host = new ServiceHost(typeof(DataBaseService.DataBaseOperator)))
                {
                    host.Open();
                    Console.WriteLine("DataBaseService Host is open.");                 
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }

            
        }
    }
}
