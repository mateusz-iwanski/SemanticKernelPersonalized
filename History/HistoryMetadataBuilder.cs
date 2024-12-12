using SemanticKernelPersonalized.AgentsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.History
{
    public class HistoryMetadataBuilder
    {
        public static IReadOnlyDictionary<string, object?> MessageContent(
            ChatDialogBase dialog,
            string author,
            Dictionary<string, object>? additionalMetaData = null,
            string? source = null,
            string? name = null
            )
        {
            var metadata = new Dictionary<string, object?>
            {
                { "uuid", Guid.NewGuid().ToString() },
                { "conversation_uuid", dialog.GetUuid().ToString() },
                { "modelId", dialog.GetModelId() },
                { "createAt", DateTime.Now.ToString() },
                { "source", source },
                { "name", name },
                { "author", author }
            };

            if (additionalMetaData != null) metadata.Concat(additionalMetaData).ToDictionary(x => x.Key, x => x.Value);

            return metadata;
        }

        public static IReadOnlyDictionary<string, object?> ItemContent(
            ChatDialogBase dialog,
            string author = "Unknown"
            )
        {
            var metadata = new Dictionary<string, object?>
            {
                { "uuid", Guid.NewGuid().ToString() },
                { "conversation_uuid", dialog.GetUuid().ToString() },
            };

            return metadata;
        }
    }
}
