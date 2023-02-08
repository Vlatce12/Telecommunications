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
    debugger;
    var onSave = function (executionContext) {
        var formContext = executionContext.getFormContext();
        if(formContext == null){
            return;
        }
        else{
        if(formContext.getAttribute("axm_contractname").getValue() == null){
            Xrm.Navigation.openErrorDialog({ errorCode:"ABC7123", details:"ERROR", message:"You must provide a Contract" }).then(
                function (success) {
                console.log(success);        
            },
                function (error) {
                console.log(error);             
            });
        }  
        else {
            return;
        }   
        }
    };      

var onLoad = function (executionContext) {

        };                         
return {
    OnLoad: onLoad,
    OnSave: onSave
}
})();