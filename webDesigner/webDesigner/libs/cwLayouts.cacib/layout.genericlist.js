/*global cwAPI:true */

var LayoutGenericList = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutGenericList.drawOne;
};
LayoutGenericList.prototype.applyCSS = function() {};

LayoutGenericList.drawOne = function(output, item, callback) {
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";
  if (!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
    // console.log(titleOnMouseOver);
  }

  //  console.log(this.css);
  link = cwAPI.createLinkForSingleView(this.targetView, item);
  itemDisplayName = "<a class='generic-list " + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a>";

  if (cwConfigs.RUN_IN_PORTAL) {
    itemDisplayName = cwAPI.createPortalLink(this.css, item);
  }

  if (this.setLink === false) {
    itemDisplayName = "<span class='generic-list " + this.css + "'>" + item.name + "</span>";
  }

  output.push("<li class='generic-list ", this.css, " ", this.css, "-", item.object_id, "'><div class='generic-list ", this.css, "'>", itemDisplayName, "</div>");
  if (!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutGenericList.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  //console.log("O", _object, _associationKey, _object.associations[_associationKey]);
  if (_.isUndefined(_object.associations[_associationKey])) {
    //console.log('draw association _associationKey[', _associationKey, '] don\'t exists for ', _object);
    return;
  }
  //console.log(_object.associations[_associationKey]);
  if (_object.associations[_associationKey].length > 0) {
    output.push("<ul class='generic-list ", this.css, " ", this.css, "-", _object.object_id, "'>");
    _.each(_object.associations[_associationKey], function(_child) {
      //console.log('chilmd', _child);    
      this.drawOneMethod(output, _child, callback);
    }.bind(this));
    output.push("</ul>");
  }
};