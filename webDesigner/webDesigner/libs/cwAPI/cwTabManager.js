/*global cwAPI :true, cwBehaviours:true*/

var cwTabManager = {};

// active the tabs
cwTabManager.loadTab = function(tabID) {
	var diagramID, designID, design;


	jQuery(tabID).find('div.cw-diagram-zone').each(function(i, diagramZone) {
		diagramID = jQuery(diagramZone).attr('data-diagramid');
		cwAPI.getDiagram(diagramID, jQuery(diagramZone).attr('id'), function() {});
	});


	jQuery(tabID).find('div.cw-diagram-designer-zone').each(function(i, diagramDesignerZone) {
		jQuery(diagramDesignerZone).addClass("property-box");
		designID = jQuery(diagramDesignerZone).attr('data-designid');
		design = jQuery('body').data('design' + designID);
		design.clean();
		design.createCanvas();
	});

	cwBehaviours.cwWorldMap.loadWorldMap(jQuery(tabID));

	/*	jQuery(tabID).find('canvas.world-map').each(function (i, canvasWorld) {
		var itemKey = jQuery(canvasWorld).attr('data-itemkey');
		var id = jQuery(canvasWorld).attr('id');
		var countries = jQuery(canvasWorld).attr('data-countries');
		jQuery(canvasWorld).remove();
		jQuery(canvasWorld).css('width', '200px');
		cwAPI.countriesToMapFromStringCountries(itemKey, id, countries);
	});
*/
};

cwTabManager.transformTabToVertical = function(selectorID) {
	jQuery(selectorID).tabs().addClass('ui-tabs-vertical ui-helper-clearfix');
	jQuery(selectorID + " li.tab-header").removeClass('ui-corner-top').addClass('ui-corner-left');
	jQuery(selectorID + " .ui-tabs-panel").addClass('ui-widget ui-widget-content');
};

cwTabManager.activeTab = function(selector) {
	jQuery(".tab-content").each(function(i, tab) {
		var htmlContent = jQuery(tab).html();

		//console.log('htmlContent', htmlContent, htmlContent.length, jQuery(tab).attr('id'));
		if(htmlContent.length === 0) {
			var hide = jQuery(tab).attr('data-hide');
			if(hide === 'true') {
				jQuery('.header-' + jQuery(tab).attr('id')).remove();
				jQuery(tab).remove();
			}
		}
	});

	jQuery(selector).tabs({
		"show": function(event, ui) {
			var tabID = ui.tab.hash;
			cwTabManager.loadTab(tabID);
			//var re = /#\w+$/;
			//var url = document.location.toString();
			//document.location.hash = ui.panel.id;
			/*			jQuery('body').scrollTop({
				"top": 0
			});*/
		},
		"cookie": {
			// store cookie for a day, without, it would be a session cookie
			expires: 1
		}
	});
	jQuery(selector).bind('tabsshow', function(event, ui) {
		//console.log('show', ui);
	});


	jQuery(selector + " .ui-tabs-nav").removeClass('ui-widget-header');
	cwTabManager.resizeTabContent(selector);
	//jQuery(selector).tabs('select', 0);
	// show default tab
	var selectedTabID = jQuery(selector + " .ui-tabs-selected a").attr('href');
	cwTabManager.loadTab(selectedTabID);

	jQuery(window).resize(function() {
		cwTabManager.resizeTabContent(selector);
	});
};


cwTabManager.resizeTabContent = function(selector) {
	var firstLi, offset, leftMenuSize, maximumSize, sizeContent;
	firstLi = jQuery(selector + " .ui-tabs-nav li.tab-header").first();
	if(firstLi.length > 0) {
		offset = firstLi.offset();
		leftMenuSize = offset.left + firstLi.width();
		maximumSize = jQuery(window).width();
		sizeContent = maximumSize - leftMenuSize - 10 - 50 - 40;
		//console.log(sizeContent);
		jQuery(selector + " .tab-content").css('width', sizeContent);
	}
};

// add a tab title
cwTabManager.createTextTab = function(output, selector, name, icon, tabID, hide) {
	output.push('<li class="', tabID, '-header tab-header header-' + selector + '"><a href="#', selector, '"><span class="ui-icon ui-icon-', icon, '"></span>', name, '</a></li>');
};

// create content for a tab
cwTabManager.createTextTabContent = function(output, selector, content_callback, hide) {
	output.push('<div id="', selector, '" class="tab-content" data-hide="', hide, '">');
	content_callback(output);
	output.push('</div>');
};


// create a diagram tab
/*function createDiagramTab(output, mainItem, diagramCategory, savedDiagrams, tabName, activeItems) {
	$.each(mainItem.diagramsExploded, function (abbr, diagrams) {
		$.each(diagrams, function (dKey, d) {
			if (abbr === diagramCategory) {
				output.push('<li><a href="#tabs-diagram-' + d.id + '"><span class="ui-icon ui-icon-image"></span>', tabName, '</a></li>');
				d.activeItems = activeItems;
				d.diagramImage = new Image();
				d.diagramImage.src = "../images/print/diagram" + d.id + ".png";
				savedDiagrams[d.id] = d;
			}
		});
	});
}

// create the diagram tab content

function createDiagramTabContent(output, mainItem, diagramCategory) {
	$.each(mainItem.diagramsExploded, function (abbr, diagrams) {
		$.each(diagrams, function (dKey, d) {
			if (abbr === diagramCategory) {
				output.push('<div id="tabs-diagram-' + d.id + '" class="diagramTabContent">');
				output.push('</div>');
			}
		});
	});
}*/