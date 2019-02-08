/*global cwAPI:true, cwConfigs:true */

cwAPI.cwSiteActions.setupPortalLinks = function() {
  if (cwConfigs.RUN_IN_PORTAL){
    jQuery('a.cw-portal-link').click(function(e){
      var ot = jQuery(this).attr('data-ot').toUpperCase();
      var id = jQuery(this).attr('data-id');
      jQuery('.cw-tooltip-custom').hide();
      jQuery('#cw-tooltip').hide();
      window.objectTypeView.setObject(cwConfigs.MODEL_FILENAME, ot, id);
    });
  }
}

cwAPI.createPortalLink = function(css, item, title){
	return "<a class='cw-portal-link " + css + "' data-ot='" + item.objectTypeScriptName + "' data-id='" + item.object_id + "' " + (!_.isUndefined(title) ? "title='" + title + "'" : "")+ ">" + item.name + "</a>";
}