/*global DiagramCanvas: true, cwAPI:true*/

DiagramCanvas.prototype.setUpNavigationOptions = function(diagramID) {
  this.searchID = jQuery(this.mainDiv).attr('id') + '-search';
  this.diagramOptions = new DiagramCanvas.cwDiagramOptions(this, diagramID, "ui-corner-all");
};

DiagramCanvas.cwDiagramOptions = function(diagramCanvas, diagramID, corner) {
  this.diagramCanvas = diagramCanvas;
  this.diagramID = diagramID;
  this.corner = corner;
  this.listID = "cw-diagram-options-" + diagramID;

  this.setUpOptions();
  this.createOptionsList();

  this.addSearchInput();
  this.setupIcons();
  this.setupToolTipsForDiagramNavigationIcons();
  this.setupFullScreenMode();
  this.setupEvents();
  this.setUpDisplay();

  if(this.isMobileBrowser()) {
    window.onorientationchange = this.orientationChanged.bind(this);
  }

};

DiagramCanvas.cwDiagramOptions.prototype.orientationChanged = function() {
  var toogleIcon = jQuery(this.UL).find('a.cw-diagram-option-fullscreen');
  this.setToFullScreen(toogleIcon);
}


DiagramCanvas.prototype.setValueForDiagramSearch = function(value) {
  jQuery("#" + this.searchID).val(value);
  jQuery("#" + this.searchID).keyup();
};

DiagramCanvas.prototype.cleanValueForDiagramSearch = function() {
  jQuery("#" + this.searchID).val("");
  jQuery("#" + this.searchID).keyup();
};


DiagramCanvas.cwDiagramOptions.prototype.createOptionsList = function() {
  var output = [];
  output.push("<ul id='" + this.listID + "' class='cw-diagram-options " + this.corner + "'>");
  _.each(this.options, function(option) {
    output.push("<li class='cw-diagram-options-li'><a class='cw-diagram-option-", option.name, " cw-diagram-option cw-tooltip-me ", this.corner, "' title='<div class=\"cw-simpleText\">", jQuery.i18n.prop("diagram_option_" + option.name), "</div>'>", jQuery.i18n.prop("diagram_option_" + option.name), "</a></li>");
  }.bind(this));
  output.push('</ul>');
  jQuery(this.diagramCanvas.mainDiv).append(output.join(''));
  this.UL = jQuery("#" + this.listID);
  //console.log("options UL", this.UL);
};

DiagramCanvas.cwDiagramOptions.prototype.addSearchInput = function() {
  var searchIcon = jQuery("#" + this.listID).find("a.cw-diagram-option-search");
  searchIcon.before("<input class='cw-diagram-option-search-input' id='" + this.diagramCanvas.searchID + "'/>");
  jQuery('#' + this.diagramCanvas.searchID).hide();
};

DiagramCanvas.cwDiagramOptions.prototype.setupIcons = function() {
  _.each(this.options, function(option) {
    //console.log(jQuery(this.UL).find('.cw-diagram-option-' + option.name), option.name, jQuery(this.UL));
    jQuery(this.UL).find('.cw-diagram-option-' + option.name).button({
      "icons": {
        "primary": 'ui-icon-' + option.icon
      },
      "text": false
    });
  }.bind(this));
};

DiagramCanvas.cwDiagramOptions.prototype.setUpDisplay = function() {
  jQuery(this.diagramCanvas.mainDiv).hover(function(e) {
    this.showNavigationBar();
  }.bind(this), function(e) {
    this.hideNavigationBar();
  }.bind(this));

};

DiagramCanvas.cwDiagramOptions.prototype.setUpOptions = function() {
  this.options = [{
    "name": "zoomin",
    "icon": "zoomin"
  }, {
    "name": "zoomout",
    "icon": "zoomout"
  }, {
    "name": "resize",
    "icon": "newwin"
  }, {
    "name": "search",
    "icon": "search"
  }, {
    "name": "fullscreen",
    "icon": "squaresmall-plus"
  }, {
    "name": "parent_diagram",
    "icon": "parentDiag"
  }];
};


