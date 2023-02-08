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
    var onLoad = function (executionContext) {
        var formContext = executionContext.getFormContext();
        var checkMethod = formContext.getAttribute("axm_preferredmethodofcontact").getValue();

        if(checkMethod==282460000){
            formContext.getControl("axm_phone").setVisible(false);
            Xrm.Page.ui.tabs.get("General_tab").sections.get("tab_2_section_1").setVisible(false);
            formContext.getControl("axm_email").setVisible(true);    
        }
        else if(checkMethod==282460001)  {
            formContext.getControl("axm_email").setVisible(false);
            Xrm.Page.ui.tabs.get("General_tab").sections.get("tab_2_section_1").setVisible(false);;
            formContext.getControl("axm_phone").setVisible(true);     
        }
        else{
            Xrm.Page.ui.tabs.get("General_tab").sections.get("tab_2_section_1").setVisible(true);;
            formContext.getControl("axm_email").setVisible(false);    
            formContext.getControl("axm_phone").setVisible(false);    
        }
    };      
var onSave = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var firstName = formContext.getAttribute("axm_firstname").getValue();
    var lastName = formContext.getAttribute("axm_lastname").getValue();
   if(firstName==null) return;
   else{
   var together=firstName;
    formContext.getAttribute("axm_customername").setValue(together);
   };
   if(lastName==null) return;
   else{
    together = together +" "+ lastName;
    formContext.getAttribute("axm_customername").setValue(together);
   }
};                        
return {
    OnLoad: onLoad,
    OnSave: onSave
}
})();