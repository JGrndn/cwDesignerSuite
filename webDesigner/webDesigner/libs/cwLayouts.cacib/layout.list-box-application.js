var LayoutBoxApplication = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListHighLevelAppli.drawOne;
};

LayoutBoxApplication.prototype.applyCSS = function() {
  jQuery("ul." + this.css + "-details").css('margin', "0px").css('padding', '0px');
  jQuery("li." + this.css + "-details").css('list-style', 'none');
  jQuery("li." + this.css + "-title").addClass('ui-widget-header');
};



LayoutBoxApplication.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  cwAPI.drawListBox(LayoutList, this, output, _associationTitleText, _object, _associationKey, _associationTitleText, callback);
};

if (_.isUndefined(cwAPI)) {
  var cwAPI = {};
}
// cwAPI.drawListBox = function(NextLayout, layout, output, _associationTitleText, _object, _associationKey, listBoxName, callback) {
//   var l, targetObject;
//   //console.log("_object", _object);
//   if (_.isUndefined(_object.associations[_associationKey])) {
//     //console.log('draw association _associationKey[', _associationKey, '] don\'t exists for ', _object);
//     return;
//   }
//   targetObject = _object.associations[_associationKey];

//   if (targetObject.length > 0) {
//     output.push("<div class='property-box ", layout.css, "-box property-box-asso'>");
//     output.push("<ul class='property-details ", layout.css, "-details ", layout.css, "-", _object.object_id, "-details'>");
//     output.push("<li class='property-details ", layout.css, "-details property-title ", layout.css, "-title ", layout.css, "-", _object.object_id, "-details'>");
//     output.push(listBoxName);
//     output.push("</li>");
//     output.push("<li class='property-details property-value ", layout.css, "-details ", layout.css, "-value ", layout.css, "-", _object.object_id, "-details'>");
//     l = new NextLayout(layout.css, this.objectTypeName, layout.setLink, layout.parentID, layout.targetView);
//     l.drawOneMethod = layout.drawOneMethod;
//     l.drawAssociations(output, _associationTitleText, _object, _associationKey, callback);
//     output.push("</li>");
//     output.push("</ul></div>");
//   }
// };
