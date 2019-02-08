/*global cwAPI:true */

var AtoZLayoutList = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = AtoZLayoutList.drawOne;
  };
AtoZLayoutList.prototype.applyCSS = function() {};

AtoZLayoutList.drawOne = function(output, item, callback) {
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";
  if(!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
   // console.log(titleOnMouseOver);
  }
  
  //  console.log(this.css);
  link = cwAPI.createLinkForSingleView(this.targetView, item);
  itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a>";

  if(cwConfigs.RUN_IN_PORTAL) {
    itemDisplayName = cwAPI.createPortalLink(this.css, item);
  }

  if(this.setLink === false) {
    itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
  }

  output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>", itemDisplayName, "</div>");
  if(!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};


AtoZLayoutList.prototype.transform = function(all_items) {
  var itemListByAlphabet = {};

  $.each(all_items, function(i, item) {
    var key = item.properties["name"].substring(0,1).toUpperCase();
    if(_.isUndefined(itemListByAlphabet[key])) {
      itemListByAlphabet[key] = [];
    }
    itemListByAlphabet[key].push(item);
  });
  return itemListByAlphabet;
};


AtoZLayoutList.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  var that = this;
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if(_object.associations[_associationKey].length > 0) {
    var itemListByCategory = this.transform(_object.associations[_associationKey]);
    output.push('<ul>');
    $.each(itemListByCategory, function(type, listItems) {
      output.push('<li class="toggle"><div><a href="#">', type, '</a></div>');
      output.push("<div class='cw-children children-", that.css, "'>");
      output.push("<ul class='", that.css, " ", that.css, "-", _object.object_id, "'>");
      _.each(listItems, function(_child) {
        that.drawOneMethod(output, _child, callback);
      });
      output.push("</ul></li>");
      //output.push("");
    });
    output.push('</ul>');
  }
};