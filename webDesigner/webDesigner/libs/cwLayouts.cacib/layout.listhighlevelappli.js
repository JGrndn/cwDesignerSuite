var LayoutListHighLevelAppli = function(_css, _objectTypeName, setLink, parentID, targetView) {
  this.css = _css;
  this.objectTypeName = _objectTypeName;
  this.setLink = setLink;
  this.targetView = targetView;
  this.parentID = parentID;
  cwAPI.appliedLayouts.push(this);
  this.drawOneMethod = LayoutListHighLevelAppli.drawOne;
};
LayoutListHighLevelAppli.prototype.applyCSS = function() {};

LayoutListHighLevelAppli.drawOne = function(output, item, callback, hasDiagram) {
  var itemDisplayName, titleOnMouseOver, link, diagramIcon, applicationClass, applicationStatus, otherName;
  titleOnMouseOver = "";
  diagramIcon = "";
  otherName = "";
  if (!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
  }
  if (hasDiagram === true) {
    diagramIcon = "  <span class='highlevelapplication sprite-hasdiagram'></span>";
  }
  //var AppliDisplay = this.setApplicationDisplayName(item);

  applicationClass = item.properties.type.replace(/\s/g, "").toLowerCase();
  applicationClass = "<span class='highlevelapplication sprite-" + applicationClass + "'></span>";

  applicationStatus = item.properties.status.replace(/\s/g, "").toLowerCase();
  switch (applicationStatus) {
    case 'active':
    case 'indevelopment':
    case 'businessstopped':
      break;
    default:
      // the icon is named sprite-status-other.png 
      applicationStatus = 'other';
      break;
  }

  applicationStatus = "<span class='highlevelapplication sprite-status-" + applicationStatus + "'></span>";

  link = cwAPI.createLinkForSingleView(this.targetView, item);
  if (item.properties.hasOwnProperty('othername')){
    otherName = "<span style='display:none' class='othername-application'>[" + item.properties.othername + "]</span>";
  }  

  itemDisplayName = applicationClass + "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a> (" + item.properties.applicationname + ")" + applicationStatus + diagramIcon + " " + otherName;

  if (this.setLink === false) {
    itemDisplayName = "<span class='" + this.css + "'>" + item.name + "</span>";
  }

  output.push("<li class=' ", this.css, " ", this.css, "-", item.object_id, "'><div class='", this.css, "'>", itemDisplayName, "</div>");
  if (!_.isUndefined(callback)) {
    //callback(output, item);
  }
  output.push("</li>");
};

LayoutListHighLevelAppli.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  var that = this;
  if (_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if (_object.associations[_associationKey].length > 0) {
    var toRemove = 0;
    // data-sum='",_object.associations[_associationKey].length,"' 
    output.push("<ul class='application hide ", that.css, " ", that.css, "-", _object.object_id, "'>");
    _.each(_object.associations[_associationKey], function(_child) {
      var empty = true;
      var hasDiagram = false;

      $.each(_child.associations, function(key, values) {
        // if the associated object is not a diagram, in other words, at least in this case, it's the father application
        if (key != 'diagram') {
          //if it has any father, it should not be displayed in the application list
          if (values.length > 0) {
            empty = false;
            return false;
          }
        } else {
          //if the associated object is a detailed diagram, mark it!
          if (values.length > 0) {
            hasDiagram = true;
          }
        }
      });
      if (empty) {
        that.drawOneMethod(output, _child, callback, hasDiagram);
      } else {
        toRemove = toRemove + 1;
      }
    });
    output.push("</ul>");
    output.push("<span id='data_sum' data-sum='", _object.associations[_associationKey].length - toRemove, "'></span>");
  }
};