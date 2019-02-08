var LayoutListBoxOrg = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListBoxOrg.drawOne;
  LayoutListBoxOrg.applyCSS = LayoutListBox.applyCSS;
  LayoutListBoxOrg.drawAssociations = LayoutListBox.drawAssociations;
};

LayoutListBoxOrg.drawOne = function(output, item, callback) {
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";
  if (!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
  }

  link = cwAPI.createLinkForSingleView(this.targetView, item);
  itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + " (" + titleOnMouseOver + ")</a>";

  if (cwConfigs.RUN_IN_PORTAL) {
    itemDisplayName = cwAPI.createPortalLink(this.css, item);
  }

  if (this.setLink === false) {
    itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
  }

  output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>", itemDisplayName, "</div>");
  if (!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutListBoxOrg.prototype.applyCSS = function() {
  jQuery("ul." + this.css + "-details").css('margin', "0px").css('padding', '0px');
  jQuery("li." + this.css + "-details").css('list-style', 'none');
  jQuery("li." + this.css + "-title").addClass('ui-widget-header'); //.css('padding-left', '5px');
  //jQuery("li." + this.css + "-value").addClass('ui-widget-content');//.css('padding', '5px');
  //jQuery("li." + this.css + "-title").css('border', "2px solid blue");
};

LayoutListBoxOrg.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  cwAPI.drawListBox(LayoutList, this, output, _associationTitleText, _object, _associationKey, _associationTitleText, callback);
};

// if (_.isUndefined(cwAPI)) {
//   var cwAPI = {};
// }