using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZgRPC;

namespace Client
{
    class Program
    {

        const String serverPoint = "127.0.0.1:50008";
        static async Task Main(string[] args)
        {
            Grpc.Core.Channel canal = new Grpc.Core.Channel(serverPoint, Grpc.Core.ChannelCredentials.Insecure);

            await canal.ConnectAsync().ContinueWith((task) =>
           {

               if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
               {
                   Console.WriteLine("El cliente se conectó al servidor Grpc correctamente");

               }
           });



            //var persona = new Persona()
            //{
            //    Nombre = "Victor",
            //    Apellido = "Cruz",
            //    Email = "vcruz@ipte.com.mx"
            //};


            var client = new PersonaService.PersonaServiceClient(canal);

            Persona[] peronaCollection =
            {
                new Persona() { Email = "grodríguez@ipte.com.mx"},
                new Persona() { Email = "vcruz.com.mx"},
                new Persona() { Email = "gcastillo.com.mx"},
                new Persona() { Email = "dsanchez.com.mx"},
            };

            var stream = client.RegistrarPersonaBidirecional();

            foreach(var persona in peronaCollection) {
                Console.WriteLine("Enciando al servidor: " + persona.Email);
                var request = new BidereccionalPersonaRequest()
                {
                    Persona = persona
                };
                await stream.RequestStream.WriteAsync(request);
            
            }

            await stream.RequestStream.CompleteAsync();

            var responseCollection = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("El cliente está recibiendo del servidor {0} {1}", stream.ResponseStream.Current.Resultado, Environment.NewLine);
                }
            });

            await responseCollection;



           


            //var request = new ClientMultiplePersonaRequest()
            //{
            //    Persona = persona
            //};

            //var stream = client.RegistrarPersonaClientMultiple();

            //foreach (int i in Enumerable.Range(1, 10))
            //{
            //    await stream.RequestStream.WriteAsync(request);
            //}

            //await stream.RequestStream.CompleteAsync();

            //var response = await stream.ResponseAsync;

            //Console.WriteLine(response.Resultado);



            //var request = new PersonaRequest()
            //{
            //    Persona =  persona
            //};

            // var request = new ServerMultiplePersonaRequest()
            // {
            //     Persona = persona
            // };

            // var client = new PersonaService.PersonaServiceClient(canal);
            // //var response = client.RegistrarPersona(request);
            // var response = client.RegistrarPersonasServidorMultiple(request);

            //while (await response.ResponseStream.MoveNext())
            // {
            //     Console.WriteLine(response.ResponseStream.Current.Resultado);
            //     await Task.Delay(250);
            // }




            canal.ShutdownAsync().Wait();
            Console.ReadKey();
        }
        
    }
}
