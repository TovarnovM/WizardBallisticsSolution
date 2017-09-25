using ClusterExecutor;
using MPAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClustSlave {
    class Program {

        static void Main(string[] args) {
            var tstp = new TstParam();
            int port = 5555;
            string regServerAddress = "192.168.1.155";
            int regServerPort = 7777;
            var ss = Dns.GetHostEntry(regServerAddress).AddressList;
            using (Node node = new Node()) {
                //open the node in slave mode
                string input;
                do {
                    node.OpenDistributed(regServerAddress, regServerPort, port);
                    /* Since the node spawns new workers in separate threads, and as
                     * background threads, we have to prevent the main thread from
                     * terminating here. */
                    input = Console.ReadLine();
                } while (input != "exit");

            }
        }
    }
}