DiagramCanvas.cwDiagramOptions.prototype.setupToolTipsForDiagramNavigationIcons = function() {
  //console.log(jQuery(this.UL).find('a.cw-diagram-option'));
  jQuery(this.UL).find('a.cw-diagram-option').tooltip({
    "delay": 250,
    "showURL": false
  });
};


DiagramCanvas.cwDiagramOptions.prototype.setupEvents = function() {
  jQuery(this.UL).find('a.cw-diagram-option-zoomin').click(function() {
    this.diagramCanvas.camera.scale *= 0.9;
    this.diagramCanvas.camera.update();
    //this.diagramCanvas.tick();
    return false;
  }.bind(this));
  jQuery(this.UL).find('a.cw-diagram-option-zoomout').click(function() {
    this.diagramCanvas.camera.scale *= 1.1;
    this.diagramCanvas.camera.update();
    //this.diagramCanvas.tick();
    return false;
  }.bind(this));
  jQuery(this.UL).find('a.cw-diagram-option-resize').click(function() {
    this.diagramCanvas.setInitPositionAndScale();
    //this.diagramCanvas.tick();
    return false;
  }.bind(this));
  jQuery(this.UL).find('a.cw-diagram-option-search').click(function() {
    jQuery('#' + this.diagramCanvas.searchID).toggle('clip');
    return false;
  }.bind(this));
  jQuery("#" + this.diagramCanvas.searchID).keyup(function() {
    var value = jQuery("#" + this.diagramCanvas.searchID).val();
    this.diagramCanvas.mainDiagramContext.createContext(value, this.diagramCanvas.camera.renderScale, function(loadedContext) {
      this.diagramCanvas.camera.update();
      //this.diagramCanvas.tick();
    }.bind(this));
  }.bind(this));
  jQuery(this.UL).find('a.cw-diagram-option-parent_diagram').click(function() {
    if (!_.isUndefined(this.diagramCanvas.diagramInfo) && !_.isUndefined(this.diagramCanvas.diagramInfo.parentsId)){
      var _parentIds = new Array();
      jQuery.each(this.diagramCanvas.diagramInfo.parentsId, function(index, _parent){
        _parentIds.push(_parent);
      });
      cwAPI.openDiagramPageWhenClickingOnParentDiagramIcon(this.diagramCanvas, _parentIds);
    }
  }.bind(this));
};

DiagramCanvas.prototype.updateSearchBoxPosition = function() {
  //var navigationUL = jQuery(this.mainDiv).children("ul.diagramNavigation");
  this.diagramOptions.UL.css('width', '24px');
  var left = jQuery(this.mainDiv).width() - this.diagramOptions.UL.width() - 15;
  this.diagramOptions.UL.css('position', 'relative').css('top', '0px');
  //jQuery(this.mainDiv).children("canvas").css('position', 'relative').css('left', "0px").css('top', '0px');
  //navigationUL.css('position', 'absolute').css('right', "0px").css('top', top + "px");
  jQuery('#' + this.searchID).css('position', 'absolute').css('left', -jQuery("#" + this.searchID).width()).addClass("ui-widget ui-widget-content ui-corner-all");
};


DiagramCanvas.cwDiagramOptions.prototype.toogleFullScreen = function(icon) {
  if(jQuery(icon).hasClass("fullscreen-mode")) {
    //console.log("setToNormalSizeScreen");
    this.setToNormalSizeScreen(icon);
    jQuery(icon).mouseleave();
  } else {
    //console.log("setToFullScreen");
    this.setToFullScreen(icon);
    jQuery(icon).mouseleave();
  }
}

DiagramCanvas.cwDiagramOptions.prototype.setupFullScreenMode = function() {
  var diagramOptions;
  this.fullScreenMode = {};
  this.fullScreenMode.parentNode = this.diagramCanvas.mainDiv.parent();
  diagramOptions = this;
  jQuery(this.UL).find('a.cw-diagram-option-fullscreen').click(function(e) {
    //console.log(jQuery(diagramCanvas.mainDiv).find("a.diagram-fullscreen span.ui-button-text"));
    diagramOptions.toogleFullScreen(this);
    return false;
  });
};

