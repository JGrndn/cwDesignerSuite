/*global cwAPI:true, LayoutList:true */


var LayoutListBoxGridview = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListBoxGridview.drawOne;
};
LayoutListBoxGridview.prototype.applyCSS = function() {
  jQuery("ul." + this.css + "-details").css('margin', "0px").css('padding', '0px');
  jQuery("li." + this.css + "-details").css('list-style', 'none');
  jQuery("li." + this.css + "-title").addClass('ui-widget-header');
};
 
LayoutListBoxGridview.drawOne = function(output, item, callback, gridproperties) {
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";

  if (!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
    if (titleOnMouseOver != "") {
      titleOnMouseOver = " (" + titleOnMouseOver + ")";
    }
  }

  if (this.setLink === false) {
    itemDisplayName = "<span class='text " + this.css + "'>" + item.name + "</span><span class='sub-text'>" + titleOnMouseOver + "</span>";
  } else {
    link = cwAPI.createLinkForSingleView(this.targetView, item);
    itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a><span class='sub-text'></span>"; //" + titleOnMouseOver + "
  }

  output.push("<tr>");
  _.each(gridproperties, function(_prop) {
    var propertyValue;
    if (_prop.propertyScriptName === "name") {
      propertyValue = itemDisplayName;
    } else {
      propertyValue = item.properties[_prop.propertyScriptName];
    }

    output.push("<td class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class=' ", this.css, "'>", propertyValue, "</div>");
    if (!_.isUndefined(callback)) {
      callback(output, item);
    }
    output.push("</td>");
  });
  output.push("</tr>");
};

LayoutListBoxGridview.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {

  var gridproperties;
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }

  _.each(_object.associations[_associationKey][0].propertiesGroups, function(groupe) {
    gridproperties = groupe.properties;
  });

  if (_object.associations[_associationKey].length > 0) {
    output.push("<div class='property-box ", this.css, "-box property-box-asso'>");
    output.push("<ul class='property-details ", this.css, "-details ", this.css, "-", _object.object_id, "-details'>");
    output.push("<li class='property-details ", this.css, "-details property-title ", this.css, "-title ", this.css, "-", _object.object_id, "-details'>");
    output.push(_associationTitleText);
    output.push("</li>");
    output.push("<li class='property-details property-value ", this.css, "-details ", this.css, "-value ", this.css, "-", _object.object_id, "-details'>");

    output.push("<table class='gridview-table ui-corner-bottom ui-widget  ", this.css, " ", this.css, "-", _object.object_id, "'>");
    _.each(gridproperties, function(_property) {
      output.push("<th class='ui-state-default'>", _property.name, "</th>");
    });
    _.each(_object.associations[_associationKey], function(_child) {

      this.drawOneMethod(output, _child, callback, gridproperties);
    }.bind(this));
    output.push("</table>");
    output.push("</li>");
    output.push("</ul></div>");
  }
};