using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REstate.Client
{
    public interface IInstancesSession : IAuthenticatedSession
    {
        Task DeleteInstance(string instanceId);
        Task<State> FireTrigger(string instanceId, string triggerName, string payload = null);
        Task<ICollection<Trigger>> GetAvailableTriggers(string instanceId);
        Task<string> GetDiagram(string instanceId);
        Task<State> GetState(string instanceId);
        Task<string> Instantiate(string machineName);
        Task<bool> IsInState(string instanceId, string stateName);
    }
}