DiagramCanvas.cwDiagramOptions.prototype.setToFullScreen = function(fullScreenIcon) {
  var mainDiv, w, h, offset;

  mainDiv = jQuery(this.diagramCanvas.mainDiv);
  this.fullScreenMode.savedScrollTop = jQuery(window).scrollTop();
  
  jQuery(fullScreenIcon).addClass("fullscreen-mode");
  jQuery(fullScreenIcon).children('span.ui-icon').removeClass("ui-icon-squaresmall-plus");
  jQuery(fullScreenIcon).children('span.ui-icon').addClass("ui-icon-squaresmall-minus");

  mainDiv.find("a.diagram-fullscreen span.ui-button-text").html(jQuery.i18n.prop("diagram_option_fullscreen_normal"));
  mainDiv.find("a.diagram-fullscreen").attr("title", jQuery.i18n.prop("diagram_option_fullscreen_normal"));

  w = jQuery(window).width() - 5 + 'px';
  h = jQuery(window).height() - 5 + 'px';

  offset = mainDiv.offset();
  mainDiv.detach();
  jQuery("body").children().fadeOut(500);
  jQuery("body").children().first().before(mainDiv);
  mainDiv.css('width', mainDiv.width() + "px").css("background-color", 'white');
  mainDiv.css('position', 'absolute').css('top', offset.top + "px").css('left', offset.left).css('z-index', "2000");

  var target = {
    'width': w,
    'height': h,
    "left": "0px",
    "top": "0px",
    "background-color": "white",
    "margin": "0px",
    "padding": "0px",
    "duration": 500
  }


  if(this.isMobileBrowser) {
    mainDiv.css('width', target.width);
    mainDiv.css('height', target.height);
    mainDiv.css('left', target.left);
    mainDiv.css('top', target.top);
    mainDiv.css('background-color', target['background-color']);
    mainDiv.css('margin', target.margin);
    mainDiv.css('padding', target.padding);
    this.diagramCanvas.resize();
  } else {
    mainDiv.animate(target, {
      "step": function() {
        this.diagramCanvas.resize();
      }.bind(this),
      "complete": function() {}
    });

  }


  jQuery('html,body').animate({
    "scrollTop": 0
  }, 100);

};


DiagramCanvas.cwDiagramOptions.prototype.isMobileBrowser = function() {
  if(navigator.userAgent.match(/Android/i) || navigator.userAgent.match(/webOS/i) || navigator.userAgent.match(/iPhone/i) || navigator.userAgent.match(/iPad/i) || navigator.userAgent.match(/iPod/i) || navigator.userAgent.match(/BlackBerry/i)) {
    return true;
  }
  return false;
};

DiagramCanvas.cwDiagramOptions.prototype.setToNormalSizeScreen = function(fullScreenIcon) {
  var mainDiv;

  mainDiv = jQuery(this.diagramCanvas.mainDiv);

  jQuery(fullScreenIcon).removeClass("fullscreen-mode");
  jQuery(fullScreenIcon).children('span.ui-icon').removeClass("ui-icon-squaresmall-minus");
  jQuery(fullScreenIcon).children('span.ui-icon').addClass("ui-icon-squaresmall-plus");
  mainDiv.find("a.diagram-fullscreen span.ui-button-text").html(jQuery.i18n.prop("diagram_navigation_fullscreen"));
  mainDiv.find("a.diagram-fullscreen").attr("title", jQuery.i18n.prop("diagram_navigation_fullscreen"));
  this.setupToolTipsForDiagramNavigationIcons();

  // reset style
  mainDiv.attr('style', '');

  mainDiv.detach();
  jQuery('body').children().fadeIn(500);

  jQuery('html,body').animate({
    "scrollTop": this.fullScreenMode.savedScrollTop
  }, 100);

  this.fullScreenMode.parentNode.append(mainDiv);
  mainDiv.resize();
  cwAPI.hideLoadingImage();
};

DiagramCanvas.cwDiagramOptions.prototype.showNavigationBar = function() {
  if(!jQuery(this.UL).is(':visible')) {
    jQuery(this.UL).show('slide', {
      'direction': 'right'
    });
  }
};


DiagramCanvas.cwDiagramOptions.prototype.hideNavigationBar = function() {
  if(!cwAPI.isUnderIE9()) {
    if(jQuery(this.UL).is(':visible')) {
      jQuery(this.UL).hide('slide', {
        'direction': 'right'
      });
    }
  }
};