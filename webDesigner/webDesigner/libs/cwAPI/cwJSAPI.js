/*global DiagramCanvas:true, alert:true, cwConfigs, jQuery : true */

var cwBehaviours = {};

jQuery.support.cors = true;

var cwAPI = {};
cwAPI.SharePoint = {};
cwAPI.appliedLayouts = [];

cwAPI.isUndefined = function(o) {
	return typeof(o) === "undefined";
};

cwAPI.SharePoint.createSharepointMenuSeparator = function() {
	return '<span class="s4-nothome s4-bcsep s4-titlesep"><span class="s4-clust ms-ltviewselectormenuseparator" style="height:11px;width:11px;position:relative;display:inline-block;overflow:hidden;"><img style="border-width:0px;position:absolute;left:-0px !important;top:-585px !important;" alt=":" src="/_layouts/images/fgimg.png"></span></span>';
};

cwAPI.SharePoint.createSharepointMenu = function(menuName, menuLink, level) {
	return '<h' + level + '><a href="' + menuLink + '">' + menuName + '</a></h' + level + '>';
};

cwAPI.SharePoint.appendMenuTitle = function(menuName, menuLink, level, lastLevel) {
	var output = [];
	output.push(cwAPI.SharePoint.createSharepointMenu(menuName, menuLink, level));
	if(!lastLevel) {
		output.push(cwAPI.SharePoint.createSharepointMenuSeparator());
	}
	jQuery("td.s4-titletext").children().last().before(output.join(''));
};


cwAPI.createLinkForSingleView = function(view, item) {
	return "index." + cwConfigs.SITE_LINK_EXTENTION + "?cwtype=single&cwview=" + view + "&cwid=" + item.object_id;
};

cwAPI.createLinkForIndexView = function(view) {
	return "index." + cwConfigs.SITE_LINK_EXTENTION + "?cwtype=index&cwview=" + view;
};

cwAPI.getDiagramPath = function(dID) {
	return cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/diagram/json/diagram' + dID + '.' + cwConfigs.JSON_EXTENTION;
};

cwAPI.getDiagram = function(dID, selectorID, callback) {
	//console.log(jQuery('#' + selectorID));
	if(jQuery('#' + selectorID).length === 0) {
		return;
	}
	jQuery.getJSON(cwAPI.getDiagramPath(dID), function(jsonDiagramFile) {
		var diagramCanvasTag, diagramCanvas;
		diagramCanvasTag = cwAPI.cwDiagramManager.appendDiagramCanvas(jQuery('#' + selectorID), jsonDiagramFile);
		//console.log(diagramCanvasTag, jQuery('#' + selectorID), selectorID);
		diagramCanvas = new DiagramCanvas(dID, jsonDiagramFile, selectorID);
		if(!_.isUndefined(callback)) {
			return callback(diagramCanvas);
		}
	});
};

if(!Function.prototype.bind) {
	Function.prototype.bind = function(oThis) {
		if(typeof this !== "function") {
			// closest thing possible to the ECMAScript 5 internal IsCallable function
			throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
		}
		var fSlice = Array.prototype.slice,
			aArgs = fSlice.call(arguments, 1),
			fToBind = this,
			FNOP = function() {},
			FBound = function() {
				return fToBind.apply(this instanceof FNOP ? this : oThis || window, aArgs.concat(fSlice.call(arguments)));
			};
		FNOP.prototype = this.prototype;
		FBound.prototype = new FNOP();
		return FBound;
	};
}


cwAPI.setupLanguage = function(language, callback) {
	jQuery.i18n.properties({
		"name": 'translations',
		"path": cwConfigs.SITE_MEDIA_PATH + "i18n/",
		"mode": "map",
		"language": language,
		"callback": function() {
			if(!_.isUndefined(callback)) {
				return callback();
			}
		}
	});
};



cwAPI.isUnderIE9 = function() {
	var version = Math.floor(jQuery.browser.version);
	//console.log(version);
	if(jQuery.browser.msie && version < 9) {
		//alert('under ie 9');
		return true;
	}
	return false;
};

cwAPI.isIEBrowser = function() {
	var browser = navigator.appName;
	if(browser == "Microsoft Internet Explorer") {
		return true;
	}
	return false;
}

cwAPI.loadTopMenu = function() {
	var pagesURL, output;

	pagesURL = cwConfigs.SITE_MEDIA_PATH + "webdesigner/generated/pages." + cwConfigs.JSON_EXTENTION;
	cwAPI.getJSONFile(pagesURL, function(pages) {

		output = [];
		output.push('<ul class="cwMenuTop">');
		_.each(pages.index, function(page) {
			output.push('<li><a href="', page.link, '">', page.name, '</a></li>');
		});
		output.push('</ul>');
		jQuery('#top_of_page').before(output.join(''));

	}, cwAPI.errorOnLoadPage);
};

cwAPI.hideLoadingImage = function() {
	jQuery('.cwloading').hide();
};

cwAPI.shortText = function(longText, textlength) {
	var short_text;
	if(longText.length > textlength) {
		short_text = longText.substring(0, textlength);

		var lastIndex = short_text.lastIndexOf(" ");
		short_text = short_text.substring(0, lastIndex);
		short_text = short_text + " ...";

	} else {
		short_text = longText;
	}
	return short_text;
};

cwAPI.openDiagramPageWhenClickingOnDiagramShape = function(diagramCanvas, diagrams){
	cwAPI.openDiagramPageCustomHandler(diagrams);
};

cwAPI.openDiagramPageWhenClickingOnParentDiagramIcon = function(diagramCanvas, diagrams){
	cwAPI.openDiagramPageCustomHandler(diagrams);
};

