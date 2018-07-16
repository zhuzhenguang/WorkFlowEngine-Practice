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
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            var dbProvider = new MSSQLProvider(connectionString);

            IWorkflowBuilder builder = new WorkflowBuilder<XElement>(
                    dbProvider,
                    new XmlWorkflowParser(),
                    dbProvider)
                .WithDefaultCache();

            new WorkflowRuntime()
                .WithBuilder(builder)
                .WithPersistenceProvider(dbProvider)
                .WithBus(new NullBus())
                .EnableCodeActions()
                .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn();
        }
    }
}