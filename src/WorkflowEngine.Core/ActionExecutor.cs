using ExpressionEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Core
{
    public interface IScopeContext
    {
        public string Scope { get; set; }
    }
    public class ScopeContext : IScopeContext
    {
        public string Scope { get; set; }
    }
    public class ActionExecutor : IActionExecutor
    {
        private readonly IOutputsRepository _outputsRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IScopeContext _scopeContext;
        private readonly IExpressionEngine _expressionEngine;
        private Dictionary<string, IActionImplementationMetadata> _implementations;

        public ActionExecutor(
            IEnumerable<IActionImplementationMetadata> implementations,
            IOutputsRepository outputsRepository,
            IServiceProvider serviceProvider,
            ILogger<ActionExecutor> logger,
            IScopeContext scopeContext,
            IExpressionEngine expressionEngine)
        {

            if (implementations.GroupBy(k => k.Type).Any(c => c.Count() > 1))
            {
                throw new ArgumentException("Double registration of " + String.Join(",", implementations.GroupBy(k => k.Type).Where(c => c.Count() > 1).Select(c => c.Key)));
            }
            _implementations = implementations?.ToDictionary(k => k.Type) ?? throw new ArgumentNullException(nameof(implementations));
            _outputsRepository = outputsRepository ?? throw new ArgumentNullException(nameof(outputsRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeContext = scopeContext;
            _expressionEngine = expressionEngine ?? throw new ArgumentNullException(nameof(expressionEngine));
        }
        public async ValueTask<IActionResult> ExecuteAsync(IRunContext context, IWorkflow workflow, IAction action)
        {
            try
            {

                if (action.ScopeMoveNext)
                {
                    await _outputsRepository.EndScope(context, workflow, action);
                }

                var actionMetadata = workflow.Manifest.Actions.FindAction(action.Key);
                _scopeContext.Scope = action.Key;
                action.Inputs = await _expressionEngine.ResolveInputs(actionMetadata, _logger);

                {
                    //if (workflow.Manifest.Actions.FindParentAction(action.Key) is ForLoopActionMetadata parent)
                    //{
                    //    await outputsRepository.AddArrayInput(context, workflow, action);
                    //}
                    //else
                    //if (actionMetadata is ForLoopActionMetadata)
                    //{
                    //    await outputsRepository.StartScope(context, workflow, action);
                    //}
                    //else
                    {
                        await _outputsRepository.AddInput(context, workflow, action);
                    }
                }

                var actionImplementation = _serviceProvider.GetRequiredService(_implementations[actionMetadata.Type].Implementation) as IActionImplementation;




                var result = new ActionResult
                {
                    Key = action.Key,
                    Status = "Succeded",
                    Result = await actionImplementation.ExecuteAsync(context, workflow, action)
                };



                await _outputsRepository.AddAsync(context, workflow, action, result);


                return result;


            }
            catch (Exception ex)
            {
                var result = new ActionResult { Key = action.Key, Status = "Failed", FailedReason = ex.ToString(), ReThrow = (ex is InvalidOperationException) };
                try
                {
                    await _outputsRepository.AddAsync(context, workflow, action, result);
                }
                catch (Exception exx)
                {

                }
                return result;
            }
        }
    }



}
