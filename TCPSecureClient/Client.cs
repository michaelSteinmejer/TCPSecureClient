using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace TCPSecureClient
{
    class Client
    {
        public static string serverCertificateFile { get; set; } = "c:/certificates/ServerSSL.cer";
        // Her laver et nyt objekt af certifikatet med 2 argumenter Filstien og kodeord
         static X509Certificate serverCertificate = new X509Certificate(serverCertificateFile, "Password123");
        X509CertificateCollection cc = new X509CertificateCollection();
        static void Main(string[] args)
        {
           
            TcpClient clientSocket = new TcpClient("127.0.0.1", 7000);
            Console.WriteLine("Client Ready");

            bool leaveInnerStreamOpen = false;
            

            string certificateServerName = "FakeServerName";

            ServicePointManager.ServerCertificateValidationCallback +=
                new RemoteCertificateValidationCallback(ValidateServerCertificate);

            NetworkStream UnsecureStream = clientSocket.GetStream();

            SslStream sslStream = new SslStream(UnsecureStream, leaveInnerStreamOpen
                ,  ValidateServerCertificate
              ,  SelectLocalCertificate
                );


            sslStream.AuthenticateAsClient(certificateServerName);
           
           

            StreamReader sr = new StreamReader(sslStream);
            StreamWriter sw = new StreamWriter(sslStream);
            sw.AutoFlush = true;

            while (true)
            {
                
                string message = Console.ReadLine();
                sw.WriteLine(message);

                if (message.ToLower() == "stop")
                {
                    break;
                }

                string answer = sr.ReadLine();
                Console.WriteLine(answer);
                    
            }


            Console.WriteLine("Session has been stopped");
            Console.ReadKey();
            UnsecureStream.Close();
            sr.Close();
            sw.Close();
        }

        #region ValideringsMetode
        public static bool ValidateServerCertificate(object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;

        }
        public static X509Certificate SelectLocalCertificate(
          object sender,
          string targetHost,
          X509CertificateCollection localCertificates,
          X509Certificate remoteCertificate,
          string[] acceptableIssuers)
        {
            
                return serverCertificate;

        } 
        #endregion
    }
}
