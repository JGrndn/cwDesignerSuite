/*global cwAPI:true, cwBehaviours :true */
cwBehaviours.cwTreeView = {};

// transform un li en accordion
cwBehaviours.cwTreeView.setTreeView = function (sidetreecontrol, container, place, collapsed) {
    cwBehaviours.cwTreeView.removeParent(container);
    if (jQuery(sidetreecontrol).length == 0) {
        jQuery(place).prepend('<div id="sidetreecontrol"><a href="#">Collapse All</a> | <a href="#">Expand All</a></div><br/>');
       // jQuery('<div id="sidetreecontrol"><a href="?#">Collapse All</a> | <a href="?#">Expand All</a></div>').insertAfter(place);
    }
    jQuery(container).treeview({
        collapsed: collapsed,
        animated: "medium",
        control: "#sidetreecontrol",
        persist: "location"
    });
};

cwBehaviours.cwTreeView.removeParent = function (container) {
    jQuery(container).find('.cw-children').each(function (i, child) {
        var c = child;
        // get 
        var childrenOfChild = jQuery(child).children();
        childrenOfChild.each(function (i, c1) {
            jQuery(c1).detach();
            jQuery(child).parent().append(jQuery(c1));
        });
        jQuery(child).remove();
    });
}