var LayoutApplicationListByAlphabet = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  //the layout of each application is the same as the high level application
  this.drawOneMethod = LayoutListHighLevelAppli.drawOne;
};

LayoutApplicationListByAlphabet.prototype.applyCSS = function() {
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

LayoutApplicationListByAlphabet.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  output.push('<span id="expand-all" class="text">Expand All</span>');
  var that = this;
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if (_object.associations[_associationKey].length > 0) {
    var itemListByAlphabet = cwAPI.transformToABCorder(_object.associations[_associationKey]);
    output.push('<ul class="level0">');
    jQuery.each(itemListByAlphabet, function(type, listItems) {
      output.push('<li  class="level0"><div class="level0"><span class="arrow down"></span><span class="text">', type, '</span><span class="number"> [', listItems.length, '] </span></div>');
      output.push("<div class='cw-children children-", that.css, "'>");
      output.push("<ul class=' ", that.css, " ", that.css, "-", _object.object_id, "'>");
      _.each(listItems, function(_child) {
        var hasDiagram = false;
        //to check is the application has an exploded diagram
        if (!_.isUndefined(_child.associations['diagram'])) {
          if (_child.associations['diagram'].length > 0) {
            hasDiagram = true;
          }
        }
        that.drawOneMethod(output, _child, callback, hasDiagram);
      });
      output.push("</ul></li>");
    });
    output.push('</ul>');
  }
};