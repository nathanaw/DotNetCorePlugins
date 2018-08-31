using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using Plugin.Abstractions;
using McMaster.NETCore.Plugins;

namespace MefWebApp.Mef
{
    public class PluginCatalog : AggregateCatalog
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginCatalog"/> class from being created.
        /// </summary>
        private PluginCatalog()
        {
        }


        /// <summary>
        /// Creates catalog from the specified base path, searching through subdirectories for "plugin.config".
        /// </summary>
        /// <param name="basePath">The base path to start looking for plugins.</param>
        /// <param name="sharedTypes">The types that both the host and the plugin need to agree and share.</param>
        /// <returns>A MEF catalog containing the available plugins.</returns>
        public static PluginCatalog Create(string basePath, Type[] sharedTypes)
        {
            var pluginCatalog = new PluginCatalog();

            // We need to include the MEF related attributes, or else the composition container
            // won't find the composition parts.
            var sharedTypesPlusMef = sharedTypes.ToList();
            sharedTypesPlusMef.Add(typeof(ExportAttribute));
            sharedTypesPlusMef.Add(typeof(ExportMetadataAttribute));
            sharedTypesPlusMef.Add(typeof(ImportAttribute));
            sharedTypesPlusMef.Add(typeof(ImportManyAttribute));
            sharedTypesPlusMef.Add(typeof(ImportingConstructorAttribute));

            foreach (var pluginFile in Directory.GetFiles(basePath, "plugin.config", SearchOption.AllDirectories))
            {
                // Read the config so that we can determine the MainAssembly.
                // This is important because the PluginLoader may load many assemblies in the plugin directory.
                // Also of note is that the LoadDefaultAssembly() may not load the MainAssembly.
                var config = PluginConfig.CreateFromFile(pluginFile);

                var loader = PluginLoader.CreateFromConfigFile(pluginFile, sharedTypesPlusMef.ToArray());

                // Load the MainAssembly into the MEF container.
                // We should only need the main assembly for MEF to do it's thing.
                var catalog = new AssemblyCatalog(loader.LoadAssembly(config.MainAssembly));

                // Add this plugin's parts to the larger aggregate.
                pluginCatalog.Catalogs.Add(catalog);
            }

            return pluginCatalog;
        }


    }
}
