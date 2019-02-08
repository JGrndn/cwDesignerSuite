/*global cwAPI:true */
var LayoutListDefinition = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListDefinition.drawOne;
  this.propertyToDisplay = language;
};

LayoutListDefinition.drawOne = function(output, item, callback) {

  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";

  // if (!_.isUndefined(item.properties.name)) {
  //   titleOnMouseOver = item.properties.name;
  //   titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
  // }
  switch (this.propertyToDisplay) {
    case "name":
      titleOnMouseOver = item.properties["englishname"];
      break;
    case "englishname":
      titleOnMouseOver = item.properties.name;
      break;
  }
  titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');

  var name = this.propertyToDisplay;
  link = cwAPI.createLinkForSingleView(this.targetView, item);
  itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.properties[name] + "</a>";
  // }

  var arrawDown = "<span class='arrow down'></span>";
  output.push("<li class=' level0 ", this.css, " ", this.css, "-", item.object_id, "'><div class='level1 ", this.css, "'>", itemDisplayName, "<span class='object_type_title'></span></div>");
  if (!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutListDefinition.prototype.applyCSS = function() {
  // $("div.level0").click(function() {
  //   if ($(this).next().is(':hidden')) {
  //     showSiblings(this);
  //   } else {
  //     hideSiblings(this);
  //   }
  // });
  // $("span#expand-all").click(function() {
  //   if ($(this).text() == "Expand All") {
  //     $(this).text("Collapse All");
  //     showSiblings("div.level0");
  //   } else {
  //     $(this).text("Expand All");
  //     hideSiblings("div.level0");
  //   }
  // });
};


function dynamicSort(property) {
  var sortOrder = 1;
  if (property[0] === "-") {
    sortOrder = -1;
    property = property.substr(1);
  }
  return function(a, b) {
    var result = (a.properties[property] < b.properties[property]) ? -1 : (a.properties[property] > b.properties[property]) ? 1 : 0;
    return result * sortOrder;
  }
}

LayoutListDefinition.prototype.transform = function(all_items) {
  var itemListByPT = {};
  all_items.sort(dynamicSort(this.propertyToDisplay));
  jQuery.each(all_items, function(i, item) {
    var key;
    if (!_.isUndefined(item.properties[indexPropertyType.id])) {
      var propertyId = item.properties[indexPropertyType.id];
      if (propertyId != 0) {
        key = item.properties[indexPropertyType.name];
      } else {
        key = "Undefined";
      }
    }
    if (_.isUndefined(itemListByPT[key])) {
      itemListByPT[key] = [];
    }
    itemListByPT[key].push(item);
  });
  return itemListByPT;
};

LayoutListDefinition.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {

  output.push('<span id="expand-all" class="text">Expand All</span>');
  var that = this;
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if (_object.associations[_associationKey].length > 0) {
    var itemListByCategory = this.transform(_object.associations[_associationKey]);
    output.push('<ul class="level0">');
    jQuery.each(itemListByCategory, function(type, listItems) {
      output.push('<li class="level0"><div class="level0"><span class="arrow down"></span><span class="text">', type, '</span></div>');
      output.push("<div class='level1 cw-children children-", that.css, "'>");
      output.push("<ul class=' level0 ", that.css, " ", that.css, "-", _object.object_id, "'>");
      _.each(listItems, function(_child) {
        that.drawOneMethod(output, _child, callback);
      });
      output.push("</ul></li>");
    });
    output.push('</ul>');
  }
};