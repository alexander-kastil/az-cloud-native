# API Versions and Revisions

Examine `config-api` in the current folder. It includes samples for API versions and revisions. Typically versions are used to separate API versions with breaking changes, while revisions can be used for minor and non-breaking changes to an API.

Sample Version: An additional method was added to `FoodController.cs` and all methods have be refactored to use an async pattern.

```c#
[HttpGet("{id}")]
public async Task<FoodItem> GetById(int id)
{
        return await ctx.Food.FirstOrDefaultAsync(v => v.ID == id);
}
```

Sample Revision: A mock parameter has been added to the GetSettings() method in `SettingsController.cs`. A change in the signature of an Api is treated as a breaking change. 

```c#
[HttpGet]
public ActionResult GetSettings(bool ShowAll)
{
    //access a single key
    var useSQLite = cfg.GetValue<string>("AppSettings:UseSQLite");
    
    //get string typed config
    var config = cfg.Get<AppConfig>();
    return Ok(config);  
}
```

Rebuild the container and publish it to ACA using `publish-change.azcli`. 
