using System;

namespace RHF.Shared
{
    public interface IOperationResult
    {
        bool Success { get; set; }

        void AddMessage(string message);
    }

    public class OperationResult : IOperationResult
    {
        public bool Success { get; set; }
        private IList<string> Messages { get; set; }
        public OperationResult()
        {
            Messages = new List<string>(); // create object instance
        }

        public void AddMessage(string message)
        {
            Messages.Add(message); // add message to internal repo
        }
    }
}