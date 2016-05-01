using Newtonsoft.Json;
using REstate.Client.Models;
using REstate.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REstate.Client
{
    public class ConfigurationSession 
        : AuthenticatedSession, IConfigurationSession
    {
        public ConfigurationSession(Uri authBaseAddress, Uri baseAddress, string apiKey, string token)
            : base(authBaseAddress, baseAddress, apiKey, token)
        {
            
        }


        public async Task<string> Instantiate(string machineName)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.PostAsync($"machines/{machineName}/instantiate",
                    new StringContent(string.Empty));

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            var instance = JsonConvert.DeserializeObject<MachineInstantiateResponse>(responseBody);

            return instance.MachineInstanceId;
        }

        public async Task<State> GetState(string instanceId)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"instances/{instanceId}/state");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            State state = JsonConvert.DeserializeObject<StateModel>(responseBody);

            return state;
        }

        public async Task<bool> IsInState(string instanceId, string stateName)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"instances/{instanceId}/isinstate/{stateName}");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            var isInStateResponse = JsonConvert.DeserializeObject<IsInStateResponse>(responseBody);

            return isInStateResponse.IsInState;
        }

        public async Task<ICollection<Trigger>> GetAvailableTriggers(string instanceId)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"instances/{instanceId}/triggers");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            var triggers = JsonConvert.DeserializeObject<TriggerModel[]>(responseBody);

            return triggers.Select(t => (Trigger)t).ToArray();
        }

        public async Task<State> FireTrigger(string instanceId, string triggerName, string payload = null)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var payloadBody = JsonConvert.SerializeObject(new PayloadContainer { Payload = payload });


                var response = await client.PostAsync($"instances/{instanceId}/fire/{triggerName}",
                    payload == null
                        ? new StringContent(string.Empty)
                        : new StringContent(payloadBody, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            State state = JsonConvert.DeserializeObject<StateModel>(responseBody);

            return state;
        }

        private class PayloadContainer
        {
            public string Payload { get; set; }
        }

        public async Task DeleteInstance(string machineInstanceId)
        {
            await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.DeleteAsync($"{machineInstanceId}");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return null;
            });
        }

        public async Task<string> GetDiagram(string instanceId)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"instances/{instanceId}/diagram");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            return responseBody;
        }

        public async Task<Machine> GetMachine(string machineName)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"machines/{machineName}");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            var configuration = JsonConvert.DeserializeObject<Machine>(responseBody);

            return configuration;
        }

        public async Task<Machine> DefineStateMachine(Machine configuration)
        {
            var payload = JsonConvert.SerializeObject(configuration);

            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.PostAsync("machines/",
                    new StringContent(payload, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            var configurationResponse = JsonConvert.DeserializeObject<Machine>(responseBody);

            return configurationResponse;
        }

        public async Task<string> GetMachineDiagram(string machineName)
        {
            var responseBody = await EnsureAuthenticatedRequest(async (client) =>
            {
                var response = await client.GetAsync($"machines/{machineName}/diagram");

                if (!response.IsSuccessStatusCode) throw GetException(response);

                return await response.Content.ReadAsStringAsync();
            });

            return responseBody;
        }
    }
}