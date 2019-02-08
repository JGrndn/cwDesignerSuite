/*global cwAPI:true, LayoutList:true */


var LayoutListBoxForDiagram = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutListBoxForDiagram.drawOne;
  };
LayoutListBoxForDiagram.prototype.applyCSS = function() {
  jQuery("ul." + this.css + "-details").css('margin', "0px").css('padding', '0px');
  jQuery("li." + this.css + "-details").css('list-style', 'none');
  jQuery("li." + this.css + "-title").addClass('ui-widget-header');
};

LayoutListBoxForDiagram.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  cwAPI.drawListBox(LayoutList, this, output, _associationTitleText, _object, _associationKey, _associationTitleText, callback);
};

if(_.isUndefined(cwAPI)) {
  var cwAPI = {};
}
cwAPI.drawListBox = function(NextLayout, layout, output, _associationTitleText, _object, _associationKey, listBoxName, callback) {
  var l, targetObject;
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  targetObject = _object.associations[_associationKey];

  if(targetObject.length > 0) {
    output.push("<div class='property-box ", layout.css, "-box property-box-asso'>");
    output.push("<ul class='property-details ", layout.css, "-details ", layout.css, "-", _object.object_id, "-details'>");
    output.push("<li class='property-details ", layout.css, "-details property-title ", layout.css, "-title ", layout.css, "-", _object.object_id, "-details'>");
    output.push(listBoxName);
    output.push("</li>");
    output.push("<li class='property-details property-value ", layout.css, "-details ", layout.css, "-value ", layout.css, "-", _object.object_id, "-details'>");
    l = new NextLayout(layout.css, this.objectTypeName, layout.setLink, layout.parentID, layout.targetView);
    l.drawOneMethod = layout.drawOneMethod;
    l.drawAssociations(output, _associationTitleText, _object, _associationKey, callback);
    output.push("</li>");
    output.push("</ul></div>");
  }
};

LayoutListBoxForDiagram.drawOne = function(output, item, callback) {
  var itemDisplayName, titleOnMouseOver, link,type;
  titleOnMouseOver = "";
  if(!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
  }
  
  link = cwAPI.createLinkForSingleView(this.targetView, item);

if(!_.isUndefined(item.properties.type)){
    type = "<span class='diagram-type'> "+item.properties.type+" - </span>";
}
  itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" +item.name + "</a>";


  if(this.setLink === false) {
    itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
  }

  output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>",type, itemDisplayName, "</div>");
  if(!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};
