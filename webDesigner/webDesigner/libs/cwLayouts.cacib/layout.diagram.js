/*global cwAPI:true, getDiagram:true*/

var LayoutDiagram = function (_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  cwAPI.appliedLayouts.push(this);
  this.setLink = setLink;
  this.parentID = parentID;
  this.targetView = targetView;
  this.diagrams = [];
};

LayoutDiagram.prototype.applyCSS = function () {
  jQuery('ul.' + this.css).css('margin', '0px').css('padding', '0px');
  jQuery('li.' + this.css).css('margin', '0px').css('padding', '0px').css('list-style', 'none');

  var h = jQuery('div.cw-diagram-zone-' + this.css).height();
  jQuery('div.cw-diagram-zone-' + this.css + " canvas").css("height", (h - 18) + "px");
  _.each(this.diagrams, function (d) {
    var diagramSelectorID = "cw-diagram-" + this.css + "-" + d.object_id;
    jQuery('#' + diagramSelectorID).prev().hover(function (e) {
      jQuery(this).css('cursor', 'pointer');
    });

    // open by default
    cwAPI.getDiagram(d.object_id, diagramSelectorID, function () {});
    // toggle on click    
/*    jQuery('#' + diagramSelectorID).prev().click(function (e) {
      var htmlContent = jQuery('#' + diagramSelectorID).html();
      if (htmlContent === "") {
        jQuery('#' + diagramSelectorID).show('fast', function () {
          //jQuery('#' + diagramSelectorID).css('min-height', '400px');
          jQuery('#' + diagramSelectorID).html('');
          cwAPI.getDiagram(d.object_id, diagramSelectorID, function () {});
        });
      } else {
        jQuery('#' + diagramSelectorID).hide('fast');
        jQuery('#' + diagramSelectorID).html('');
      }
    });*/
  }.bind(this));
};



LayoutDiagram.prototype.drawAssociations = function (output, _associationTitleText, _object, _associationKey, callback) {
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if (_object.associations[_associationKey].length > 0) {
    output.push("<ul class='association-link-box association-link-box-", this.css, " ", this.css, "'>");
    _.each(_object.associations[_associationKey], function (_child) {
      output.push("<li class='" + this.css + "'>");
      //output.push("<div class='",this.css, "'><a class='diagram-toggle-link'>", _child.name, "</a></div>");
      output.push(cwAPI.cwDiagramManager.getDiagramDivString(_child.object_id, this.css));
      this.diagrams.push(_child);
      if (!_.isUndefined(callback)) {
        callback(output, _child, _object);
      }
    }.bind(this));
    output.push('</li>');
    output.push("</ul>");
  }
};
