﻿using System;  
using System.IO;
using ZgRPC;

namespace Server
{
    class Program
    {
        const int Port = 50008;
        static void Main(string[] args)
        {
            Grpc.Core.Server server = null;
            try
            {
                server = new Grpc.Core.Server()
                {
                    Services = { PersonaService.BindService(new PersonaServiceImpl()) },
                    Ports = { new Grpc.Core.ServerPort("localhost", Port, Grpc.Core.ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("El servidor se está ejecutando en el puerto : " + Port);
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine("Errores en el Servidor" + e.Message);
            }
            finally {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }

        }
    }
}
