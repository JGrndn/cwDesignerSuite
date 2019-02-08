/*global cwAPI:true */
var LayoutListByPropertyType = function(_css, _objectTypeName, setLink, parentID, targetView) {
    this.css = _css;
    this.objectTypeName = _objectTypeName;
    this.setLink = setLink;
    this.targetView = targetView;
    this.parentID = parentID;
    cwAPI.appliedLayouts.push(this);
    this.drawOneMethod = LayoutListByPropertyType.drawOne;
    this.propertyAsIndex = "definitioncontext";
    this.propertyAsIndexId = "definitioncontext_id";
  };

LayoutListByPropertyType.setPropertyInfo = function(){

}
  LayoutListByPropertyType.drawOne = function(output, item, callback) {
  
  var itemDisplayName, titleOnMouseOver, link;
  titleOnMouseOver = "";

  if(!_.isUndefined(item.properties.description)) {
    titleOnMouseOver = item.properties.description;
    titleOnMouseOver = titleOnMouseOver.replace(/\'/g, ' ');
    if(titleOnMouseOver!=""){
    titleOnMouseOver = " ("+titleOnMouseOver+")";
    }
  }


  if(this.setLink === false) {
    itemDisplayName = "<span class='tooltip-me processlevel0 text " + this.css + "'>" + item.name +"</span>" ;
  }
  else{
    link = cwAPI.createLinkForSingleView(this.targetView, item);
    itemDisplayName = "<a class='" + this.css + " tooltip-me' href='" + link + "' title='" + titleOnMouseOver + "'>" + item.name + "</a>";
  }

  var arrawDown ="<span class='arrow down'></span>";
  output.push("<li class=' level0 ", this.css, " ", this.css, "-", item.object_id, "'><div class='level0 ", this.css, "'>", itemDisplayName, "<span class='object_type_title'>",this.objectTypeName,"</span></div>");
  if(!_.isUndefined(callback)) {
    callback(output, item);
  }
  output.push("</li>");
};

LayoutListByPropertyType.prototype.applyCSS = function() {
   $("div.level0").click(function(){
      if ($(this).next().is(':hidden')) {
        showSiblings(this);
       }
      else {  
        hideSiblings(this);
        }
    });
    $("span#expand-all").click(function(){
      if($(this).text()=="Expand All"){
        $(this).text("Collapse All");
        showSiblings("div.level0");
        }
      else{
        $(this).text("Expand All");
        hideSiblings("div.level0");
      }     
    });
};

LayoutListByPropertyType.prototype.transform = function(all_items) {
  var itemListByPT = {};

  var propertyAsIndex = "definitioncontext";
  jQuery.each(all_items, function(i, item) {
    var key;
    if(!_.isUndefined(item.properties[indexPropertyType.id])) {
      var propertyId = item.properties[indexPropertyType.id];
      if(propertyId != 0) {
       // console.log(indexPropertyType.id);
        //console.log(item.properties['definitioncontext_id']);
        key = item.properties[indexPropertyType.name];
      } else {
        key = "Undefined";
      }
    } 
    if(_.isUndefined(itemListByPT[key])) {
      itemListByPT[key] = [];
    }
    itemListByPT[key].push(item);
  });
  return itemListByPT;
};

LayoutListByPropertyType.prototype.drawAssociations = function(output, _associationTitleText, _object, _associationKey, callback) {
  
  output.push('<span id="expand-all" class="text">Collapse All</span>');
  var that = this;
  if(_.isUndefined(_object.associations[_associationKey])) {
    return;
  }
  if(_object.associations[_associationKey].length > 0) {
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