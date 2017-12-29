using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.Data.ItemResolvers;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.SecurityModel;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.StringExtensions;

namespace YOUR_NAME_SPACE
{
    /// <summary> Hocks into app start of sitecore to add custom code eg. bundles or IoC </summary>
    public class CustomItemResolver : ItemResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Pipelines.HttpRequest.ItemResolver" /> class.
        /// </summary>
        /// <param name="itemManager">The item manager.</param>
        /// <param name="pathResolver">The path resolver.</param>
        public CustomItemResolver(BaseItemManager itemManager, ItemPathResolver pathResolver)
            : this(itemManager, pathResolver, Settings.ItemResolving.FindBestMatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Pipelines.HttpRequest.ItemResolver" /> class.
        /// </summary>
        /// <param name="itemManager">The item manager.</param>
        /// <param name="pathResolver">The path resolver.</param>
        /// <param name="itemNameResolvingMode">The item name resolving mode.</param>
        protected CustomItemResolver(BaseItemManager itemManager, ItemPathResolver pathResolver, MixedItemNameResolvingMode itemNameResolvingMode)
            :base(itemManager,pathResolver, itemNameResolvingMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Pipelines.HttpRequest.ItemResolver" /> class.
        /// </summary>
        [Obsolete("Please use another constructor with parameters")]
        public CustomItemResolver()
            : this(ServiceLocator.ServiceProvider.GetRequiredService<BaseItemManager>(), ServiceLocator.ServiceProvider.GetRequiredService<ItemPathResolver>())
        {
        }

        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            base.Process(args);
            if (this.SkipItemResolving(args))
                return;
            bool flag = false;
            Item obj1 = (Item)null;
            string str = string.Empty;
            var url = System.Web.HttpContext.Current.Request.Url;
            var siteContext = Sitecore.Sites.SiteContextFactory.GetSiteContext(url.Host, url.PathAndQuery);
            try
            {
                this.StartProfilingOperation("Resolve current item.", args);
                HashSet<string> stringSet = new HashSet<string>();
                foreach (string candidatePath in this.GetCandidatePaths(args))
                {
                    if (stringSet.Add(candidatePath))
                    {
                        if (this.TryResolveItem(candidatePath, args, out obj1, out flag))
                        {
                            str = candidatePath;
                            break;
                        }
                        if (flag)
                            return;
                    }
                }
                
                SiteContext site = siteContext;
                if (obj1 == null || obj1.Name.Equals("*"))
                {
                    Item obj2 = this.ResolveByMixedDisplayName(args, out flag);
                    if (obj2 != null)
                        obj1 = obj2;
                }
                if (obj1 != null || site == null || (flag || !this.UseSiteStartPath(args)) || !this.TryResolveItem(site.StartPath, args, out obj1, out flag))
                    return;
                str = site.StartPath;
            }
            finally
            {
                args.PermissionDenied = flag;
            }
        }
    }
}