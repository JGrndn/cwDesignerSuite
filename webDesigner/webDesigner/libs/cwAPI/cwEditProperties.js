/*global cwAPI:true, cwConfigs :true */

cwAPI.cwEditProperties ={};

cwAPI.cwEditProperties.editProperties = function() {

  jQuery('.cw-edit-property-value').dblclick(function (e) {
    var itemID, v;
    if (jQuery(this).children('input').length) {
      return false;
    }
    itemID = jQuery(this).attr('id');
    v = jQuery(this).html();
    jQuery(this).html('');

    jQuery(this).append(jQuery('<input type="text" value="' + v + '"/>'));
    cwAPI.cwEditProperties.appendEditPropertyOptions(this, itemID);
  });
}

cwAPI.cwEditProperties.appendEditPropertyOptions = function(parent, pID) {
  var ul, liCancel, liSave;

  ul = jQuery('<ul/>').addClass('cw-edit-property-options');
  liCancel = jQuery('<li/>').addClass("ui-icon ui-icon-close cw-edit-cancel").attr('data-itemid', pID).click(function (e) {
    var itemID, v;
    itemID = jQuery(this).attr('data-itemid');
    v = jQuery('#' + itemID + ' input').val();
    jQuery('#' + itemID).html(v);
    return false;
  });
   liSave = jQuery('<li/>').addClass("ui-icon ui-icon-check cw-edit-save").attr('data-itemid', pID).click(function (e) {
    var itemID, v, item, objectTypeScriptName, objectID, jsonEditURL;
     itemID = jQuery(this).attr('data-itemid');
     v = jQuery('#' + itemID + ' input').val();

     item = jQuery('#' + itemID);
     objectTypeScriptName = item.attr('data-objecttype');
     objectID = item.attr('data-objectid');

    //console.log('save');
     jsonEditURL = cwConfigs.WEBDESIGNER_SERVER_URL + "Update/" + objectTypeScriptName + "/" + item.attr('data-propertyname') + "/" + objectID;
    jQuery.ajax({
      "url": jsonEditURL,
      "type": "POST",
      "dataType": "json",
      "data": {
        "scriptName": item.attr('data-propertyname'),
        "value": v
      },
      "cache": false,
      "success": function (res) {
        //console.log(res);
        if (res.status === "OK") {
          item.html(res.newValue);
          item.css('background-color', 'green').css('color', 'white');
        } else {
          item.css('background-color', 'red').css('color', 'white');
        }
      }
    });
    return false;
  });

  ul.append(liSave);
  ul.append(liCancel);
  //o.push('<li class="ui-icon ui-icon-check cw-edit-save" data-itemid="', pID, '"></li>')
  jQuery(parent).append(ul);
}
