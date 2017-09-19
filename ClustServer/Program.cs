using MPAPI.RegistrationServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClustServer {
    class Program {
        static void Main(string[] args) {
            using (var rs = new RegistrationServerBootstrap()) {
                rs.Open(7777);
            }
        }
    }
}
