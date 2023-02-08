using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Calculations
{
    public class Calculations : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                if (context.MessageName.ToLower() == "create")
                {
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    if (targetEntity == null)
                    {
                        return;
                    }
                    else
                    {   
                             QueryExpression query = new QueryExpression()
                        {
                            EntityName = "axm_serviceline",
                            ColumnSet = new ColumnSet("axm_service_servicelineid", "axm_contract_servicelineid", "axm_cost", "axm_margin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values ={
                                       service.Retrieve("axm_services", targetEntity.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id, new ColumnSet()).Id,
                                    }
                                        },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values = {
                                       service.Retrieve("axm_contract", targetEntity.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet()).Id,
                                        }
                                    }
                                }
                            }
                        };
                        EntityCollection entityCollection = service.RetrieveMultiple(query);

                        CalculateRollupFieldRequest request = new CalculateRollupFieldRequest()
                        {
                            Target = new EntityReference("axm_contract", service.Retrieve("axm_contract", targetEntity.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet("axm_totallsll")).Id),
                            FieldName = ("axm_totallsll")
                        };
                    CalculateRollupFieldResponse response = (CalculateRollupFieldResponse)service.Execute(request);

                        QueryExpression queryContractService = new QueryExpression()
                        {
                            EntityName = "axm_contractservice",
                            ColumnSet = new ColumnSet("axm_service_contractserviceid","axm_contract_contractserviceid", "axm_totalcost","axm_totalmargin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_contractserviceid",
                                        Operator= ConditionOperator.Equal,
                                        Values=
                                        {
                                            targetEntity.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id,
                                        }
                                },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_contractserviceid",
                                        Operator = ConditionOperator.Equal,
                                        Values =
                                        {
                                            targetEntity.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id,
                                        }
                                    }
                                }
                            }
                        };
                         EntityCollection entityCollectionContractServ = service.RetrieveMultiple(queryContractService);

                        if(entityCollectionContractServ.Entities.Count != 0) {
                        Entity entityContServ = entityCollectionContractServ[0];
                        entityContServ["axm_totalcost"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_cost").Value)));
                        entityContServ["axm_totalmargin"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_margin").Value)));
                        service.Update(entityContServ);
                            }
                        else
                        {
                                throw new InvalidPluginExecutionException("Choosen service is not added in Contract Service");
                        }
                    }
                }
                if (context.MessageName.ToLower() == "update")
                {
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    Entity preImage = context.PreEntityImages["preImage"] as Entity;

                    if (targetEntity == null || preImage == null)
                    {
                        return;
                    }
                    else
                    {
                     QueryExpression query = new QueryExpression()
                        {
                            EntityName = "axm_serviceline",
                            ColumnSet = new ColumnSet("axm_service_servicelineid", "axm_contract_servicelineid", "axm_cost", "axm_margin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values ={
                                         service.Retrieve("axm_services", preImage.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id, new ColumnSet()).Id,
                                    }
                                        },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values = {
                                        service.Retrieve("axm_contract", preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet()).Id,
                                        }
                                    }
                                }
                            }
                        };
                             EntityCollection entityCollection = service.RetrieveMultiple(query);

                            CalculateRollupFieldRequest request = new CalculateRollupFieldRequest()
                        {
                            Target = new EntityReference("axm_contract",service.Retrieve("axm_contract", preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet("axm_totallsll")).Id),
                            FieldName = ("axm_totallsll")
                        };
                    CalculateRollupFieldResponse response = (CalculateRollupFieldResponse)service.Execute(request);
                         QueryExpression queryContractService = new QueryExpression()
                        {
                            EntityName = "axm_contractservice",
                            ColumnSet = new ColumnSet("axm_service_contractserviceid","axm_contract_contractserviceid", "axm_totalcost","axm_totalmargin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_contractserviceid",
                                        Operator= ConditionOperator.Equal,
                                        Values=
                                        {
                                            preImage.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id,
                                        }
                                },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_contractserviceid",
                                        Operator = ConditionOperator.Equal,
                                        Values =
                                        {
                                            preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id,
                                        }
                                    }
                                }
                            }
                        };
                        EntityCollection entityCollectionContractServ = service.RetrieveMultiple(queryContractService);
                        Entity entityContServ = entityCollectionContractServ[0];
                        entityContServ["axm_totalcost"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_cost").Value)));
                        entityContServ["axm_totalmargin"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_margin").Value)));
                        service.Update(entityContServ);
                    }
                }
            }
            else
            {
                if (context.MessageName.ToLower() == "delete")
                {
                    
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    Entity preImage = context.PreEntityImages["preImage"] as Entity;

                    if (preImage == null)
                    {
                        return;
                    }
                    else
                    {
                         QueryExpression query = new QueryExpression()
                        {
                            EntityName = "axm_serviceline",
                            ColumnSet = new ColumnSet("axm_service_servicelineid", "axm_contract_servicelineid", "axm_cost", "axm_margin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values ={
                                         service.Retrieve("axm_services", preImage.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id, new ColumnSet()).Id,
                                    }
                                        },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_servicelineid",
                                        Operator = ConditionOperator.Equal,
                                        Values = {
                                         service.Retrieve("axm_contract", preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet()).Id,
                                        }
                                    }
                                }
                            }
                        };
                             EntityCollection entityCollection = service.RetrieveMultiple(query);

                            CalculateRollupFieldRequest request = new CalculateRollupFieldRequest()
                        {
                            Target = new EntityReference("axm_contract",service.Retrieve("axm_contract", preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id, new ColumnSet("axm_totallsll")).Id),
                            FieldName = ("axm_totallsll")
                        };
                    CalculateRollupFieldResponse response = (CalculateRollupFieldResponse)service.Execute(request);

                          QueryExpression queryContractService = new QueryExpression()
                        {
                            EntityName = "axm_contractservice",
                            ColumnSet = new ColumnSet("axm_service_contractserviceid","axm_contract_contractserviceid", "axm_totalcost","axm_totalmargin"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_service_contractserviceid",
                                        Operator= ConditionOperator.Equal,
                                        Values=
                                        {
                                            preImage.GetAttributeValue<EntityReference>("axm_service_servicelineid").Id,
                                        }
                                },
                                    new ConditionExpression
                                    {
                                        AttributeName = "axm_contract_contractserviceid",
                                        Operator = ConditionOperator.Equal,
                                        Values =
                                        {
                                            preImage.GetAttributeValue<EntityReference>("axm_contract_servicelineid").Id,
                                        }
                                    }
                                }
                            }
                        };
                        EntityCollection entityCollectionContractServ = service.RetrieveMultiple(queryContractService);

                        Entity entityContServ = entityCollectionContractServ[0];
                        entityContServ["axm_totalcost"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_cost").Value)));
                        entityContServ["axm_totalmargin"] = new Money(entityCollection.Entities.Sum(o=>Convert.ToDecimal(o.GetAttributeValue<Money>("axm_margin").Value)));
                        service.Update(entityContServ);
                    }
                }
            }
        }
    }
}
    
