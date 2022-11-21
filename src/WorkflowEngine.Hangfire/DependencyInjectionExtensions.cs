using Microsoft.Extensions.DependencyInjection;
using WorkflowEngine;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Actions;
using WorkflowEngine.Core.Expressions;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddWorkflowEngine<TOutputsRepository>(this IServiceCollection services)
            where TOutputsRepository: class, IOutputsRepository
        {
            services.AddTransient<IWorkflowExecutor, WorkflowExecutor>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IHangfireWorkflowExecutor, HangfireWorkflowExecutor>();
            services.AddTransient<IHangfireActionExecutor, HangfireWorkflowExecutor>();
                       services.AddTransient(typeof(ScheduledWorkflowTrigger<>));

            services.AddScoped<IArrayContext, ArrayContext>();
            services.AddScoped<IScopeContext, ScopeContext>();
            services.AddScoped<IRunContextAccessor, RunContextFactory>();
            services.AddAction<ForeachAction>("Foreach");

            services.AddFunctions();
            services.AddScoped<IOutputsRepository, TOutputsRepository>();
            services.AddSingleton<IWorkflowRepository, DefaultWorkflowRepository>();

            services.AddHostedService<WorkflowStarterBackgroundJob>();

            services.AddTransient<IWorkflowAccessor, DefaultWorkflowAccessor>();
            services.AddTransient<IHangfireActionExecutorResultInspector, DefaultHangfireActionExecutorResultInspector>();
            return services;
        }

        
    }
}
