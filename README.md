# Sitecore_9_permission_bug_fix
To use this fix is quite easy, add the class into one of your libraries and add the following patch

The code does exactly the same thing as the sitecore.kernel the only difference is the args.PermissionDenied = flag; which was present on SC 8.2 and got removed on SC 9.0 (initial release)

ps. don't forget to install the nuget Microsoft.Extensions.DependencyInjection.Abstractions.

<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <pipelines>
      <httpRequestBegin>
        <processor patch:instead="processor[@type='Sitecore.Pipelines.HttpRequest.ItemResolver, Sitecore.Kernel']" type="YOUR_NAME_SPACE.CustomPipelines.CustomItemResolver, YOUR_DLL_NAME" />
      </httpRequestBegin>
    </pipelines>
  </sitecore>
</configuration>