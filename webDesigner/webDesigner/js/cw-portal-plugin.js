var availableWebDesignerItems = ["application", "process", "technology", "location"];

function gridSelect(gridItem) {
    var ot = gridItem.GetMember("ObjectTypeScriptName").Value.toLowerCase();
    if (isWebDesignerAvailable(ot)) {
        jQuery("#panel_BottomContentCallback").addClass('webdesigner');
    } else {
        jQuery("#panel_BottomContentCallback").removeClass('webdesigner');
    }
    window.viewState.gridSelect(gridItem);
}


function isWebDesignerAvailable(objectType) {
    objectType = objectType.toLowerCase();
    return _.include(availableWebDesignerItems, objectType.toLowerCase());
}

var savedObjectTypeView = {};
$.extend(savedObjectTypeView, window.objectTypeView);

function loadWebDesignerItem(ot, id) {
    ot = ot.toLowerCase();
    jQuery("#viewlette__contentCallback").html("<div id='top_of_page'></div><div class='webdesigner-zone'></div><img class='cwloading' src='webdesigner/images/_loading.gif' alt='Loading...'>");
    cwAPI.loadUniquePageSingle(ot, id.toString());
    cwAPI.chainLoadPage(ot);
}

ObjectTypeView.prototype.setObject = function (model, objectType, id) {
    if (isWebDesignerAvailable(objectType)) {
        jQuery("#panel_BottomContentCallback").addClass('webdesigner');
    } else {
        jQuery("#panel_BottomContentCallback").removeClass('webdesigner');
    }
    savedObjectTypeView.setObject(model, objectType, id);
}


ViewState.prototype.bottomContentCallbackComplete = function () {
    var delegate = window.viewState.getCurrentView();
    var DomElementId = "";
    var NewPaneHeight = window.document.body.scrollHeight;
    var NewPaneWidth = window.document.body.scrollWidth;

    if (!loadWebDesignerFromViewState()) {
        if (delegate.bottomContentCallbackComplete) {
            return delegate.bottomContentCallbackComplete(DomElementId, NewPaneHeight, NewPaneWidth);
        }
    }
};


function loadWebDesignerFromViewState() {
    var id = window.viewState.ObjectId;
    var ot = window.viewState.ObjectType;
    if (id != null && ot != null && isWebDesignerAvailable(ot.toLowerCase())) {
        loadWebDesignerItem(ot.toLowerCase(), id);
        return true;
    }
    return false;
}



ObjectTypeView.prototype.selectTab = function (name) {
    if (name === "Proprties") {
        if (loadWebDesignerFromViewState()) {
            return;
        }
    }
    savedObjectTypeView.selectTab(name);
};


