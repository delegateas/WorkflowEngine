using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Core;

namespace WorkflowEngine.DemoApp
{
  

    public class SendEmailAction : IActionImplementation
    {
        public const string SendEmailActionType = "SendEmailAction";
       

        public async ValueTask<object> ExecuteAsync(IWorkflow workflow, IAction action)
        {
            await Task.Delay(TimeSpan.FromMinutes(3));
          
            return null;
        }
    }

    public class EAVCreateAction : IActionImplementation
    {
        public const string EAVCreateActionType = "EAVCreateAction";


        public async ValueTask<object> ExecuteAsync(IWorkflow workflow, IAction action)
        {
            Console.WriteLine($"Hello world from 1");
            await Task.Delay(TimeSpan.FromMinutes(3));

            Console.WriteLine($"Hello world from 2");

            return null;
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

            services.AddTransient<IWorkflowExecutor, WorkflowExecutor>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IHangfireWorkflowExecutor, HangfireWorkflowExecutor>();
            services.AddTransient<IHangfireActionExecutor, HangfireWorkflowExecutor>();
            services.AddSingleton<IWorkflowRepository, DefaultWorkflowRepository>();
            services.AddAction<SendEmailAction>(SendEmailAction.SendEmailActionType);
            services.AddAction<EAVCreateAction>(EAVCreateAction.EAVCreateActionType);

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
                                Inputs =
                                {
                                    ["operation"] = "create",
                                    ["entity"] ="targetgroupresults"
                                }
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
                         (executor) => executor.TriggerAsync(new TriggerContext { Workflow = workflows.First() }));

                    await c.Response.WriteAsync("Background JOb:" + a);

                    // c.RequestServices.GetRequiredService


                });
            });
        }
    }
}
