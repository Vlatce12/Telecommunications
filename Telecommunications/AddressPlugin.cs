using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Telecommunications
{
    public class AddressPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                if(context.MessageName.ToLower() == "create")
                {
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    Entity updateEntity = service.Retrieve("axm_customer", targetEntity.GetAttributeValue<EntityReference>("axm_customer_contractid").Id, new ColumnSet("axm_street", "axm_city", "axm_country"));
                    if(updateEntity == null || targetEntity == null)
                    {
                        return;
                    }
                    else
                    {
                        
                        if (targetEntity.GetAttributeValue<String>("axm_contractname") == null)
                        {
                            throw new InvalidPluginExecutionException("You must provide a Contract Name");
                        }
                        else { 
                        targetEntity["axm_street"] = updateEntity.GetAttributeValue<String>("axm_street");
                        targetEntity["axm_city"] = updateEntity.GetAttributeValue<String>("axm_city");
                        targetEntity["axm_country"] = updateEntity.GetAttributeValue<String>("axm_country");
                        service.Update(targetEntity);
                        }
                    }
                }
            }
        }
    }
}
