
cwAPI.cwSiteActions = {};

cwAPI.cwSiteActions.doActionsForSingle = function () {
  _.each(cwAPI.appliedLayouts, function (layout) {
    layout.applyCSS();
  });

  jQuery('ul.properties-zone-area').css('margin', '0px').css('padding', '0px');
  cwAPI.cwSiteActions.setupPortalLinks();
}


cwAPI.cwSiteActions.doLayoutsSpecialActions = function() {

  //console.log();
  _.each(cwAPI.appliedLayouts, function (layout) {
    layout.applyCSS();
  });

  jQuery('.navigation-icon').addClass('ui-icon ui-icon-info');
  jQuery('.navigation-icon').click(function () {
    document.location.href = jQuery(this).attr('href');
  });

  cwAPI.cwSiteActions.setupPortalLinks();
  cwCustomerSiteActions.doLayoutsSpecialActionsLocal();
}

