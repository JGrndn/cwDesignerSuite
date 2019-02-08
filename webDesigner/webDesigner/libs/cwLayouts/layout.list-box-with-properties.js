/*global cwAPI:true */

var LayoutListboxWithProperties = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutListboxWithProperties.drawOne;
  };
LayoutListboxWithProperties.prototype.applyCSS = function() {};

LayoutListboxWithProperties.drawOne = function(output, item, callback) {
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";
  if(!_.isUndefined(item.properties.description)) {

    titleOnMouseOver = item.properties.description;
    //titleOnMouseOver = titleOnMouseOver.replace(/[^A-Za-z0-9]/g, ' ');
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
  }


  link = cwAPI.createLinkForSingleView(this.targetView, item);
  itemDisplayName = "<a class='" + this.css + "' href='" + link + "' title='" + titleOnMouseOver + "'>" + cwAPI.shortText(item.name, 88) + "</a>";

  if(this.setLink === false) {
    itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
  }
  //  output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>", itemDisplayName, "</div>");
  // if (!_.isUndefined(callback)) {
  //   callback(output, item);
  // }
  // output.push("</li>");
  //draw one method
  //head
  output.push('<div class="cw-listbox-with-properties"><div class="cw-listbox-with-properties-header ui-widget-header"><span class="cw-ui-icon cw-ui-icon-', this.css, '"></span>', itemDisplayName, '</div>');
  output.push('<div class="cw-listbox-with-properties-content ', this.css, '">');
  //console.log(item.properties);
  jQuery.each(item.properties, function(index, property) {

    if(index != "name" && index != "type" && index != "type_abbreviation" && index != "type_id") {
      output.push('<span class="', index, '">', property, '</span><br/>');
    }
  });
  output.push('</div>');
  output.push("</div>");

  if(!_.isUndefined(callback)) {
    callback(output, item);
  }
};

LayoutListboxWithProperties.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if(_object.associations[_associationKey].length > 0) {
    _.each(_object.associations[_associationKey], function(_child) {
      this.drawOneMethod(output, _child, callback);
    }.bind(this));
  }
};