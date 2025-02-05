using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SpecOps.Classes;
using SpecOps.Models;
using SpecOps.Services;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace SpecOps.Pages.User
{
    // Unnecessary now that we'll restrict the entire site to members of our groups
    //[AuthorizeOr(new[] { SecurityPolicy.User, SecurityPolicy.Admin })]
    public class ScriptsModel : PageModel
    {
        private readonly ILogger<ScriptsModel> Logger;
        private readonly IScriptService ScriptService;
        private IMemoryCache MemoryCache { get; set; }

        public SelectList Categories { get; set; }
        public string OutputLevels { get; set; }

        private readonly AppSettings appSettings;

        public ScriptsModel(IScriptService scriptService, ILogger<ScriptsModel> logger, IOptionsSnapshot<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            this.ScriptService = scriptService;
            this.Logger = logger;
            this.appSettings = appSettings.Value;
            this.MemoryCache = memoryCache;
        }

        // TODO: Figure out how to really make these async...
        public async Task OnGetAsync(string rpCategoryId = "", string rpScriptId = "")
        {        
            OutputLevels = JsonSerializer.Serialize(appSettings.OutputLevels);

            Categories = new SelectList(ScriptService.GetCategories());
        }

        public async Task<JsonResult> OnGetScriptsAsync(string categoryId)
        {
            return new JsonResult(ScriptService.GetScripts(categoryId));
        }

        public async Task<JsonResult> OnGetScriptAsync(string scriptId)
        {
            return new JsonResult(ScriptService.GetScript(scriptId));
        }

        public async Task OnGetCancelScriptAsync(string connectionId)
        {
            var cacheKey = $"{connectionId}|CancellationTokenSource";

            if (MemoryCache.TryGetValue(cacheKey, out CancellationTokenSource cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
