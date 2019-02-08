/*global cwAPI :true, cwConfigs: true */

cwAPI.cwPropertiesGroups = {};

cwAPI.cwPropertiesGroups.displayPropertiesGroupFromKey = function(output, object, key) {
  if (!_.isUndefined(object.propertiesGroups[key])) {
    cwAPI.cwPropertiesGroups.displayPropertiesGroup(output, object, object.propertiesGroups[key]);
  }
};

cwAPI.cwPropertiesGroups.displayPropertiesGroup = function(output, mainObject, jsonPropertiesGroup) {
  switch (jsonPropertiesGroup.layout) {
    case 'Property Box':
      cwAPI.cwPropertiesGroups.createPropertyBox(output, mainObject, jsonPropertiesGroup);
      break;
    case 'memotext':
      cwAPI.cwPropertiesGroups.createMemoTextZone(output, mainObject, jsonPropertiesGroup);
      break;
    case "Property Box(hyperlink)":
      cwAPI.cwPropertiesGroups.createPropertyBoxHyperlink(output, mainObject, jsonPropertiesGroup);
      break;
    case 'list':
      break;
    case 'table':
      cwAPI.cwPropertiesGroups.createTable(output, mainObject, jsonPropertiesGroup);
      break;
  }
};

//propertybox
cwAPI.cwPropertiesGroups.createPropertyBox = function(output, mainObject, jsonPropertiesGroup) {
  var divId = jsonPropertiesGroup.name.replace(/[^A-Za-z0-9]/g, "").toLowerCase();
  output.push('<div id="', divId, '" class="cw-propertiesTable cw-propertybox"><div class="cw-propertiesTable-header ui-widget-header cw-propertybox-header">', jsonPropertiesGroup.name, '</div>');
  _.each(jsonPropertiesGroup.properties, function(property) {
    //console.log(mainObject, property);
    output.push('<div class="cw-propertybox-content ui-widget-content">', mainObject.properties[property.propertyScriptName], '</div>');
    //cwAPI.cwPropertiesGroups.putPropertiesInTable(output, property.propertyScriptName, property.name, mainObject, property.propertyType);
  });
  output.push('</div>');
};

//propertybox hyperlink
cwAPI.cwPropertiesGroups.createPropertyBoxHyperlink = function(output, mainObject, jsonPropertiesGroup) {
  output.push('<div class="cw-propertiesTable cw-propertiesGroup-memotext"><div class="cw-propertiesTable-header ui-widget-header cw-propertiesGroup-memotext-header">', jsonPropertiesGroup.name, '</div>');
  _.each(jsonPropertiesGroup.properties, function(property) {
    var docLink = mainObject.properties[property.propertyScriptName];
    var link = '<a href="' + docLink + '">' + docLink + '</a>';
    output.push('<div class="cw-propertiesGroup-memotext-content ui-widget-content">', link, '</div>');
  });
  output.push('</div>');
};

cwAPI.cwPropertiesGroups.createMemoTextZone = function(output, mainObject, jsonPropertiesGroup) {
  output.push('<div class="cw-propertiesTable cw-propertiesGroup-memotext"><div class="cw-propertiesTable-header ui-widget-header cw-propertiesGroup-memotext-header">', jsonPropertiesGroup.name, '</div>');
  _.each(jsonPropertiesGroup.properties, function(property) {
    //console.log(mainObject, property);
    output.push('<div class="cw-propertiesGroup-memotext-content ui-widget-content">', mainObject.properties[property.propertyScriptName], '</div>');
    //cwAPI.cwPropertiesGroups.putPropertiesInTable(output, property.propertyScriptName, property.name, mainObject, property.propertyType);
  });
  output.push('</div>');
};


cwAPI.cwPropertiesGroups.createTable = function(output, mainObject, jsonPropertiesGroup) {
  output.push('<div class="cw-propertiesTable"><div class="cw-propertiesTable-header ui-widget-header">', jsonPropertiesGroup.name, '</div><table class="cw-propertiesTable">');
  _.each(jsonPropertiesGroup.properties, function(property) {
    cwAPI.cwPropertiesGroups.putPropertiesInTable(output, property.propertyScriptName, property.name, mainObject, property.propertyType);
  });
  output.push('</table></div>');
};


cwAPI.cwPropertiesGroups.types = {};

cwAPI.cwPropertiesGroups.types.imageValue = function(value) {
  if (value !== "") {
    value = "<img src='" + cwConfigs.SITE_MEDIA_PATH + 'images/logos/64/' + value + ".png' alt='" + value + "'/>";
  }
  return value;
};

cwAPI.cwPropertiesGroups.types.booleanValue = function(value) {
  if (value !== "0") {
    value = $.i18n.prop("global_yes");
  } else {
    value = $.i18n.prop("global_no");
  }
  return value;
};

cwAPI.cwPropertiesGroups.types.dateValue = function(value) {
  value = value.substring(0, 10);
  if (value === "30/12/1899") {
    value = "";
  } else {
    value = formattedDate(value);
    value = dateFormat(value, "mmmm dS, yyyy");
  }
  return value;
};

var formattedDate = function(value) {
  var splitedDate = value.split("/");
  var date = new Date(splitedDate[2], parseInt(splitedDate[1]) - 1, splitedDate[0]);
  return date;
};

cwAPI.cwPropertiesGroups.putPropertiesInTable = function(output, pName, displayName, object, type) {
  var pID, value;
  value = object.properties[pName];

  // console.log(type);
  pID = object.objectTypeScriptName + object.object_id + pName;
  if (!_.isUndefined(value) && !_.isUndefined(type)) {
    switch (type) {
      case "date":
        value = cwAPI.cwPropertiesGroups.types.dateValue(value);
        break;
      case "image":
        value = cwAPI.cwPropertiesGroups.types.imageValue(value);
        break;
      case "boolean":
        value = cwAPI.cwPropertiesGroups.types.booleanValue(value);
        break;
    }
  }
  output.push('<tr><th>', displayName, '</th><td class="cw-edit-property-value" data-objectid="', object.object_id, '" id="', pID, '" data-objecttype="', object.objectTypeScriptName, '" data-propertyname="', pName, '">', value, '</td></tr>');
};