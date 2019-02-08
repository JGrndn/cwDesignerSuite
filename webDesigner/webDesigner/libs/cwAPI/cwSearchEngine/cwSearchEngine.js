/*global jQuery: true, cwAPI:true*/
// V0.2.2
// 2010/08/30 
// Copyright Casewise 2010 
// 2010/08/30 : garde le sous ensemble 
// 2010/09/01 : ajout de la récursivité sur les recherches, ajout de l'attribut clean permattant de garder ou d'effacer le contexte de la recherche 
// 2010/09/18 : ajout du tooltip pour la recherche
cwAPI.cwSearchEngine = {};
cwAPI.cwSearchEngine.searchMemory = {};
cwAPI.cwSearchEngine.searchMemory.searchResultData = null;
cwAPI.cwSearchEngine.searchMemory.inputLength = 0;

cwAPI.cwSearchEngine.doCustomSearch = function (items) {
    if (location.hash.length != 0 && location.hash != "#q=") {
        var $b = jQuery("#cw_search_button");
        if ($b.length > 0) {
            var loaded = $b.attr('date-already-loaded');
            if (_.isUndefined(loaded)) {
                var q = location.hash;
                while (q.charAt(0) === "#") {
                    q = q.substring(3);
                }
                jQuery('.cw-searchengine-suggest').val(q);
                $b.attr('date-already-loaded', 1);
                jQuery("#cw_search_button").trigger("click");
            }
        }
        return true;
    }
    return false;
};

cwAPI.cwSearchEngine.removeSearchEngineZone = function(text) {
  if(_.isUndefined(text)) {
    return "";
  }
  var out = text.replace(/<span class='cw-searchengine-item-found'>/gi, '');
  out = out.replace(/<\/span>/, '');
  return out;
};

// add the search engine context to the page 
cwAPI.cwSearchEngine.appendSearchEngineInput = function(title, name, context, JSONItems, searchEngine, callback_context) {
  var searchButtonName = jQuery.i18n.prop("search_option_search");
  var resetButtonName = jQuery.i18n.prop("search_option_reset");
  context.append("<div id='cw-searchengine-search-area_" + name + "' class='cw-searchengine-search-area'><span class='cw-searchengine-text-zone'>" + title + "</span><input type='text' id='cw-searchengine-input-suggest-" + name + "' class='cw-searchengine-suggest'/><button type='button' id='cw_search_button'>" + searchButtonName + "</button> <button type='button' id='cw_reset_button'>" + resetButtonName + "</button> </div>");
  if(searchEngine.instantSearch == true) {
    jQuery("#cw_search_button").hide();
    jQuery("#cw_reset_button").hide();
    cwAPI.cwSearchEngine.prepareInstantSearch(name, {
      "associations": JSONItems
    }, searchEngine);
  } else {
    jQuery("#cw_search_button").show();
    jQuery("#cw_reset_button").show();
    cwAPI.cwSearchEngine.prepareSearch(name, {
      "associations": JSONItems
    }, searchEngine);
  }
};

// check if the attribute match the value 
cwAPI.cwSearchEngine.matchAttribute = function(_item_data, attributeValue, shouldMatch) {
  var _regexp, _matches_name, found, _regexp_name;
  if(_item_data === null) {
    return [_item_data, false];
  }
  if(_item_data.length === 0) {
    return [_item_data, false];
  }
  //console.log(_item_data); 
  _item_data[attributeValue] = cwAPI.cwSearchEngine.removeSearchEngineZone(_item_data[attributeValue]);
  _regexp = new RegExp('(?![^&;]+;)(?!<[^<>]*)(' + shouldMatch + ')(?![^<>]*>)(?![^&;]+;)', 'gi');
  _matches_name = _item_data[attributeValue].match(_regexp);
  found = false;

  if(_matches_name) {
    //console.log(_matches_name); 
    jQuery.each(_matches_name, function(i, item) {
      _regexp_name = new RegExp('(?![^&;]+;)(?!<[^<>]*)(' + _matches_name[i] + ')(?![^<>]*>)(?![^&;]+;)', 'gi');
      _item_data[attributeValue] = _item_data[attributeValue].replace(_regexp_name, "<span class='cw-searchengine-item-found'>" + _matches_name[i] + '</span>');
      found = true;
    });
  }
  return [_item_data, found];
};

cwAPI.cwSearchEngine.searchValueRec = function(_input_value, _item, searchItem) {
  //console.log(_item, searchItem);
  var foundItems = [];

  if(_.isUndefined(_item.associations[searchItem.id])) {
    return foundItems;
  }
  // pour les elements à rechercher
  //console.log('search', _item.associations, searchItem.id);
  jQuery.each(_item.associations[searchItem.id], function(i, element) {
    var childrenCount, res;
    childrenCount = 0;
    // pour chaque propriété
    res = cwAPI.cwSearchEngine.matchAttribute(element, "name", _input_value);
    if(res[1] === true) {
      childrenCount = 1;
    }
    jQuery.each(searchItem.properties, function(propertyKey, property) {
      var res;
      if(property === "name") {
        return;
      }
      res = cwAPI.cwSearchEngine.matchAttribute(element.properties, property, _input_value);
      if(res[1] === true) {
        childrenCount = 1;
      }
    });
    if(searchItem !== null && searchItem.children.length > 0) {
      jQuery.each(searchItem.children, function(childKey, child) {
        element.associations[child.id] = cwAPI.cwSearchEngine.searchValueRec(_input_value, element, child);
        childrenCount += element.associations[child.id].length;
      });
    }
    if(childrenCount > 0) {
      foundItems.push(element);
    }
  });
  //console.log("fI",foundItems)
  return foundItems;
};

