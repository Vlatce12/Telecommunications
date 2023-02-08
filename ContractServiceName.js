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
    var parentRecordReference = Xrm.Utility.getPageContext().input.createFromEntity;
	if (parentRecordReference == null){
		return;
	}
    else{
        formContext.getAttribute("axm_summaryname").setValue(parentRecordReference.name);
    }   
    }
    var onSave = function (executionContext) {

    };
   
    return {
        OnLoad: onLoad,
        OnSave: onSave
    }
})();
