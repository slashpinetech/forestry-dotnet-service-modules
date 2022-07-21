[![MIT License](https://img.shields.io/github/license/slashpinetech/forestry-dotnet-service-modules?color=1F3B2B&style=flat-square)](https://github.com/slashpinetech/forestry-dotnet-service-modules/blob/main/LICENSE)

# Forestry .NET -- Service Modules

Forestry .NET is a set of open-source libraries for building modern web
applications using ASP.NET Core.

This service modules package adds support for configuring services using an
object-oriented approach.

## Usage

### Simple Example

Getting started with Service Modules is easy. Create a class that implements
`IServiceModule`, like so:

```c#
public class BugsnagModule : IServiceModule
{
    public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
    {
        services.AddBugsnag(configuration =>
        {
            configuration.ApiKey = "YourApiKeyGoesHere";
            configuration.ReleaseStage = ctx.Environment.EnvironmentName;
        });
    }
}
```

Then, in your `Startup` class, add the following:

```c#
services.AddModules(typeof(Startup).Assembly, Environment, Configuration)
    .AddModule<BugsnagModule>();
```

That's the simplest use of service modules. However, we included our API key above
in the code, which is a no-no. Let's look at how to bind our configuration to a
module to keep the API key safe.

### Configuration Binding

Continuing with our Bugsnag example above, let's consider a configuration section
in our appsettings.json.

```json
{
  "Bugsnag": {
    "ApiKey": "YourApiKeyGoesHere"
  }
}
```

We can tell the service modules system to bind that configuration to our module
by adding the name of the config section to the `AddModule<T>()` call.

```c#
services.AddModules(typeof(Startup).Assembly, Environment, Configuration)
    .AddModule<BugsnagModule>("Bugsnag");
```

Let's revisit the example module and add a new property to bind to.

```c#
public class BugsnagModule : IServiceModule
{
    public string? ApiKey { get; set; }

    public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
    {
        if (!string.IsNullOrEmpty(ApiKey))
        {
            services.AddBugsnag(configuration =>
            {
                configuration.ApiKey = ApiKey;
                configuration.ReleaseStage = ctx.Environment.EnvironmentName;
            });
        }
    }
}
```

The value of our `ApiKey` JSON property will be bound to the `ApiKey` property in
our module. If we don't set an API key, we can check that the property is not null
and disable Bugsnag integration.

### Configuration Validation

In the Bugsnag example, we didn't configure our services if the API key was
null. What if we need a configuration property to be present. Let's consider an
example that configures EF core.

```json
{
  "Database": {
    "ConnectionString": "YourSqlServerConnectionStringGoesHere;",
    "EnableSensitiveDataLogging": true
  }
}
```

```c#
public class DatabaseModule : IServiceModule
{
    [Required]
    public string? ConnectionString { get; set; }

    public bool EnableSensitiveDataLogging { get; set; } = false;

    public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(ConnectionString!)
                .EnableSensitiveDataLogging(EnableSensitiveDataLogging);
        });
    }
}
```

You can use _any_ validation attributes on your properties in a module to ensure
that your configuration values are provided. If validation fails, startup will
halt, and error messages will be written to the log informing you of the
validation errors.

```c#
services.AddModules(typeof(Startup).Assembly, Environment, Configuration)
    .AddModule<BugsnagModule>("Bugsnag")
    .AddModule<DatabaseModule>("Database");
```

### Polymorphic Configuration Binding

So far, so good, right? But what if you want to bind to a different module at
runtime based on our configuration? Let's consider an example for sending email
via SMTP or an HTTP-based service like SendGrid.

Our configuration could like like either of these:

```json
{
  "Email": {
    "Type": "Smtp",
    "FromName": "My Web App",
    "FromAddress": "no-reply@example.com",
    "Hostname": "localhost",
    "Port": 25
  }
}
```

```json
{
  "Email": {
    "Type": "SendGrid",
    "FromName": "My Web App",
    "FromAddress": "no-reply@example.com",
    "ApiKey": "YourSendGridApiKey"
  }
}
```

Next, let's look at our modules.

```c#
[ServiceModuleInfo(DefaultImpl = typeof(SmtpEmailModule))]
public abstract class EmailModule : IServiceModule
{
    [Required]
    public string? FromName { get; set; }

    [Required]
    [EmailAddress]
    public string? FromAddress { get; set; }

    public abstract void Configure(IServiceCollection services, IServiceConfigurationContext ctx);
}
```

```c#
[ServiceModuleName("Smtp")]
public class SmtpEmailModule : EmailModule
{
    [Required]
    public string? Host { get; set; }

    public int Port { get; set; } = 25;

    public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
    {
        services.AddSingleton(_ => new SmtpSettings(
            new EmailAddress(FromAddress!, FromName),
            Host!,
            Port
        ));
        services.AddTransient<IEmailSender, SmtpEmailSender>();
    }
}
```

```c#
[ServiceModuleName("SendGrid")]
public class SendGridEmailModule : EmailModule
{
    [Required]
    public string? ApiKey { get; set; }

    public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
    {
        services.AddSingleton(_ => new SendGridSettings(
            new EmailAddress(FromAddress!, FromName),
            ApiKey!
        ));

        services.AddTransient<IEmailSender, SendGridEmailSender>();
    }
}
```

Then we register our module in `Startup`.

```c#
services.AddModules(typeof(Startup).Assembly, Environment, Configuration)
    .AddModule<BugsnagModule>("Bugsnag")
    .AddModule<DatabaseModule>("Database")
    .AddModule<EmailModule>("Email");
```

When registering a polymorphic module, we peek at the `Type` property to see
which implementation to bind to.
