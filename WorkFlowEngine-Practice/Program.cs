using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Bus;
using OptimaJet.Workflow.Core.Parser;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;

namespace WorkFlowEngine_Practice
{
    class Program
    {
        static void Main()
        {
            WorkflowRuntime runTime = InitRunTime();


            Guid processId = Guid.NewGuid();
            string identityId = Guid.NewGuid().ToString();

            runTime.CreateInstance(new CreateInstanceParams("scheme1", processId)
            {
                IdentityId = identityId
            });
            IEnumerable<WorkflowCommand> availableCommands = runTime.GetAvailableCommands(processId, identityId);

            foreach (WorkflowCommand availableCommand in availableCommands)
            {
                Console.WriteLine(availableCommand);
            }

            Console.ReadLine();
        }

        static WorkflowRuntime InitRunTime()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            var dbProvider = new MSSQLProvider(connectionString);

            IWorkflowBuilder builder = new WorkflowBuilder<XElement>(
                    dbProvider,
                    new XmlWorkflowParser(),
                    dbProvider)
                .WithDefaultCache();

            WorkflowRuntime runtime = new WorkflowRuntime()
                .WithBuilder(builder)
                .WithPersistenceProvider(dbProvider)
                .WithBus(new NullBus())
                .EnableCodeActions()
                .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn();

            runtime.ProcessActivityChanged += (sender, args) => { };
            runtime.ProcessStatusChanged += (sender, args) => { };

            runtime.Start();

            return runtime;
        }
    }
}