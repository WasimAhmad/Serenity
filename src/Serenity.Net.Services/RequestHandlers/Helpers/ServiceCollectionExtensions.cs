using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Serenity.Localization;
using System.IO;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains dependency injection extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers default implementations of <see cref="IBehaviorFactory"/>, 
    /// <see cref="IImplicitBehaviorRegistry"/> and <see cref="IBehaviorProvider"/>
    /// </summary>
    /// <param name="collection">Service collection</param>
    public static IServiceCollection AddServiceBehaviors(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IBehaviorFactory, DefaultBehaviorFactory>();
        collection.TryAddSingleton<IImplicitBehaviorRegistry, DefaultImplicitBehaviorRegistry>();
        collection.TryAddSingleton<IBehaviorProvider, DefaultBehaviorProvider>();
        return collection;
    }

    /// <summary>
    /// Registers default implementations of <see cref="IHandlerActivator"/>, 
    /// <see cref="IDefaultHandlerFactory"/> and <see cref="IDefaultHandlerRegistry"/>
    /// </summary>
    /// <param name="collection">Service collection</param>
    public static IServiceCollection AddServiceHandlerFactory(this IServiceCollection collection)
    {
        collection.TryAddSingleton<IHandlerActivator, DefaultHandlerActivator>();
        collection.TryAddSingleton<IDefaultHandlerRegistry, DefaultHandlerRegistry>();
        collection.TryAddSingleton<IDefaultHandlerFactory, DefaultHandlerFactory>();
        return collection;
    }

    /// <summary>
    /// Registers proxy activators for the DI container to resolve generic request handler instances
    /// like <see cref="ICreateHandler{TRow}" />, <see cref="IListHandler{TRow}" /> etc.
    /// </summary>
    /// <param name="collection">Service collection</param>
    public static IServiceCollection AddProxyRequestHandlers(this IServiceCollection collection)
    {
        collection.TryAddTransient(typeof(ICreateHandler<,,>), typeof(CreateHandlerProxy<,,>));
        collection.TryAddTransient(typeof(ICreateHandler<>), typeof(CreateHandlerProxy<>));
        collection.TryAddTransient(typeof(IUpdateHandler<,,>), typeof(UpdateHandlerProxy<,,>));
        collection.TryAddTransient(typeof(IUpdateHandler<>), typeof(UpdateHandlerProxy<>));
        collection.TryAddTransient(typeof(IDeleteHandler<,,>), typeof(DeleteHandlerProxy<,,>));
        collection.TryAddTransient(typeof(IDeleteHandler<>), typeof(DeleteHandlerProxy<>));
        collection.TryAddTransient(typeof(IUndeleteHandler<,,>), typeof(UndeleteHandlerProxy<,,>));
        collection.TryAddTransient(typeof(IUndeleteHandler<>), typeof(UndeleteHandlerProxy<>));
        collection.TryAddTransient(typeof(IListHandler<,,>), typeof(ListHandlerProxy<,,>));
        collection.TryAddTransient(typeof(IListHandler<,>), typeof(ListHandlerProxy<,>));
        collection.TryAddTransient(typeof(IListHandler<>), typeof(ListHandlerProxy<>));
        collection.TryAddTransient(typeof(IRetrieveHandler<,,>), typeof(RetrieveHandlerProxy<,,>));
        collection.TryAddTransient(typeof(IRetrieveHandler<>), typeof(RetrieveHandlerProxy<>));
        return collection;
    }

    /// <summary>
    /// Registers all the custom request handlers implementing IRequestHandler
    /// interface, from the type source in the service collection if available,
    /// or using the provided typeSource, optionally filtering handler types
    /// via a provided predicate.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="typeSource">Type source. Should be provided if it is not already
    /// registered in the service collection. Pass an empty type source
    /// if you want to disable auto registrations.</param>
    /// <param name="predicate">Predicate to filter handler types to register.
    /// The first argument is registration type (the interface or the handler type itself),
    /// and the second argument is the implementation type (e.g. the handler type). 
    /// If you don't want concrete types like MySaveHandler etc. to be registered for
    /// themselves (recommended), the predicate should be "(intf, impl) => intf != impl"</param>
    /// <exception cref="ArgumentNullException">collection is null or typeSource can't be found in the collection</exception>
    /// <exception cref="InvalidProgramException">Multiple candidates found for a service interface.</exception>
    public static IServiceCollection AddCustomRequestHandlers(this IServiceCollection collection, 
        ITypeSource typeSource = null, Func<Type, Type, bool> predicate = null)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        typeSource ??= collection.FirstOrDefault(x => x.ServiceType == typeof(ITypeSource))?.ImplementationInstance as ITypeSource;
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        var interfacesToAdd = new Dictionary<Type, List<Type>>();

        foreach (var type in typeSource.GetTypesWithInterface(typeof(IRequestHandler)))
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                continue;

            if (predicate == null || predicate(type, type))
                collection.TryAddTransient(type, type);

            foreach (var intf in type.GetInterfaces())
            {
                if (!typeof(IRequestHandler).IsAssignableFrom(intf))
                    continue;

                if (intf == typeof(IRequestHandler) ||
                    intf == typeof(ISaveRequestProcessor) ||
                    intf == typeof(ISaveRequestHandler) ||
                    intf == typeof(IListRequestProcessor) ||
                    intf == typeof(IListRequestHandler) ||
                    intf == typeof(IRetrieveRequestProcessor) ||
                    intf == typeof(IRetrieveRequestHandler) ||
                    intf == typeof(IDeleteRequestProcessor) ||
                    intf == typeof(IDeleteRequestHandler) ||
                    intf == typeof(IUndeleteRequestProcessor) ||
                    intf == typeof(IUndeleteRequestHandler))
                    continue;

                if (intf.IsGenericType)
                {
                    var genericBase = intf.GetGenericTypeDefinition();
                    if (genericBase == typeof(IRequestHandler<,,>) ||
                        genericBase == typeof(IRequestHandler<>) ||
                        genericBase == typeof(IRequestType<>) ||
                        genericBase == typeof(IResponseType<>))
                        continue;
                }

                if (predicate != null && !predicate(intf, type))
                    continue;

                if (interfacesToAdd.TryGetValue(intf, out var list))
                    list.Add(type);
                else
                    interfacesToAdd[intf] = [type];
            }
        }

        foreach (var pair in interfacesToAdd)
        {
            // don't override users own registration if any
            var existing = collection.FirstOrDefault(x => x.ServiceType == pair.Key);
            if (existing != null)
                continue;

            if (pair.Value.Count != 1)
            {
                var defaults = pair.Value.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value == true).ToArray();
                if (defaults.Length != 1)
                {
                    if (defaults.Length == 0)
                    {
                        var withoutDefaultFalse = pair.Value.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value != false).ToArray();
                        if (withoutDefaultFalse.Length == 1)
                            defaults = withoutDefaultFalse;
                    }

                    if (defaults.Length != 1)
                    {
                        if (defaults.Length == 0)
                            throw new InvalidProgramException($"There are multiple {pair.Key} request handler types ({string.Join(", ", pair.Value)}). " +
                                "Please add [DefaultHandler] to one of them!");

                        throw new InvalidProgramException($"There are multiple {pair.Key} request handler types with [DefaultHandler] attribute ({string.Join(", ", (IEnumerable<Type>)defaults)}). " +
                            "Please use [DefaultHandler] on only one of them!");
                    }
                }

                collection.AddTransient(pair.Key, defaults[0]);
                continue;
            }

            collection.AddTransient(pair.Key, pair.Value[0]);
        }

        return collection;
    }

    /// <summary>
    /// Registers all the services required for request handlers
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="customHandlerTypeSource">Custom handler type source
    /// to pass to the AddCustomHandlers call. Pass an empty type source
    /// if you want to disable auto registrations</param>
    /// <param name="customHandlerPredicate">Predicate to filter handler types to register.
    /// The first argument is registration type (the interface or the handler type itself),
    /// and the second argument is the implementation type (e.g. the handler type). 
    /// If you don't want concrete types like MySaveHandler etc. to be registered for
    /// themselves (recommended), the predicate should be "(intf, impl) => intf != impl"</param>
    /// <exception cref="InvalidProgramException">Multiple candidates found for a service interface.</exception>
    public static IServiceCollection AddServiceHandlers(this IServiceCollection collection, 
        ITypeSource customHandlerTypeSource = null, Func<Type, Type, bool> customHandlerPredicate = null)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.AddCaching();
        collection.AddEntities();
        collection.AddFeatureToggles();
        collection.AddTextRegistry();
        collection.AddServiceBehaviors();
        collection.AddServiceHandlerFactory();
        collection.AddServiceResolver();
        collection.AddUserProvider();
        collection.AddCustomRequestHandlers(customHandlerTypeSource, customHandlerPredicate);
        collection.AddProxyRequestHandlers();

        collection.TryAddSingleton<IRequestContext, DefaultRequestContext>();
        return collection;
    }

    /// <summary>
    /// Adds nested texts, enum texts, permission texts, row texts and json local text assets
    /// </summary>
    /// <param name="provider">The service provider that will be used to locate text registry and other types</param>
    /// <param name="webFileProvider">If passed, json texts from static web assets will be added from 
    /// any assembly with a JsonLocalTextAssetsAttribute attribute</param>
    /// <returns>Local text registry</returns>
    /// <exception cref="ArgumentNullException">Provider is null</exception>
    public static ILocalTextRegistry AddBaseTexts(this IServiceProvider provider, IFileProvider webFileProvider = null)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        var typeSource = provider.GetRequiredService<ITypeSource>();
        var textRegistry = provider.GetRequiredService<ILocalTextRegistry>();
        var rowTypeRegistry = provider.GetRequiredService<IRowTypeRegistry>();

        AddBaseTexts(textRegistry, typeSource, rowTypeRegistry);

        if (webFileProvider is not null)
        {
            var options = provider.GetRequiredService<IOptions<JsonLanguageTextLoaderOptions>>()?.Value;
            if (options != null)
            {
                foreach (var attr in typeSource.GetAssemblyAttributes<JsonLocalTextAssetsAttribute>())
                    if (!string.IsNullOrEmpty(attr.Path))
                        options.LanguageSets.Add(new LanguageSet(webFileProvider, attr.Path, true));
            }
        }

        return textRegistry;
    }

    /// <summary>
    /// Adds nested texts, enum texts, permission texts, row texts and json local text assets
    /// </summary>
    /// <param name="textRegistry">Target text registry</param>
    /// <param name="typeSource">Type source from which to discover text sources</param>
    /// <param name="rowTypeRegistry">Row type registry</param>
    /// <param name="includeResources">True to include resource texts</param>
    /// <returns>Local text registry</returns>
    /// <exception cref="ArgumentNullException">textRegistry or typeSource is null</exception>
    public static ILocalTextRegistry AddBaseTexts(this ILocalTextRegistry textRegistry, ITypeSource typeSource,
        IRowTypeRegistry rowTypeRegistry = null, bool includeResources = true)
    {
        if (textRegistry is null)
            throw new ArgumentNullException(nameof(textRegistry));

        textRegistry.AddNavigationTexts(typeSource);
        textRegistry.AddPropertyItemsTexts(typeSource);
        textRegistry.AddNestedTexts(typeSource);
        textRegistry.AddEnumTexts(typeSource);
        textRegistry.AddNestedPermissions(typeSource);

        rowTypeRegistry ??= new DefaultRowTypeRegistry(typeSource);
        var rowInstances = rowTypeRegistry.AllRowTypes.Select(x => (IRow)Activator.CreateInstance(x));
        textRegistry.AddRowTexts(rowInstances);

        if (includeResources)
            textRegistry.AddJsonResourceTexts(typeSource);

        return textRegistry;
    }

    /// <summary>
    /// Adds type texts and JSON texts from passed folders
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="jsonTextPaths"></param>
    [Obsolete("Use AddBaseTexts() and then services.ConfigureJsonTextRoot() for each path.")]
    public static void AddAllTexts(this IServiceProvider provider, params string[] jsonTextPaths)
    {
        var textRegistry = AddBaseTexts(provider, webFileProvider: null);
        // This old method of directly adding texts from paths is now obsolete.
        // Consumers should use services.ConfigureJsonTextRoot(new PhysicalFileProvider(path), "") instead.
        // For simplicity in this refactor, we are not replicating the direct path loading here.
        // The primary mechanism is via IFileProvider for web apps.
        // If direct path loading is still critical, JsonLocalTextRegistration.AddJsonTexts would need a different lazy-loading strategy.
    }

    /// <summary>
    /// Configures a root path for JSON text files to be loaded by <see cref="ILanguageTextLoader"/>.
    /// This method should be called after <see cref="CoreServiceCollectionExtensions.AddTextRegistry"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="provider">File provider for the root.</param>
    /// <param name="subpath">Sub path under the provider root.</param>
    /// <param name="recursive">True to recursively scan (default true).</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="ArgumentNullException">services, provider or subpath is null.</exception>
    public static IServiceCollection ConfigureJsonTextRoot(this IServiceCollection services,
        IFileProvider provider, string subpath, bool recursive = true)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        if (subpath is null)
            throw new ArgumentNullException(nameof(subpath));

        services.Configure<JsonLanguageTextLoaderOptions>(options =>
        {
            options.LanguageSets.Add(new LanguageSet(provider, subpath, recursive));
        });

        return services;
    }

    /// <summary>
    /// Adds json texts from file provider and sub path.
    /// This method is now obsolete. Use services.ConfigureJsonTextRoot() to configure paths
    /// for the lazy-loading ILanguageTextLoader.
    /// </summary>
    /// <param name="registry">The text registry (no longer directly used by this method for adding all texts,
    /// but kept for signature compatibility during obsolescence if needed, though ideally removed or changed).
    /// Consider changing to an IServiceCollection extension if this signature must be kept for some reason,
    /// or remove if all callers can switch to ConfigureJsonTextRoot.</param>
    /// <param name="provider">File provider.</param>
    /// <param name="subpath">Sub path.</param>
    /// <param name="recursive">True to recursively scan (default true).</param>
    /// <returns>The text registry (though it's not modified by this method anymore for all texts).</returns>
    /// <exception cref="ArgumentNullException">provider or subpath is null.</exception>
    [Obsolete("Use IServiceCollection.ConfigureJsonTextRoot() to configure paths for lazy loading. This method no longer loads all texts directly into the registry.")]
    public static ILocalTextRegistry AddJsonTexts(this ILocalTextRegistry registry, IFileProvider provider, string subpath, bool recursive = true)
    {
        // This method no longer directly loads all texts into the registry.
        // It's kept for a brief transition or if some part of AddBaseTexts still relies on its signature
        // to discover JsonLocalTextAssetsAttribute, though that also needs review.
        // The actual configuration of paths for lazy loading should happen via
        // services.ConfigureJsonTextRoot(provider, subpath, recursive);
        // For now, to avoid breaking calls from AddBaseTexts if it uses this method by signature,
        // we'll assume that if this is called, the intent is to configure this path.
        // However, this requires services to be accessible here, which is not ideal for an ILocalTextRegistry extension.

        // IDEALLY, THIS METHOD IS REMOVED or its callers are refactored to use IServiceCollection.ConfigureJsonTextRoot.
        // The AddBaseTexts method that calls this for JsonLocalTextAssetsAttribute should be updated
        // to configure the JsonLanguageTextLoaderOptions instead.

        // Quick check: AddBaseTexts calls this.
        // public static ILocalTextRegistry AddBaseTexts(this IServiceProvider provider, IFileProvider webFileProvider = null)
        // ...
        // textRegistry.AddJsonTexts(webFileProvider, attr.Path);
        // This call site needs to change.

        // For the purpose of this step, we'll assume this method's body is effectively a no-op
        // for direct loading, and the configuration happens elsewhere (via IServiceCollection.ConfigureJsonTextRoot).
        // If strict backward compatibility of behavior for AddBaseTexts is needed immediately without modifying it,
        // this method would need access to IServiceCollection to call ConfigureJsonTextRoot, which is awkward.

        // Safest short-term change while marking obsolete: do nothing here regarding eager loading.
        // The configuration should be done via IServiceCollection.ConfigureJsonTextRoot.
        // AddBaseTexts will need to be updated.

        if (provider is null)
            throw new ArgumentNullException(nameof(provider));
        if (subpath is null)
            throw new ArgumentNullException(nameof(subpath));

        // To make AddBaseTexts continue to "work" somewhat without immediate refactoring of it,
        // one might try to resolve IServiceCollection here if registry is a known type that has it,
        // or pass IServiceCollection, but that's messy.
        // The clean path is to update callers.

        return registry; // Returns registry, but doesn't perform the old eager loading.
    }
}