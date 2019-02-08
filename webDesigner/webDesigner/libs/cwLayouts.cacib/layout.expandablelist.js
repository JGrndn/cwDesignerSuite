var LayoutListExpandable = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListExpandable.drawOne;
};

LayoutListExpandable.prototype.applyCSS = function() {
  if ($("span#expand-all").length == 0) {
    $("div.cw-main-zone").children("div:first").prepend('<span id="expand-all" class="text">Expand All</span>');
    //output.push('<span id="expand-all" class="text">Expand All</span>');
  }
};

LayoutListExpandable.drawOne = function(output, item, callback) {
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
    itemDisplayName = "<span class='level0 text " + this.css + "'>" + item.name + "</span><span class='sub-text'>" + titleOnMouseOver + "</span><span class='number'></span>";
  } else {
    link = cwAPI.createLinkForSingleView(this.targetView, item);
    itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a><span class='sub-text'>" + titleOnMouseOver + "</span>";
  }

  output.push("<li class=' level0 ", this.css, " ", this.css, "-", item.object_id, "'><div class='level0 ", this.css, "'><span class='arrow down'></span>", itemDisplayName, "<span class='object_type_title'>", this.objectTypeName, "</span></div>");

  if (!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutListExpandable.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  var that = this;
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if (_object.associations[_associationKey].length > 0) {
    output.push("<ul class='level0 ", that.css, " ", that.css, "-", _object.object_id, "'>");
    _.each(_object.associations[_associationKey], function(_child) {
      that.drawOneMethod(output, _child, callback);
    });
    output.push("</ul>");
  }
  output.push("<div class='total'></div>");
};