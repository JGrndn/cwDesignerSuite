/*global pageName : true, drawCurrentPageFunction : true, cwAPI:true, cwConfigs :true */

//console.log('pgi');
cwAPI.loadIndexPage = function() {
    var queryObject = cwAPI.getQueryStringObject();
    
	if (_.isUndefined(queryObject.cwtype)) {
		jQuery('#top_of_page').after('cwtype is required in the url, could be index or single');
	} else {
		if (_.isUndefined(queryObject.cwview)) {
			jQuery('#top_of_page').after('cwview is required in the url');
		} else {
			queryObject.cwview = queryObject.cwview.toLowerCase();
			//<img class='cwloading' src='../images/_loading.gif' alt='Loading...'>
			jQuery('#top_of_page').after("<div class='cw-main-zone' id='zone_" + queryObject.cwview + "'></div>");
			switch (queryObject.cwtype) {
			case 'single':
				if (_.isUndefined(queryObject.cwid)) {
					jQuery('#top_of_page').after('cwid is required in the url');
				} else {
					/*if ('diagram' === queryObject.cwview) {

						//jQuery('#top_of_page').cwhop("1", "2");
						jQuery('#top_of_page').css('height', '100%');
						jQuery('#top_of_page').appendDiagramZoneFromDiagramID(queryObject.cwid, 'cw-standalone-diagram', function(diagramCanvas) {
							cwAPI.hideLoadingImage();
						});
					} else {*/
						//jQuery('#top_of_page').after('ready to load');
						cwAPI.chainLoadPage(queryObject.cwview);
						cwAPI.loadUniquePageSingle(queryObject.cwview, queryObject.cwid);
					/*}*/
				}
				break;
			case 'index':
				jQuery('head').append("<script type='text/javascript' src='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/' + queryObject.cwview + '/' + queryObject.cwview + ".generated.js'></script>");
				cwAPI.chainLoadPage(queryObject.cwview);
				cwAPI.loadUniquePageIndex(queryObject.cwview);
				break;
			}
		}
	}
};


cwAPI.chainLoadPage = function(pageName, pageType) {
	var headerOutput = [];
	headerOutput.push("<link type='text/css' rel='stylesheet' media='all' href='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/handmade/' + pageName + '/' + pageName + ".css' />");
	headerOutput.push("<!--[if IE 7]><link type='text/css' rel='stylesheet' media='all' href='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/handmade/' + pageName + '/' + pageName + ".ie7.css'/><![endif]-->");
	headerOutput.push("<!--[if IE 8]><link type='text/css' rel='stylesheet' media='all' href='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/handmade/' + pageName + '/' + pageName + ".ie8.css'/><![endif]-->");
	headerOutput.push("<!--[if lt IE 9]><link type='text/css' rel='stylesheet' media='all' href='" + cwConfigs.SITE_MEDIA_PATH + "webdesigner/handmade/" + pageName + "/" + pageName + ".lt-ie9.css'/><![endif]-->");
	headerOutput.push("<script type='text/javascript' src='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/' + pageName + '/layouts/' + pageName + ".js'></script>");
	headerOutput.push("<script type='text/javascript' src='" + cwConfigs.SITE_MEDIA_PATH + 'webdesigner/handmade/' + pageName + '/' + pageName + ".js'></script>");
	jQuery('head').append(headerOutput.join(''));
};



cwAPI.getQueryStringObject = function() {
	var queryString, pageArguments, pageArgumentsObject, pageArgumentKeyAndValue;
	pageArgumentsObject = {};
	if (jQuery(location).attr('href').indexOf('?') !== -1) {
		queryString = jQuery(location).attr('href');
		pageArguments = queryString.split('?');
		//console.log(pageArguments);
		_.each(pageArguments[1].split('&'), function(a) {
			pageArgumentKeyAndValue = a.split('=');
			if (pageArgumentKeyAndValue[1].indexOf('#') !== -1) {
				 pageArgumentKeyAndValue[1] = pageArgumentKeyAndValue[1].split('#')[0];
			}
			pageArgumentsObject[pageArgumentKeyAndValue[0]] = pageArgumentKeyAndValue[1];
		});
	} else {
		jQuery('#top_of_page').after('page usage : index.html?cwtype=[<b>single</b>|<b>index</b>]&cwview=<b>view_name</b>&cwid=<b>item_id</b>');
	}
	return pageArgumentsObject;
};

cwAPI.getJSONFile = function(url, success, error) {
	jQuery.ajax({
		'url': url,
		'dataType': 'json',
		'cache': true,
		'success': success,
		'error': error,
		'crossDomain': true
	});
};



cwAPI.errorOnLoadPage = function(err) {
	if (err.status === 404) {
		jQuery('#top_of_page').after('The required item do not exists yet, please wait a few minutes before the item will be available.');
	}
	//console.log("status ", err.status, " ,statusText ", err.statusText, " ,responseText ", err.responseText, " ,readyState ", err.readyState);
};


cwAPI.updateIDIfRequired = function(id) {
	var pageArgumentsWithPipes;
	if (id.indexOf('|') !== -1) {
		// has pipe
		pageArgumentsWithPipes = id.split('|');
		// get the last element
		return pageArgumentsWithPipes[pageArgumentsWithPipes.length - 1];
	} else if (id.indexOf('%7c') !== -1) {
		pageArgumentsWithPipes = id.split('%7c');
		return pageArgumentsWithPipes[pageArgumentsWithPipes.length - 1];
	} else {
		return id;
	}
};

cwAPI.loadUniquePageSingle = function(pageName, id) {
	var jsonFile;
	if (!_.isUndefined(id)) {
		id = cwAPI.updateIDIfRequired(id);

		if (_.isUndefined(cwConfigs.WEBDESIGNER_SERVER_URL)) {
			jsonFile = cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/' + pageName + '/json/' + pageName + id + '.' + cwConfigs.JSON_EXTENTION;
		} else {
			jsonFile = cwConfigs.WEBDESIGNER_SERVER_URL + 'Page/' + pageName + '/' + id + '?' + Math.random();
		}

		cwAPI.getJSONFile(jsonFile, function(o) {
			cwAPI.hideLoadingImage();
			//if (_.isUndefined(window["drawItems_" + pageName]) && _.isFunction(window["drawItems_" + pageName])){
			var data = o;
			if (!_.isUndefined(o.diagram)){
				data = o.diagram;
			}
			window['drawItems_' + pageName](data);
			//} else {
			//	jQuery('#top_of_page').after('the requested page do not exists');
			//}
		}, cwAPI.errorOnLoadPage);
	} else {
		jQuery('#top_of_page').after('the provided url is missing the ' + pageName + ' ID');
	}

};

cwAPI.loadUniquePageIndex = function(pageName) {
	var jsonFile;
	if (_.isUndefined(cwConfigs.WEBDESIGNER_SERVER_URL)) {
		jsonFile = cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/' + pageName + '/json/' + pageName + '.' + cwConfigs.JSON_EXTENTION;
	} else {
		jsonFile = cwConfigs.WEBDESIGNER_SERVER_URL + 'Page/' + pageName + '?' + Math.random();
	}



	//jsonFile = 'http://localhost:8080/publicationscw/APM/webdesigner/generated/' + pageName + '/json/' + pageName + id + '.json';
	cwAPI.getJSONFile(jsonFile, function(o) {
		cwAPI.hideLoadingImage();
		window['doWhenLoaded_' + pageName](o);
	}, cwAPI.errorOnLoadPage);
};
