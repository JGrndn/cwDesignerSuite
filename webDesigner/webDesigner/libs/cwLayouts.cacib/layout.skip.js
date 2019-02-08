/*global cwAPI:true */

var LayoutSkip = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutSkip.drawOne;
  };
LayoutSkip.prototype.applyCSS = function() {
  jQuery("ul." + this.css + "-details").css('margin', "0px").css('padding', '0px');
  jQuery("li." + this.css + "-details").css('list-style', 'none');
  jQuery("li." + this.css + "-title").addClass('ui-widget-header');
};

LayoutSkip.drawOne = function(output, item, callback) {
  if(!_.isUndefined(callback)) {
    callback(output, item);
  }
};

// next layout should be LayoutList. If not, display can be surprising
// more that one child node should be avoided
LayoutSkip.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }

  output.push("<div class='property-box ", this.css, "-box property-box-asso'>");
  output.push("<ul class='property-details ", this.css, "-details ", this.css, "-", _object.object_id, "-details'>");
  output.push("<li class='property-details ", this.css, "-details property-title ", this.css, "-title ", this.css, "-", _object.object_id, "-details'>");
  output.push(_associationTitleText);
  output.push("</li>");
  output.push("<li class='property-details property-value ", this.css, "-details ", this.css, "-value ", this.css, "-", _object.object_id, "-details'>");
  /* for each item*/

  _.each(_object.associations[_associationKey], function(_child) {
    this.drawOneMethod(output, _child, callback);
  }.bind(this));

  output.push("</li>");
  output.push("</ul></div>");

};