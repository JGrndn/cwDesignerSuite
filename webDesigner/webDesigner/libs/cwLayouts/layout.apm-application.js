/*global cwAPI:true, cwConfigs : true */

var LayoutAPMApplication = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutAPMApplication.drawOne;
  };
LayoutAPMApplication.prototype.applyCSS = function() {};

LayoutAPMApplication.drawOne = function(output, item, callback) {


  var tooltipApp, locations, link, appDescription, appType, appName, itemDisplayName;
  tooltipApp = [];
  link = cwAPI.createLinkForSingleView(this.targetView, item);
  appDescription = cwAPI.cwSearchEngine.removeSearchEngineZone(item.properties.description).replace(/'/gi, '');
  appType = cwAPI.cwSearchEngine.removeSearchEngineZone(item.properties.type);
  appName = cwAPI.cwSearchEngine.removeSearchEngineZone(item.properties.name);
  tooltipApp.push(appName);
  if (appType !== "") {
    tooltipApp.push('<b> - ', appType, '</b>');
  }
  //tooltipApp.push("<div class=\"apm-application-tooltip-description\">", appDescription, "</div>");
  output.push('<li class=\'apm-application ui-corner-all\'>');

  LayoutAPMApplicationAPI.saveChartValue(output, item, tooltipApp);

  itemDisplayName = '<a data-item-id=\'' + item.object_id + '\' class=\'cw-tooltip-me-custom-apm\' href=\'' + link + '\' title=\'' + tooltipApp.join('') + '\'>' + item.name + '</a>';

  if (cwConfigs.RUN_IN_PORTAL) {
    itemDisplayName = cwAPI.createPortalLink("cw-tooltip-me-custom", item, tooltipApp.join(''));
  }


  output.push('<div class=\'apm-application\'>', itemDisplayName, '</div>');



  locations = item.associations.location_20055;
  if (!_.isUndefined(locations) && locations.length > 0) {
    output.push('<ul class=\'apm-location-flag\'>');
    _.each(locations, function(location) {
      if (location.properties.isocode === "") {
        return;
      }
      output.push('<li class=\'apm-location-flag\'><img class=\'cw-tooltip-me\' title=\'', location.name, '\' src=\'', cwConfigs.SITE_MEDIA_PATH, '/images/flags/', location.properties.isocode.toLowerCase(), '.png\'/></li>');
    }.bind(this));
    output.push('</ul>');
  }
  output.push('</li>');
};

LayoutAPMApplication.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  if (_.isUndefined(_object.associations[_associationKey])) {
    //console.log('FAIL',_object, _associationKey);
    return;
  }

  if (_object.associations[_associationKey].length > 0) {
    output.push("<ul class='apm-application'>");
    _.each(_object.associations[_associationKey], function(_child) {
      this.drawOneMethod(output, _child, callback);
    }.bind(this));
    output.push('</ul>');
  }
};

var LayoutAPMApplicationAPI = {};

LayoutAPMApplicationAPI.saveChartValue = function(o, application, tooltipApp) {

  var lTotal = parseInt(application.properties.numberoflicenses);
  var lUsed = parseInt(application.properties.licensesused);
  var lFree = parseInt(lTotal - application.properties.licensesused);
  //lUsed = (lUsed / lTotal) * 100;
  //lFree = (lFree / lTotal) * 100;
  //debugger;
  var pieData = [];
  pieData.push({
    "category": "Used Licenses",
    "value": lUsed
  });
  pieData.push({
    "category": "Free Licenses",
    "value": lFree
  });
  o.push("<span id='apm-application-chart-container-", application.object_id, "' data-values='", JSON.stringify(pieData), "' class='hidden'>", tooltipApp.join(''), '</span>');
}


LayoutAPMApplicationAPI.setAPMTooltips = function() {
  jQuery('.cw-tooltip-me-custom-apm').tooltip({
    "bodyHandler": function() {
      var id = jQuery(this).attr('data-item-id');

      var tooltipContainer = jQuery('#apm-application-chart-container-' + id);
      var valuesString = tooltipContainer.attr('data-values');

      var values = JSON.parse(valuesString);
      var container = jQuery('<div/>').attr('id', 'chart-', id, '">');

      if (values[0].value === 0 && values[1].value === 0) {
        return tooltipContainer.html();
      }
      setTimeout(function() {
        container.kendoChart({
          theme: "blueopal",
          legend: {
            position: "bottom",
            labels: {              
               template: "#= text # (#= value #)"
            }
          },
          seriesDefaults: {
            labels: {
              visible: true,
              template: "#= category # - #= kendo.format('{0:P}', percentage)#"
            }
          },
          chartArea: {
            width: 600,
            height: 250
          },
          animation: false,
          series: [{
            type: "pie",
            data: values
          }]
        });
      }, 100);
      var tooltipContent = jQuery('<div/>').append(tooltipContainer.html()).append(container);
      return tooltipContent;
    },
    "showURL": false
  });
}