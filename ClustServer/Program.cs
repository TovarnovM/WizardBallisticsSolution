using MPAPI.RegistrationServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClustServer {
    class Program {
        static void Main(string[] args) {
            string input = "n";
            do {
                try {
                    using (var rs = new RegistrationServerBootstrap()) {
                        rs.Open(7777);
                    }
                } catch (Exception) {
                    Console.WriteLine();
                    Console.Write(@"retry?[y/n] ");
                    input = Console.ReadLine();
                }
            } while (input.StartsWith("y"));

        }
    }
}
