/*global cwAPI:true */
var LayoutListByCategory = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutList.drawOne;
  };
LayoutListByCategory.prototype.applyCSS = function() {
  jQuery('li.toggle > div > a').click(function(e) {
    jQuery(this).parent().next().toggle();
  });
};

LayoutListByCategory.prototype.transform = function(all_items) {
  var itemListByCategory = {};

  jQuery.each(all_items, function(i, item) {
    var key;
    if(!_.isUndefined(item.properties.type_id)) {
      var type_id = item.properties.type_id;
      if(type_id != 0) {
        key = item.properties.type;
      } else {
        key = "non défini(e)";
      }
    } else if(!_.isUndefined(item.properties.internalexternal_id)) {
      var type_id = item.properties.internalexternal_id;
      if(type_id != 0) {
        key = item.properties.internalexternal;
      } else {
        key = "non défini(e)";
      }

    }

    if(_.isUndefined(itemListByCategory[key])) {
      itemListByCategory[key] = [];
    }
    itemListByCategory[key].push(item);
  });
  return itemListByCategory;
};

LayoutListByCategory.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  var that = this;
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if(_object.associations[_associationKey].length > 0) {
    var itemListByCategory = this.transform(_object.associations[_associationKey]);
    output.push('<ul>');
    jQuery.each(itemListByCategory, function(type, listItems) {
      output.push('<li class="toggle"><div><a href="#">', type, '</a></div>');
      output.push("<div class='cw-children children-", that.css, "'>");
      output.push("<ul class='", that.css, " ", that.css, "-", _object.object_id, "'>");
      _.each(listItems, function(_child) {
        that.drawOneMethod(output, _child, callback);
      });
      output.push("</ul></li>");
      output.push("");
    });
    output.push('</ul>');
  }
};