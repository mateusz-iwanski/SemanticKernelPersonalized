using Microsoft.SemanticKernel;
using SemanticKernelPersonalized.Agents.KernelVersion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Plugins.General
{
    public class PluginExplorerPlugin
    {
        private readonly Kernel _kernel;

        public PluginExplorerPlugin(Kernel kernel)
        {
            _kernel = kernel;
        }

        [KernelFunction("list_plugins")]
        [Description("Lists all registered plugins and their functions with descriptions.")]
        public string ListPlugins()
        {
            var sb = new StringBuilder();
            var plugins = _kernel.Plugins;

            sb.AppendLine("# Registered Plugins");
            sb.AppendLine();

            foreach (var plugin in plugins)
            {
                sb.AppendLine($"## Plugin: {plugin.Name}");

                foreach (var function in plugin.GetFunctionsMetadata())
                {
                    sb.AppendLine($"### Function: {function.Name}");

                    if (!string.IsNullOrEmpty(function.Description))
                    {
                        sb.AppendLine($"*{function.Description}*");
                    }

                    if (function.Parameters.Any())
                    {
                        sb.AppendLine("#### Parameters:");
                        foreach (var param in function.Parameters)
                        {
                            sb.AppendLine($"- **{param.Name}**: {param.Description}");
                            if (param.DefaultValue != null)
                            {
                                sb.AppendLine($"  - Default value: `{param.DefaultValue}`");
                            }
                        }
                    }
                    sb.AppendLine();
                }
                sb.AppendLine("---");
            }

            return sb.ToString();
        }

        [KernelFunction("search_plugins")]
        [Description("Searches for plugins and functions matching the given search term.")]
        public string SearchPlugins(
            [Description("Search term to filter plugins and functions")] string searchTerm)
        {
            var sb = new StringBuilder();
            var plugins = _kernel.Plugins;
            searchTerm = searchTerm.ToLower();

            sb.AppendLine($"# Search results for: {searchTerm}");
            sb.AppendLine();

            foreach (var plugin in plugins)
            {
                var matchingFunctions = plugin.GetFunctionsMetadata()
                    .Where(f => f.Name.ToLower().Contains(searchTerm) ||
                               f.Description.ToLower().Contains(searchTerm) ||
                               f.Parameters.Any(p => p.Name.ToLower().Contains(searchTerm) ||
                                                   p.Description.ToLower().Contains(searchTerm)));

                if (plugin.Name.ToLower().Contains(searchTerm) || matchingFunctions.Any())
                {
                    sb.AppendLine($"## Plugin: {plugin.Name}");

                    foreach (var function in matchingFunctions)
                    {
                        sb.AppendLine($"### Function: {function.Name}");
                        if (!string.IsNullOrEmpty(function.Description))
                        {
                            sb.AppendLine($"*{function.Description}*");
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine("---");
                }
            }

            if (sb.Length <= 0)
            {
                return "No matching plugins or functions found.";
            }

            return sb.ToString();
        }

        [KernelFunction("get_plugin_details")]
        [Description("Gets detailed information about a specific plugin.")]
        public string GetPluginDetails(
            [Description("Name of the plugin to get details for")] string pluginName)
        {
            var plugin = _kernel.Plugins.FirstOrDefault(p =>
                p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));

            if (plugin == null)
            {
                return $"Plugin '{pluginName}' not found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"# Plugin: {plugin.Name}");
            sb.AppendLine();
            sb.AppendLine("## Functions:");

            foreach (var function in plugin.GetFunctionsMetadata())
            {
                sb.AppendLine($"### {function.Name}");
                if (!string.IsNullOrEmpty(function.Description))
                {
                    sb.AppendLine($"*{function.Description}*");
                }

                if (function.Parameters.Any())
                {
                    sb.AppendLine("#### Parameters:");
                    foreach (var param in function.Parameters)
                    {
                        sb.AppendLine($"- **{param.Name}**");
                        sb.AppendLine($"  - Description: {param.Description}");
                        if (param.DefaultValue != null)
                        {
                            sb.AppendLine($"  - Default value: `{param.DefaultValue}`");
                        }
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
