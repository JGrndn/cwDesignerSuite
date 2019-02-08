/*global cwAPI:true, WorldMap :true, cwBehaviours: true */

cwBehaviours.cwWorldMap = {};

cwBehaviours.cwWorldMap.drawJomComWorldMap = function(canvasID, zoomList) {
  var oSettings = {};
  oSettings.id = canvasID;
  oSettings.bgcolor = "#FFFFFF";
  oSettings.fgcolor = "#FFFFFF";
  oSettings.fillColor = "#A2D0EA";
  oSettings.bordercolor = "#D1E8F5";
  oSettings.borderwidth = "1";
  oSettings.padding = "10";
  oSettings.zoom = zoomList;
  cwBehaviours.cwWorldMap.WorldMap(oSettings);
};

cwBehaviours.cwWorldMap.countriesToMapFromStringCountries = function(itemKey, canvasID, countriesString) {
  if (countriesString.length) {
    var c = jQuery('<canvas class="cw-world-map" id="' + canvasID + '" data-itemkey="' + itemKey + '" data-countries="' + countriesString + '"></canvas>');
     if (cwAPI.isUnderIE9()) {
       G_vmlCanvasManager.initElement(el);
     }
    jQuery("ul." + itemKey).after(c);
    jQuery('ul.' + itemKey).css('width', '20%').css('margin-left', '0px').css('margin-right', '0px').css('display', 'inline-block').css('vertical-align', 'top');
    jQuery("#" + canvasID).css("width", "75%").css("height", '400px').css('display', 'inline-block');
    cwBehaviours.cwWorldMap.drawJomComWorldMap(canvasID, countriesString);
  }
};

cwBehaviours.cwWorldMap.countriesToMapFromItems = function(itemKey, canvasID, items) {
  var countries, countryISO;
  countries = [];
  _.each(items, function(childLoc) {
    if (childLoc.properties.isocode !== "") {
      countryISO = cwAPI.cwSearchEngine.removeSearchEngineZone(childLoc.properties.isocode.toLowerCase());
      //countryISO = childLoc.properties.isocode.toLowerCase();
      countries.push(countryISO);
    }
  });
  if (countries.length) {
    cwBehaviours.cwWorldMap.countriesToMapFromStringCountries(itemKey, canvasID, countries.join(','));
  }
};

cwBehaviours.cwWorldMap.loadWorldMap = function(selector) {
  var countries, id, itemKey;

  jQuery(selector).find('canvas.cw-world-map').each(function(i, canvasWorld) {
    itemKey = jQuery(canvasWorld).attr('data-itemkey');
    id = jQuery(canvasWorld).attr('id');
    countries = jQuery(canvasWorld).attr('data-countries');
    jQuery(canvasWorld).remove();
    cwBehaviours.cwWorldMap.countriesToMapFromStringCountries(itemKey, id, countries);
  });
};