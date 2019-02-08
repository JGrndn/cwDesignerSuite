/*global cwAPI:true */
var LayoutListByCategory = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListByCategory.drawOne;
};

LayoutListByCategory.drawOne = function(output, item, callback) {

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
    itemDisplayName = "<span class='tooltip-me processlevel0 text " + this.css + "'>" + item.name + "</span>";
  } else {
    link = cwAPI.createLinkForSingleView(this.targetView, item);
    itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a>";
  }

  output.push("<li class=' level0 ", this.css, " ", this.css, "-", item.object_id, "'><div class='level0 ", this.css, "'><span class='arrow down'></span>", itemDisplayName, "<span class='object_type_title'>", this.objectTypeName, "</span></div>");
  if (!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutListByCategory.prototype.applyCSS = function() {
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

LayoutListByCategory.prototype.transform = function(all_items) {
  var itemListByCategory = {};

  jQuery.each(all_items, function(i, item) {
    var key;
    if (!_.isUndefined(item.properties.type_id)) {
      var type_id = item.properties.type_id;
      if (type_id != 0) {
        key = item.properties.type;
      } else {
        key = "Undefined";
      }
    } else if (!_.isUndefined(item.properties.internalexternal_id)) {
      var type_id = item.properties.internalexternal_id;
      if (type_id != 0) {
        key = item.properties.internalexternal;
      } else {
        key = "Undefined";
      }

    }

    if (_.isUndefined(itemListByCategory[key])) {
      itemListByCategory[key] = [];
    }
    itemListByCategory[key].push(item);
  });
  return itemListByCategory;
};

LayoutListByCategory.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {

  if ($("span#expand-all").length == 0) {

    output.push('<span id="expand-all" class="text">Expand All</span>');
  }
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