using Hangfire.Storage;
using System.Collections.Generic;

namespace WorkflowEngine
{
    public static class HangfireExtensions
    {


        public static void SetJobExternalKey(this IStorageConnection connection, string externalId, string jobId)
        {
            // This method can be implemented in 1.1.0
            connection.SetRangeInHash($"x-backgroundjob-keys:{externalId}", new[] { new KeyValuePair<string, string>("JobId", jobId) });
        }

        public static string GetJobIdByKey(this IStorageConnection connection, string externalId)
        {
            // This method can be implemented in 1.1.0
            var entries = connection.GetAllEntriesFromHash($"x-backgroundjob-keys:{externalId}");
            if (entries == null || !entries.ContainsKey("JobId"))
                return null;

            return entries["JobId"];
        }
    }
}
