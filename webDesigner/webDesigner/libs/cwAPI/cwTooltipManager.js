

cwAPI.setToolTipsOnTitles = function () {

  jQuery('.tooltip-me').tooltip({
    showURL: false,
    fade: 250
  });
  jQuery('.cw-tooltip-me').tooltip({
    showURL: false,
    fade: 250
  });
  jQuery('.tooltip-me-left').tooltip({
    showURL: false,
    fade: 250,
    extraClass: 'tooltip-left'
  });
  jQuery('.cw-tooltip-me-custom').tooltip({
    showURL: false,
    fade: 250,
    extraClass: 'cw-tooltip-custom'
  });
};