cwAPI.openDiagramPageCustomHandler = function(diagrams){
	if (!_.isUndefined(diagrams.length) && diagrams.length > 0){
		if (diagrams.length === 1){
			var fakeObject = { object_id:diagrams[0].id };
			window.location.href = cwAPI.createLinkForSingleView('diagram', fakeObject);
			//'index.html?cwtype=single&cwview=diagram&cwid=' + diagrams[0];
		}
		else{
			// gérer le choix pour sélectionner un diagramme quand plusieurs sont disponibles
			cwAPI.displayDialogBoxWithMultipleObjects('diagram', diagrams);
		}
	}
	return;
};

cwAPI.onAssociationRegionClickingHandler = function(diagramCanvas, sourceType, sourceId, viewName, targets){
	if (!_.isUndefined(targets.length) && targets.length > 0){
		if (targets.length === 1){
			var fakeObject = { object_id:targets[0].id };
			window.location.href = cwAPI.createLinkForSingleView(viewName, fakeObject);
		}
		else{
			// gérer le choix pour sélectionner un objet quand plusieurs sont disponibles
			cwAPI.displayDialogBoxWithMultipleObjects(viewName, targets);
		}
	}
	return;	
};

cwAPI.displayDialogBoxWithMultipleObjects = function(viewName, targetObjects){
	var form = [];
	form.push('<form class="form-select"><h3>Select an object</h3>');
	form.push('<div class="diagramsToView"><ul>');
	form.push('</ul></div>');
	form.push('</form>');

	var $ul = jQuery('.diagramsToView ul');
	$.each(targetObjects, function(i, item){
		var link = cwAPI.createLinkForSingleView(viewName, {object_id: item.id});
  		$ul.append($("<li><a class='' href='" + link + "'>" + item.name + "</a></li>"));
	});
	/*var that, o, $div, $ul;

    cwAPI.CwPopout.show($.i18n.prop('diagram_select'));
    cwAPI.CwPopout.onClose(function() {
      cwAPI.unfreeze();
    });

    function outputImage($li, explodedDiagram) {
      var image = new Image();
      image.src = cwAPI.getSiteMediaPath() + 'images/diagrams/diagram' + explodedDiagram.object_id + '.png?' + Math.random();
      image.onload = function() {
        $li.children().first().before('<img class="cwMiniImageDiagramPreview" src="' + image.src + '"/>');
      }
    }
    o = [];
    that = this;
    o.push('<form action="#" class="form-select">');
    o.push('<h3>', $.i18n.prop('diagram_selectADiagramToView'), '</h3>');
    o.push('<div class="cwDiagramExplosionMultipleChoice"><ul>');
    o.push('</ul></div>');
    o.push('</form>');
    $div = $(o.join(''));
    cwAPI.CwPopout.setContent($div);

    $ul = $div.find('ul').first();
    for (var i = 0; i < explodedDiagrams.length; i++) {
      (function(ed) {
        var miniO = [];
        miniO.push('<li>');
        miniO.push('<div>', ed.name, ' (', ed.object_id, ')</div>', '</li>');
        var $li = $(miniO.join('')).click(function() {
          cwAPI.CwPopout.hide();
          cwAPI.unfreeze();
          that.drillDownInDiagram(ed.object_id);
        });
        $ul.append($li);
      }(explodedDiagrams[i]));
    }
    cwAPI.freeze();*/
};

cwAPI.openObjectPageWhenDoubleClickingOnDiagramShape = function(diagramCanvas, cwObject){
	window.location.href = cwAPI.createLinkForSingleView(cwObject.objectTypeName.toLowerCase(), cwObject);
};

cwAPI.displayInfoAboutObject = function(cwObject){
	if (!_.isUndefined(cwObject) && !_.isUndefined(cwObject.name)){
		jQuery('#cw-diagramShape-info').text(cwObject.name);
	}
};

cwAPI.hideInfoAboutObject = function(){
	jQuery('#cw-diagramShape-info').empty();
	jQuery('#cw-diagramShape-info').hide();
};

cwAPI.appendFloatingDivToBody = function(){
	if (jQuery().length > 0){
		return;
	}
	else{
		jQuery('body').append(cwAPI.getShapeInfoDivString());
		jQuery('#cw-diagramShape-info').css('position','absolute');
		jQuery('.cw-diagram-zone').bind('contextmenu', function(e){
			return false;
		});
		return;
	}
};

cwAPI.getShapeInfoDivString = function(){
	return '<div class="cw-diagramShape-info" id="cw-diagramShape-info" display="none"></div>';
};

cwAPI.compareShapes = function(shape, previousShape){
	if (shape === null || _.isUndefined(shape) || previousShape === null || _.isUndefined(previousShape)){
		return false;
	}
	if (shape.x != previousShape.x || shape.y != previousShape.y || shape.w != previousShape.w || shape.h != previousShape.h){
		return false;
	}
	return true;
};

cwAPI.setListExpandable = function (div) {
    var _siblings = $(div).siblings();
    if ($(_siblings).is(':visible')) {
        $(_siblings).hide("slow");
    } else {
        $(_siblings).show("slow");
    }
};

cwAPI.transformToABCorder = function (all_items) {
    var isInteger_re = /^\s*(\+|-)?\d+\s*$/;

    function isInteger(s) {
        return String(s).search(isInteger_re) != -1;
    }
    var itemList = {};
    jQuery.each(all_items, function (i, item) {
        var key = item.properties["name"].substring(0, 1).toUpperCase();
        if (isInteger(key)) {
            key = "0-9";
        }

        if (_.isUndefined(itemList[key])) {
            itemList[key] = [];
        }
        itemList[key].push(item);
    });
    return itemList;
};