cwAPI.cwSearchEngine.searchValueInAttributes = function(_input_value, _item_dataInput, searchEngine, output) {
  var _item_data, _regexp, foundItems;
  if(_item_dataInput === null) {
    return;
  }
  _item_data = jQuery.extend(true, {}, _item_dataInput);
  _regexp = new RegExp('(?![^&;]+;)(?!<[^<>]*)(' + _input_value + ')(?![^<>]*>)(?![^&;]+;)', 'gi');

  //console.log('search in', _item_dataInput, _item_dataInput.associations);
  foundItems = {};
  // pour chaque requirement
  jQuery.each(searchEngine.searchItemsRequirements, function(searchItemKey, searchItem) {
    if(_.isUndefined(searchItem)) {
      return;
    }
    foundItems[searchItem.id] = cwAPI.cwSearchEngine.searchValueRec(_input_value, _item_data, searchItem);
  });

  //console.log('found', foundItems);
  cwAPI.cwSearchEngine.searchMemory.searchResultData = {
    "associations": foundItems
  };
  searchEngine.searchFunction(foundItems, true);
  jQuery('.cw-searchengine-suggest').focus();

};

// prepare the search action on the attributes 
cwAPI.cwSearchEngine.prepareSearch = function(viewName, JSONItems, searchEngine) {
  var output, inputVar, _input_value;
  inputVar = "cw-searchengine-input-suggest-" + viewName;
  //search by key enter
  jQuery('#' + inputVar).keyup(function(e) {
    //if the key enter typed
    if(e.keyCode === 13) {
      search();
    }
  });

  //search by click the button search
  jQuery('#cw_search_button').click(function() {
    search();
  });
  var search = function() {
      cwAPI.cwSearchEngine.searchMemory.searchResultData = JSONItems;
      _input_value = jQuery('#' + inputVar).val();
      jQuery('#zone_' + viewName).html('');

      if(_input_value.length < 2) {
        cwAPI.cwSearchEngine.printAllElements(JSONItems, searchEngine);
        return;
      }
      if(_input_value.length === 0) {
        cwAPI.cwSearchEngine.searchMemory.searchResultData = JSONItems;
      }
      input_value = _input_value.replace(/ /g, ".*");
      output = [];
      if(cwAPI.cwSearchEngine.searchMemory.searchResultData === null) {
        cwAPI.cwSearchEngine.searchMemory.searchResultData = JSONItems;
      }
      cwAPI.cwSearchEngine.searchValueInAttributes(_input_value, cwAPI.cwSearchEngine.searchMemory.searchResultData, searchEngine, output);
    };
  //onclick reset button
  jQuery('#cw_reset_button').click(function() {
    // print all the elements, clear input value, and leave
    cwAPI.cwSearchEngine.printAllElements(JSONItems, searchEngine);
    jQuery('#' + inputVar).val(null);
    return;
  });
};

// print all the elements
cwAPI.cwSearchEngine.printAllElements = function(JSONItems, searchEngine) {
  cwAPI.cwSearchEngine.searchMemory.searchResultData = null;
  searchEngine.searchFunction(JSONItems.associations, false);

};
// prepare the search action on the attributes 
cwAPI.cwSearchEngine.prepareInstantSearch = function(viewName, JSONItems, searchEngine) {
  var output, inputVar, _input_value;
  inputVar = "cw-searchengine-input-suggest-" + viewName;

  // if something has been typed 
  jQuery('#' + inputVar).keyup(function(e) {
    // get the typed value 
    _input_value = jQuery(this).val();
    // clean the list of elements 
    jQuery('#zone_' + viewName).html('');
    // if there is less than 2 caracters, exit 
    if(_input_value.length < 2) {
      // print all the elements and leave  
      cwAPI.cwSearchEngine.printAllElements(JSONItems, searchEngine);
      return;
    }
    if(e.keyCode === 8) {
      cwAPI.cwSearchEngine.searchMemory.searchResultData = JSONItems;
    }

    _input_value = _input_value.replace(/ /g, ".*");
    output = [];
    if(cwAPI.cwSearchEngine.searchMemory.searchResultData === null) {
      cwAPI.cwSearchEngine.searchMemory.searchResultData = JSONItems;
    }
    //console.log("searchMemory.searchResultData",  cwAPI.cwSearchEngine.searchMemory.searchResultData.associations);
    cwAPI.cwSearchEngine.searchValueInAttributes(_input_value, cwAPI.cwSearchEngine.searchMemory.searchResultData, searchEngine, output);
  });
};