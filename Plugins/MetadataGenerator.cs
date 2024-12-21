using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelPersonalized.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Plugins
{
    public class MetadataGenerator
    {
        private readonly Kernel _kernel;

        public MetadataGenerator(Kernel kernel)
        {
            _kernel = kernel;
        }

        //[KernelFunction("detail_fun")]
        //[Description("Fun detail with data")]
        //public string GetChatHistory()
        //{
        //    // how to get chathistory from chatCompletion?
        //    //return  JsonSerializer.Serialize(connector.getConversationHistory().GetHistory());
        //}


    }
}
