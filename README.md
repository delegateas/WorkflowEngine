
# Workflow Engine on top of Hangfire

```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Bigs3cRet' -e 'MSSQL_PID=Express' -p 1433:1433 --name hangfiredemo -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu
docker exec -it hangfiredemo /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Bigs3cRet -Q "CREATE DATABASE hangfiredemo"
```

and run the demo app and go to /magic endpoint


Example workflow
```json
{

	"triggers":{
		"Trigger":{					
					"type":"EAVTrigger",
					"inputs":{
						"operation": ["create"],
						"entity":"targetgroupresults",
					}
				},
	},
	"actions":{
		
		"Create_Form_Submission":{
			"type":"EAVCreate",
			"runAfter":{},
			"inputs":{
				"entity": "formsubmissions"
			}
		},
		"Send_Email":{
			"type":"SendEmail",
			"runAfter":{
				"Create_Form_Submission": ["Succeded"]
			},
			"inputs":{
				"form": "xxx@xxx.dk",
				"to":"xxx@xxx.dk",
				"message":"Hello world from workflow"
			}
		}
	}
}

```

Actions
```
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
```

Service Registration
```
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
                                ["to"] ="xxx@xxx.dk",
                                ["from"] ="xxx@xxx.dk",
                                ["message"]="Hello world form workflow engine"
                            },
                            RunAfter= {
                                ["Create_Form_Submission"] = new []{ "Succeded" }
                    }
                    }
                }
        }
    });
```