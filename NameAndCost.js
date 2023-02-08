if (typeof (axm) === "undefined") {
    axm = {
        __namespace: true
    };
}
if (typeof (axm.tableName) === "undefined") {
    axm.tableName = {
        __namespace: true
    };
}
axm.tableName.name = (function () {
    var onLoad = function (executionContext) {
        var formContext = executionContext.getFormContext();
        var lookup = formContext.getAttribute("axm_service_servicelineid").getValue();
       if(lookup==null) return;
       else{
        Xrm.WebApi.retrieveRecord("axm_services", lookup[0].id, "?$select=axm_cost").then(
            function success(result) {
                formContext.getAttribute("axm_cost").setValue(result.axm_cost);
            },
        );
       };
    }
    var onSave = function (executionContext) {
            var formContext = executionContext.getFormContext();
            var parentRecordReference = Xrm.Utility.getPageContext().input.createFromEntity;
            if (parentRecordReference == null){
                return;
            }
            else{
                formContext.getAttribute("axm_servicename").setValue(parentRecordReference.name);

            }   
        };
    return {
        OnLoad: onLoad,
        OnSave: onSave,

    }
})();
