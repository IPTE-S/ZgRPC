using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZgRPC;
using static ZgRPC.PersonaService;

namespace Server
{
    public class PersonaServiceImpl : PersonaServiceBase
    {
        public override Task<PersonaResponse> RegistrarPersona(PersonaRequest request, ServerCallContext context)
        {
            string mensaje = "Se insertó correctamente el usuario: " + request.Persona.Nombre + " - " + request.Persona.Email;

            PersonaResponse response = new PersonaResponse()
            {
                Resultado = mensaje
            };

            return Task.FromResult(response);

        }

        public override async Task RegistrarPersonasServidorMultiple(ServerMultiplePersonaRequest request, IServerStreamWriter<ServerMultiplePersonaResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("El servidor recibió el Request del cliente: " + request.ToString());

            string mensaje = "Se insertó correctamente el usuario: " + request.Persona.Nombre + " - " + request.Persona.Email;

            foreach (int i in Enumerable.Range(1,10))
            {
                ServerMultiplePersonaResponse response = new ServerMultiplePersonaResponse()
                {
                    Resultado = string.Format("El response {0} tiene contenido {1}", i, mensaje)
                };
                await responseStream.WriteAsync(response);
            }
        }

        public override async Task<ClientMultiplePersonaResponse> RegistrarPersonaClientMultiple(IAsyncStreamReader<ClientMultiplePersonaRequest> requestStream, ServerCallContext context)
        {
            string resultado = "";

            while (await requestStream.MoveNext())
            {
                resultado += string.Format("Request Mensaje en el Servisor {0} {1}", requestStream.Current.Persona.Email, Environment.NewLine);
            }

            var responseMessage = new ClientMultiplePersonaResponse()
            {
                Resultado = resultado
            };
            return responseMessage;
        }

        public override async Task RegistrarPersonaBidirecional(IAsyncStreamReader<BidereccionalPersonaRequest> requestStream, IServerStreamWriter<BidereccionalPersonaResponse> responseStream, ServerCallContext context)
        {

            while (await requestStream.MoveNext()) {
                var mensaje = string.Format("Comunicación Bidireccional: {0} {1}", requestStream.Current.Persona.Email, Environment.NewLine);
                Console.WriteLine(mensaje);
                var response = new BidereccionalPersonaResponse()
                {
                    Resultado = mensaje
                };
                await responseStream.WriteAsync(response);
            }

        }

    }
    
}
