using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BookShop.Areas.Admin.Services;

public class MVCActionsDiscoveryService : IMVCActionsDiscoveryService
{
    private readonly ConcurrentDictionary<string, Lazy<ICollection<ControllerViewModel>>> _allSecuredActionsWithPloicy = new ConcurrentDictionary<string, Lazy<ICollection<ControllerViewModel>>>();

    public ICollection<ControllerViewModel> MvcControllers { get; }

    public MVCActionsDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        if (actionDescriptorCollectionProvider is null)
            throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));

        MvcControllers = new List<ControllerViewModel>();
        var lastControllerName = string.Empty;
        ControllerViewModel currentController = null;

        var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;
        foreach (var actionDescriptor in actionDescriptors)
        {
            if (!(actionDescriptor is ControllerActionDescriptor descriptor))
                continue;

            var controllerTypeInfo = descriptor.ControllerTypeInfo;
            var actionMethodInfo = descriptor.MethodInfo;

            if (lastControllerName != descriptor.ControllerName)
            {
                currentController = new ControllerViewModel
                {
                    AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                    ControllerAttributes = GetAttributes(controllerTypeInfo),
                    ControllerDisplayName = controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    ControllerName = descriptor.ControllerName,
                };
                MvcControllers.Add(currentController);

                lastControllerName = descriptor.ControllerName;
            }

            currentController?.Actions.Add(new ActionViewModel
            {
                ControllerId = currentController.ControllerId,
                ActionName = descriptor.ActionName,
                ActionDisplayName = actionMethodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                ActionAttributes = GetAttributes(actionMethodInfo),
                IsSecuredAction = IsSecuredAction(controllerTypeInfo, actionMethodInfo)
            });
        }
    }

    public ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName)
    {
        var getter = _allSecuredActionsWithPloicy.GetOrAdd(policyName, y => new Lazy<ICollection<ControllerViewModel>>(
            () =>
            {
                var controllers = new List<ControllerViewModel>(MvcControllers);
                foreach (var controller in controllers)
                {
                    controller.Actions = controller.Actions.Where(
                        model => model.IsSecuredAction &&
                        (
                        model.ActionAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName ||
                        controller.ControllerAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName
                        )).ToList();
                }
                return controllers.Where(model => model.Actions.Any()).ToList();
            }));
        return getter.Value;
    }

    private static List<Attribute> GetAttributes(MemberInfo actionMethodInfo)
    {
        return actionMethodInfo.GetCustomAttributes(inherit: true)
                               .Where(attribute =>
                               {
                                   var attributeNamespace = attribute.GetType().Namespace;
                                   return attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                                          attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;
                               }).Cast<Attribute>().ToList();
    }

    private static bool IsSecuredAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
    {
        var actionHasAllowAnonymousAttribute = actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(inherit: true) != null;
        if (actionHasAllowAnonymousAttribute)
            return false;

        var controllerHasAuthorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
        if (controllerHasAuthorizeAttribute)
            return true;

        var actionMethodHasAuthorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
        if (actionMethodHasAuthorizeAttribute)
            return true;

        return false;
    }
}
