using ExpressionEngine;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Actions;
using WorkflowEngine.Core.Expressions;

namespace WorkflowEngine.DemoApp
{
  

    public class SendEmailAction : IActionImplementation
    {
        public const string SendEmailActionType = "SendEmailAction";
       

        public async ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            await Task.Delay(TimeSpan.FromMinutes(3));
          
            return null;
        }
    }

    public class EAVCreateAction : IActionImplementation
    {
        public const string EAVCreateActionType = "EAVCreateAction";


        public async ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            Console.WriteLine($"Hello world from 1");
            await Task.Delay(TimeSpan.FromMinutes(3));

            Console.WriteLine($"Hello world from 2");

            return null;
        }
    }
    
    public class RetrieveRecordAction : IActionImplementation
    {
        private readonly IExpressionEngine expressionEngine;

        public RetrieveRecordAction(IExpressionEngine expressionEngine)
        {
            this.expressionEngine=expressionEngine;
        }
        public ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            //  var next = workflow.Manifest.Actions.FindAction(action.Key);
            var inputs = action.Inputs;



            return new ValueTask<object>(new
            {
                id = Guid.Parse(inputs["recordId"]?.ToString()),
                targetgroupid = Guid.NewGuid()
            });
        }
    }
    public class RetrieveTargetGroupTargetsAction : IActionImplementation
    {
        public ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            return new ValueTask<object>(new
            {
                value = new[] { new { targetid=Guid.NewGuid()}, new { targetid = Guid.NewGuid() } }
            });
        }
    }
    public class FindFormSubmissionForAccountAction : IActionImplementation
    {
        public ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            var next = workflow.Manifest.Actions.FindAction(action.Key);

            return new ValueTask<object>(new { formsubmissionid = Guid.NewGuid() });
        }
    }
    public class CreateESDHCaseFromFormSubmissionAction : IActionImplementation
    {
        public ValueTask<object> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            var next = workflow.Manifest.Actions.FindAction(action.Key);

            return new ValueTask<object>(new { id = Guid.NewGuid() });
        }
    }
   


    public class Startup
    {
      

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire((sp , configuration) => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    sp.GetRequiredService<IConfiguration>().GetValue<string>("ConnectionString"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            services.AddHangfireServer();
            services.AddRazorPages();
            services.AddExpressionEngine();


            services.AddTransient<IWorkflowExecutor, WorkflowExecutor>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IHangfireWorkflowExecutor, HangfireWorkflowExecutor>();
            services.AddTransient<IHangfireActionExecutor, HangfireWorkflowExecutor>();
            services.AddSingleton<IWorkflowRepository, DefaultWorkflowRepository>();
            services.AddAction<SendEmailAction>(SendEmailAction.SendEmailActionType);
            services.AddAction<EAVCreateAction>(EAVCreateAction.EAVCreateActionType);
            services.AddAction<RetrieveRecordAction>();
            services.AddAction<RetrieveTargetGroupTargetsAction>();
            services.AddAction<FindFormSubmissionForAccountAction>();
            services.AddAction<CreateESDHCaseFromFormSubmissionAction>();
            services.AddAction<ForeachAction>("Foreach");
            services.AddSingleton<IOutputsRepository, DefaultOutputsRepository>();
            services.AddFunctions();
            services.AddScoped<IRunContextAccessor, RunContextFactory>();
            services.AddScoped<IArrayContext, ArrayContext>();
            services.AddScoped<IScopeContext, ScopeContext>();
            services.AddSingleton<IWorkflow>(new Workflow
            {
                Id = Guid.Empty,
                Version = "1.0",
                Manifest = new WorkflowManifest
                {
                    Triggers =
                        {
                            ["Trigger"] = new TriggerMetadata
                            {
                                Type = "EAVTrigger",
                                //Inputs =
                                //{
                                //    ["operation"] = "create",
                                //    ["entity"] ="targetgroupresults"
                                //}
                            }
                        },
                    Actions =
                        {
                            ["Create_Form_Submission"] = new ActionMetadata{
                                Type = "EAVCreateAction",
                                Inputs ={
                                    ["entity"] ="formsubmissions"
                                }
                            },
                            ["Send_Email"] = new ActionMetadata{
                                  Type = "SendEmailAction",
                                    Inputs ={
                                        ["to"] ="tst@delgate.dk",
                                        ["from"] ="pks@delegate.dk",
                                        ["message"]="Hello world form workflow engine"
                                    },
                                    RunAfter= {
                                        ["Create_Form_Submission"] = new []{ "Succeded" }
                            }
                            }
                        }
                }
            });

            services.AddSingleton<IWorkflow>(new Workflow
            {
                Id = Guid.Parse("dd087e82-c61a-4ce2-b961-b976baa4bf17"),
                Version = "1.0",
                Manifest = new WorkflowManifest
                {
                    Triggers =
                    {
                        ["Trigger"] = new TriggerMetadata
                        {
                            Type = "Manual",
                            
                        }
                    },
                    Actions =
                    {
                        [nameof(RetrieveRecordAction)] = new ActionMetadata
                        {
                            Type = nameof(RetrieveRecordAction),
                            Inputs =
                            {
                                ["entityName"] = "forms",
                                ["recordId"] = "@triggerBody()?['recordId']"
                            }
                        },
                        [nameof(RetrieveTargetGroupTargetsAction)] = new ActionMetadata
                        {
                            Type = nameof(RetrieveTargetGroupTargetsAction),
                            RunAfter = new WorkflowRunAfterActions
                            {
                                [nameof(RetrieveRecordAction)] = new []{"Succeded"}
                            },
                            Inputs =
                            {
                                ["targetgroupid"] = $"@outputs('{nameof(RetrieveRecordAction)}')?['body/targetgroupid']"
                            }
                        },
                        ["Loop_over_targetgroup"] = new ForLoopActionMetadata
                        {
                            RunAfter = new WorkflowRunAfterActions
                            {
                                [nameof(RetrieveTargetGroupTargetsAction)] = new []{"Succeded"}
                            },
                            ForEach =  $"@outputs('{nameof(RetrieveTargetGroupTargetsAction)}')?['body/value']",
                            Type = "Foreach",
                            Inputs =
                            {

                            },
                            Actions =
                            {
                                [nameof(FindFormSubmissionForAccountAction)] = new ActionMetadata
                                {
                                    Type = nameof(FindFormSubmissionForAccountAction),
                                    Inputs =
                                    {
                                        ["accountid"] = "@items('Loop_over_targetgroup')?['targetid']",
                                        ["formid"] = "@triggerBody()?['recordId']",
                                    }
                                },
                                [nameof(CreateESDHCaseFromFormSubmissionAction)] = new ActionMetadata
                                {
                                    RunAfter = new WorkflowRunAfterActions
                                    {
                                        [nameof(FindFormSubmissionForAccountAction)] = new []{"Succeded"}
                                    },
                                    Type = nameof(CreateESDHCaseFromFormSubmissionAction),
                                    Inputs =
                                    {
                                        ["formsubmissionid"] = $"@outputs('{nameof(FindFormSubmissionForAccountAction)}')?['body/formsubmissionid']",
                                    }
                                }

                            }
                        }

                    }
                }
        });

            //   services.AddScoped()
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {

            app.UseHangfireDashboard();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async c =>
                 {
                     await c.Response.WriteAsync("Hello world");


                 });

                endpoints.MapGet("/magic", async c =>
                {
                    var workflows = await c.RequestServices.GetRequiredService<IWorkflowRepository>().GetAllWorkflows();

                    var a = Hangfire.BackgroundJob.Enqueue<IHangfireWorkflowExecutor>(
                         (executor) => executor.TriggerAsync(new TriggerContext { 
                             Workflow = workflows.First(),
                             Trigger = new Trigger { ScheduledTime = DateTimeOffset.UtcNow, Key = workflows.First().Manifest.Triggers.First().Key,
                                 Type =workflows.First().Manifest.Triggers.First().Value.Type },  RunId = Guid.NewGuid() }));

                    await c.Response.WriteAsync("Background JOb:" + a);

                    // c.RequestServices.GetRequiredService


                });

                endpoints.MapGet("/runs", async c =>
                {
                    await c.Response.WriteAsync(JToken.FromObject(c.RequestServices.GetRequiredService<IOutputsRepository>()).ToString());
                 });
                     endpoints.MapGet("/magic/{id}", async c =>
                {
                    var workflows = await c.RequestServices.GetRequiredService<IWorkflowRepository>().GetAllWorkflows();
                    var inputs = new Dictionary<string, object>
                    {
                        ["recordId"] = Guid.NewGuid(),
                        ["entityName"] ="Forms",
                    };
                    var a = Hangfire.BackgroundJob.Enqueue<IHangfireWorkflowExecutor>(
                         (executor) => executor.TriggerAsync(new TriggerContext { 
                             RunId = Guid.NewGuid() ,
                             Trigger = new Trigger { 
                                 Inputs = inputs,
                                 ScheduledTime = DateTimeOffset.UtcNow, Type=workflows.First(w => w.Id.ToString() == c.GetRouteValue("id") as string).Manifest.Triggers.FirstOrDefault().Value.Type,
                                 Key = workflows.First(w => w.Id.ToString() == c.GetRouteValue("id") as string).Manifest.Triggers.FirstOrDefault().Key
                             },
                             Workflow = workflows.First(w=>w.Id.ToString() == c.GetRouteValue("id") as string)
                         }));

                    await c.Response.WriteAsync("Background JOb:" + a);

                    // c.RequestServices.GetRequiredService


                });
            });
        }
    }
}
