/*global cwAPI:true */

var LayoutEmpty = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = null;
};
LayoutEmpty.prototype.applyCSS = function() {};

// LayoutEmpty.drawOne = function(output, item, callback) {
//   // var itemDisplayName, titleOnMouseOver, link;
//   // //console.log(this.parent, _object, _child);
//   // /*  if (!_.isUndefined(this.parent)) {
//   //   if (this.parent.link_id === item.link_id) {
//   //     return;
//   //   }
//   // }*/
//   // //console.log(item);
//   // titleOnMouseOver = "";
//   // if (!_.isUndefined(item.properties.description)) {
//   //   titleOnMouseOver = item.properties.description;
//   // }
//   // //  console.log(this.css);
//   // link = cwAPI.createLinkForSingleView(this.targetView, item);
//   // itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a>";

//   // if (cwConfigs.RUN_IN_PORTAL) {
//   //   itemDisplayName = cwAPI.createPortalLink(this.css, item);
//   // }

//   // if (this.setLink === false) {
//   //   itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
//   // }

//   // output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>", itemDisplayName, "</div>");
//   // if (!_.isUndefined(callback)) {
//   //   callback(output, item);
//   // }
//   // output.push("</li>");
// };

LayoutEmpty.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  var grandChildren = {};
  var tmp = {};

  if (_object.associations[_associationKey].length > 0) {
    _.each(_object.associations[_associationKey], function(_child) {
      jQuery.each(_child.associations, function(_childrenAssoKey, childrenAsso) {
        if (_.isUndefined(grandChildren[_childrenAssoKey])) {
          grandChildren[_childrenAssoKey] = [];
        }
        for (var i = 0; i < childrenAsso.length; i++) {
          var object_id = childrenAsso[i].object_id;
          //if the grand child does not exist in the list
          if (_.isUndefined(tmp[object_id])) {
            tmp[object_id] = object_id;
            grandChildren[_childrenAssoKey].push(childrenAsso[i]);
          } else {}
        }
      });
    });

    //recreate the new items
    _object.associations[_associationKey] = [];
    var newChild = {
      "associations": grandChildren
    };
    _object.associations[_associationKey].push(newChild);
  }

  if (_object.associations[_associationKey].length > 0) {
    _.each(_object.associations[_associationKey], function(_newchild) {
      callback(output, _newchild);
    }.bind(this));
  }
};