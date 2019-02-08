/*global DiagramShape:true, DiagramJoiner:true, cwPoint:true, cwAPI:true, G_vmlCanvasManager:true, CanvasCamera:true, DiagramContext:true */

var DiagramCanvas = function(diagramID, jsonDiagramFile, selectorID) {
    var diagramShape, selectorSize, selectedShape;
    var that = this;

    window.onresize = function(){
      this.resize();
    }.bind(this);

    window.requestAnimFrame = (function() {
      return (
        window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function() {
          window.setTimeout(that.tick.bind(that), 50);
      });
    })();


    this.selectorID = selectorID;
    if(jQuery('#' + this.selectorID).length === 0) {
      return;
    }
    this.mainDiv = jQuery('#' + this.selectorID);
    this.canvasID = this.selectorID + "-canvas";

    this.diagramInfo = jsonDiagramFile.diagram;

    this.navigationOptions = {};
    this.navigationOptions.selectOnS = true;
    this.navigationOptions.explodeOnD = true;
    this.mainDiagramContext = new DiagramContext(jsonDiagramFile);

    jQuery("body").data('diagram' + diagramID, this);
    jQuery("#" + this.selectorID).attr('data-diagramid', diagramID);

    this.setupNavigationDiagrams();
    this.setUpMainContext();

    this.camera = new CanvasCamera(this.canvas, this.diagramInfo.size, this.tick.bind(this));
    this.camera.scaleIsMoreThanScaleMax = function(camera) {};
    this.camera.customHasBeenSelected = function() {
      jQuery(this.mainDiv).addClass('cw-diagram-zone-isSelected ');
      this.diagramOptions.showNavigationBar();
    }.bind(this);
    this.camera.customHasBeenReleased = function() {
      jQuery(this.mainDiv).removeClass('cw-diagram-zone-isSelected');
    }.bind(this);


    this.updateSize();
    this.setUpNavigationOptions(diagramID);

    this.setInitPositionAndScale();
    this.lastScaleOnRender = this.camera.scale;

    if(cwAPI.isUnderIE9()) {
      this.image = new Image();
      var that = this;
      this.image.onload = function() {
        that.tick();
      };
      this.image.src = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/diagram' + diagramID + '.png';
      this.setUpKeyEvents();
      this.setUpMouseEvents();
      this.setUpResize();
    } else {
      this.mainDiagramContext.createContext("", this.camera.renderScale, function(loadedContext) {
        this.tick();
        this.setUpKeyEvents();
        this.setUpMouseEvents();
        this.setUpTouchEvents();
        this.setUpResize();
      }.bind(this));
    }
  };


DiagramCanvas.prototype.setUpTouchEvents = function() {
  this.touchStartTime = new Date().getTime();
  this.touchStartPosition = new cwPoint(0, 0);
  this.touchOneFinger = false;
  jQuery(this.canvas)[0].addEventListener("touchstart", this.touchStart.bind(this), false);
  jQuery(this.canvas)[0].addEventListener("touchend", this.touchEnd.bind(this), false);
  jQuery(this.canvas)[0].addEventListener("touchmove", this.touchMove.bind(this), false);
};


DiagramCanvas.prototype.touchMove = function(e) {}

DiagramCanvas.prototype.touchStart = function(e) {

  if(e.touches.length === 5) {
    var toogleIcon = jQuery(this.diagramOptions.UL).find('a.cw-diagram-option-fullscreen');
    this.diagramOptions.toogleFullScreen(toogleIcon);
  }

  this.touchOneFinger = false;
  e = this.camera.injectTouchEventsInMouseEvent(e);
  if(e.touches.length === 1) {
    this.touchOneFinger = true;
  }
  this.touchStartPosition = new cwPoint(e.pageX, e.pageY);
  var touchDeltaTime = new Date().getTime() - this.touchStartTime;
  if(touchDeltaTime < 300 && this.touchOneFinger === true) {
    this.camera.mouseMove(e);
    this.isSKeyPressed = true;
    this.goToSelectedObjectLink(e);
    this.isSKeyPressed = false;
  }
  this.touchStartTime = new Date().getTime();
}

DiagramCanvas.prototype.touchEnd = function() {
  var e = {};
  e.pageX = this.touchStartPosition.x;
  e.pageY = this.touchStartPosition.y;
  var touchDeltaTime = new Date().getTime() - this.touchStartTime;
  this.touchOneFinger = false;
}


DiagramCanvas.prototype.reduceDiagramCanvasHeight = function(selectorSize) {
  selectorSize = jQuery("#" + this.selectorID).height();
  if(this.diagramInfo.size.h < 200) {
    this.diagramInfo.size.h = 200;
  }
  if(this.diagramInfo.size.h < selectorSize) {
    jQuery(this.mainDiv).css('height', this.diagramInfo.size.h + 20 + "px");
  }
};

DiagramCanvas.prototype.setUpDebugZone = function() {
  if(jQuery("#cw-diagram-debugger").length === 0) {
    jQuery("body").append('<div id="cw-diagram-debugger" style="position:fixed;top:0px;right:0px;background-color:#eee">debugger</div>');
  }

};

DiagramCanvas.prototype.setUpMainContext = function() {
  this.canvas = document.getElementById(this.canvasID);
  if(cwAPI.isUnderIE9()) {
    G_vmlCanvasManager.initElement(this.canvas);
    this.ctx = this.canvas.getContext('2d');
  } else {
    this.ctx = jQuery('#' + this.canvasID)[0].getContext('2d');
  }
};


DiagramCanvas.prototype.focusOnShapeIfMouseIsOver = function(e, callback) {
  var shapeGhost, focusPoint, wRatio, hRatio, diff, tX, tY, destination, ratio, direction, zoomFactor;
  shapeGhost = this.isMouseOnAShape();
  if(shapeGhost !== null) {
    focusPoint = new cwPoint(shapeGhost.shape.shape.x + shapeGhost.shape.shape.w / 2, shapeGhost.shape.shape.y + shapeGhost.shape.shape.h / 2);
    wRatio = shapeGhost.shape.shape.w / this.canvas.width;
    hRatio = shapeGhost.shape.shape.h / this.canvas.height;
    ratio = wRatio;
    if(hRatio < wRatio) {
      ratio = hRatio;
    }
    diff = this.camera.scale - ratio;
    tX = shapeGhost.shape.shape.x * (1 / ratio) * this.camera.renderScale;
    tY = shapeGhost.shape.shape.y * (1 / ratio) * this.camera.renderScale;
    destination = new cwPoint(tX, tY);
    destination.inverse();
    direction = -1;
    if(diff < 0) {
      direction = 1;
    }
    zoomFactor = 0.05;
    this.camera.mouseWheelRepeat = this.getNumberOfIterationRequired(this.camera.scale, ratio, zoomFactor, direction);
    if(direction > 0) {
      this.camera.mouseWheelRepeat *= -1;
    }
    if(this.camera.mouseWheelRepeat !== 0) {
      this.camera.zoom(e, direction, zoomFactor, 5, function() {
        this.camera.translateToDestinationOnly(destination, 20, 5, function() {
          return callback(shapeGhost.shape);
        }.bind(this));
      }.bind(this));
    }
  }
};


DiagramCanvas.prototype.getNumberOfIterationRequired = function(startScale, finalScale, scaleFactor, direction) {
  var scale, iteration;
  if(startScale === finalScale) {
    return 0;
  }
  scale = startScale;
  iteration = 0;
  while(true) {
    if(direction > 0) {
      if(scale > finalScale) {
        break;
      }
    } else {
      if(scale < finalScale) {
        break;
      }
    }
    scale *= ((direction * scaleFactor) + 1);
    iteration += 1;
  }
  return iteration;
};

DiagramCanvas.prototype.setUpMouseEvents = function() {
  jQuery(this.canvas).mousemove(function(e) {
    this.mouseMove(e);
  }.bind(this));


  jQuery(this.canvas).bind('mousewheel', function(e) {
    if(!this.camera.isSelected()) {
      return;
    }
    if(this.isDKeyPressed === true) {
      if(this.navigationDiagramsShape !== null) {
        this.navigationDiagramsShape = this.getGhostShape(this.navigationDiagramsShape.shape);
      } else {
        this.loadNavigationDiagramIfMouseIsOver();
      }
      this.strokeDiagramContextIfMouseIsOver();

    }
  }.bind(this));

  jQuery(this.canvas).dblclick(function(e) {
    if(!this.camera.isSelected()) {
      return;
    }
    this.goToSelectedObjectLink(e);
    this.zoomIntoNextContext(e);
  }.bind(this));

  jQuery(this.canvas).mouseleave(function(e){
    jQuery('#cw-diagramShape-info').empty();
    jQuery('#cw-diagramShape-info').hide();
  }.bind(this));

  jQuery(this.canvas).mousedown(function(e){
    if (e.which === 3){ // right click
      jQuery('#cw-diagramShape-info').show();
    }
  }.bind(this));

  this.camera.onClick = function(e) {
    var diagramShape, ghostShape, link;
    ghostShape = this.isMouseOnAShape();
    if(ghostShape !== null) {
      this.mouseIsInShapeOnClick(ghostShape, e);
    }
  }.bind(this);

};

DiagramCanvas.prototype.resize = function() {
  this.updateSize();
  this.setInitPositionAndScale();
  this.camera.update();
};

DiagramCanvas.prototype.setUpResize = function() {
  jQuery(window).resize(function() {
    this.resize();
  }.bind(this));
};

DiagramCanvas.prototype.setUpKeyEvents = function() {
  this.isSKeyPressed = false;
  this.isDKeyPressed = false;
  jQuery("body").keydown(function(e) {
    if(!this.camera.isSelected()) {
      return;
    }
    if(this.navigationOptions.selectOnS) {
      if(e.keyCode === 83) { // S
        this.isSKeyPressed = true;
        this.strokeShapeIfMouseIsOver("#004488", e);
      }
    }
    if(this.navigationOptions.explodeOnD) {
      if(e.keyCode === 68) { // D
        this.isDKeyPressed = true;
        this.loadNavigationDiagramIfMouseIsOver();
      }
    }


  }.bind(this));
  jQuery("body").keyup(function(e) {
    if(!this.camera.isSelected()) {
      return;
    }
    if(e.keyCode === 68) { // D
      this.isDKeyPressed = false;
      this.resetNavigationDiagram();
    }
    if(e.keyCode === 83) { // S
      this.isSKeyPressed = false;
    }
  }.bind(this));
};


DiagramCanvas.prototype.updateSize = function() {
  jQuery(this.canvas).css('width', (jQuery(this.mainDiv).width() - 44) + "px");
  jQuery(this.canvas).css('height', (jQuery(this.mainDiv).height() - 18) + "px");

  this.ctx.canvas.width = jQuery(this.canvas).width();
  this.ctx.canvas.height = jQuery(this.canvas).height();
};

DiagramCanvas.prototype.setInitPositionAndScale = function() {
  var scaleX, scaleY, diffWidth, diffHeight, initScale;

  scaleX = 1 / ((this.ctx.canvas.width - 30) / this.diagramInfo.size.w);
  scaleY = 1 / (this.ctx.canvas.height / this.diagramInfo.size.h);
  initScale = scaleX;
  if(scaleY > scaleX) {
    initScale = scaleY;
  }
  if(1 / initScale > 1) {
    initScale = 1;
  }

  this.camera.scale = initScale * this.camera.renderScale;
  diffWidth = this.ctx.canvas.width - this.diagramInfo.size.w * (1 / initScale);
  diffHeight = this.ctx.canvas.height - this.diagramInfo.size.h * (1 / initScale);

  this.camera.translate.set(diffWidth / 2, diffHeight / 2);
  this.updateSearchBoxPosition();
};


DiagramCanvas.prototype.getGhostShape = function(shape) {
  var scale, shapeGhost;
  scale = 1 / this.camera.scale * this.camera.renderScale;
  shapeGhost = {};
  shapeGhost.w = shape.shape.w * (scale);
  shapeGhost.h = shape.shape.h * (scale);
  shapeGhost.x = (shape.shape.x * (scale)) + this.camera.translate.x;
  shapeGhost.y = (shape.shape.y * (scale)) + this.camera.translate.y;
  shapeGhost.shape = shape;
  return shapeGhost;
};


DiagramCanvas.prototype.isMouseOnAShape = function() {
  var shapeGhost, scale;
  shapeGhost = null;

  jQuery.each(this.mainDiagramContext.reverseShapes, function(i, shape) {
    shapeGhost = this.getGhostShape(shape);
    if(this.pointInBox(this.camera.mousePosition, shapeGhost) === true) {
      if (cwAPI.compareShapes(shapeGhost, this.selectedShape) != true){
        cwAPI.hideInfoAboutObject(shapeGhost, this.selectedShape);
      }
      cwAPI.displayInfoAboutObject(shapeGhost.shape.shape.cwObject);
      return false;
    } else {
      shapeGhost = null;
    }
  }.bind(this));

  this.selectedShape = shapeGhost;
  return shapeGhost;
};

DiagramCanvas.prototype.getClickableRegionsOnShape = function(shapeGhost){
  var regions = shapeGhost.shape.paletteEntry.regions;
  regions = jQuery.grep(regions, function(item, index) {
    return(item.type === "explosion" || item.type === "association" || item.type === "navigation");
  });
  return regions;
};

DiagramCanvas.prototype.getBoxFromRegion = function(shapeGhost, region){
  var smallBox = {
    "x": region.x * ratioMMToCanvasSize,
    "y": region.y * ratioMMToCanvasSize,
    "w": region.w * ratioMMToCanvasSize,
    "h": region.h * ratioMMToCanvasSize
  };
  if(region.topType === "%") {
    smallBox.y = shapeGhost.shape.shape.h * region.y / 100;
  } else {}
  if(region.leftType === "%") {
    smallBox.x = shapeGhost.shape.shape.w * region.x / 100;
  }
  if(region.widthType === "%") {
    smallBox.w = shapeGhost.shape.shape.w * region.w / 100;
  }
  if (region.heightType === "%"){
    smallBox.h = shapeGhost.shape.shape.h * region.h / 100;
  }
  if (region.widthType === "fill"){
    smallBox.w = shapeGhost.shape.shape.w - (smallBox.x);
  }
  if (region.heightType === "fill"){
    smallBox.h = shapeGhost.shape.shape.h - (smallBox.y);
  }

  switch(region.anchor){
    case "top":
      smallBox.y = 0;
      break;
    case "right":
      smallBox.x = shapeGhost.shape.shape.w - smallBox.w; 
      break;
    case "bottom":
      smallBox.y = shapeGhost.shape.shape.h - smallBox.h;
      break;
    case "left":
      smallBox.x = 0;
      break;
    default:
      break;
  }

  smallBox.x = smallBox.x * 1 / this.camera.scale;
  smallBox.y = smallBox.y * 1 / this.camera.scale;
  smallBox.w = smallBox.w * 1 / this.camera.scale;
  smallBox.h = smallBox.h * 1 / this.camera.scale;

  
  var mouse = {
    "x": this.camera.mousePosition.x,
    "y": this.camera.scaledCanvasMousePosition.y
  };
  var box = {
    "x": shapeGhost.x + smallBox.x,
    "y": shapeGhost.y + smallBox.y,
    "w": smallBox.w,
    "h": smallBox.h
  };
  return box;
};

DiagramCanvas.prototype.mouseIsInShapeOnClick = function(shapeGhost, event) {
  // if click on exploded region, go to target page !
  if (!_.isUndefined(shapeGhost.shape.paletteEntry)){
    var regions = this.getClickableRegionsOnShape(shapeGhost);
    if(!_.isUndefined(regions) && regions.length != 0) {
      this.strokeShape(shapeGhost, "#004488", event);
      for(var i=0; i<regions.length; i++){
        var region = regions[i];
        var box = this.getBoxFromRegion(shapeGhost, region);

        if(this.pointInBox(this.camera.mousePosition, box)) {
          switch (region.type){
            case 'explosion':
              var diagramIdsToOpen = new Array();
              jQuery.each(shapeGhost.shape.shape.cwObject.associations.diagramExploded, function(index, _diagram){
                diagramIdsToOpen.push({name:_diagram.name, id:_diagram.object_id});
              });
              cwAPI.openDiagramPageWhenClickingOnDiagramShape(this, diagramIdsToOpen);
              break;
            case 'association':
            case 'navigation':
              var objectsToOpen = new Array();
              jQuery.each(shapeGhost.shape.shape.cwObject.associations[region.associationType], function(index, _target){
                objectsToOpen.push({name:_target.name, id:_target.object_id});
              });
              cwAPI.onAssociationRegionClickingHandler(this, shapeGhost.shape.shape.cwObject.objectTypeScriptName, shapeGhost.shape.shape.cwObject.object_id, shapeGhost.shape.shape.cwObject.associationsTargetObjectTypes[region.associationType].targetScriptName, objectsToOpen);
              break;
            default:
              break;
          }       
          return;
        }
      }
    }

  }
};

DiagramCanvas.prototype.mouseIsOnAClickableRegion = function(shapeGhost){
  if (!_.isUndefined(shapeGhost.shape.paletteEntry)){
    var regions = this.getClickableRegionsOnShape(shapeGhost);
    if(!_.isUndefined(regions) && regions.length != 0) {
      for(var i=0; i<regions.length; i++){
        var region = regions[i];
        var box = this.getBoxFromRegion(shapeGhost, region);
        if(this.pointInBox(this.camera.mousePosition, box)) {
          try{
            switch (region.type){
              case 'explosion':
                if (shapeGhost.shape.shape.cwObject.associations.diagramExploded.length > 0){
                  return true;
                }
                break;
              case 'association':
              case 'navigation':
                if (shapeGhost.shape.shape.cwObject.associations[region.associationType].length > 0){
                  return true;  
                }
                break;
              default:
              break;
            }
          }
          catch(err){
          }
        }
      }
    }
  }
  return false;
};

DiagramCanvas.prototype.strokeShape = function(shapeGhost, strokeColor, event) {
  var shape, symbol;
  shapeGhost.name = "NAME_NOT_SET";
  shape = new DiagramShape(shapeGhost, shapeGhost.shape.paletteEntry);
  var item = shapeGhost;
  item.paletteEntryStyle = shape.paletteEntry;
  item.customSymbol = shape.symbol;
  symbol = shape.getSymbol(item);
  this.ctx.strokeStyle = strokeColor;
  this.ctx.lineWidth = 2;
  shape.drawSymbolPath(this.ctx, 1, item);
  this.ctx.stroke();
  this.ctx.lineWidth = 1;
};


DiagramCanvas.prototype.strokeShapeIfMouseIsOver = function(strokeColor, event) {
  var shapeGhost;
  shapeGhost = this.isMouseOnAShape();
  if(shapeGhost !== null) {
    this.strokeShape(shapeGhost, strokeColor, event);
  }
};

DiagramCanvas.prototype.mouseMove = function(e) {
  jQuery('#cw-diagramShape-info').css({'top':e.pageY+10,'left':e.pageX+10});

  var shapeGhost = this.isMouseOnAShape();
  if (shapeGhost !== null){
    if(this.mouseIsOnAClickableRegion(shapeGhost)){
      jQuery('#'+this.canvasID).css('cursor', 'pointer');  
    }
    else{
      jQuery('#'+this.canvasID).css('cursor', 'default');  
    }
  }
  else{
    jQuery('#'+this.canvasID).css('cursor', 'default');
  }

  if(!this.camera.isSelected()) {
    return;
  }
  if(this.isSKeyPressed) {
    this.strokeShapeIfMouseIsOver("#004488", e);
  }
  if(this.isDKeyPressed) {
    this.loadNavigationDiagramIfMouseIsOver();
    this.strokeDiagramContextIfMouseIsOver();
  }
};


DiagramCanvas.prototype.tick = function() {

  requestAnimFrame(this.tick.bind(this));


  this.shapeToolTipRemove();
  this.resetGhostDiagramPositionIfExists();
  if(cwAPI.isUnderIE9()) {
    this.tickIE();
  } else {
    this.tickNormal();
  }
};


DiagramCanvas.prototype.tickNormal = function() {
  this.camera.update();
  this.camera.debug();
  this.ctx.save();
  this.camera.clearContext(this.ctx);
  this.camera.transform(this.ctx);
  this.ctx.drawImage(this.mainDiagramContext.renderCanvas, 0, 0, this.diagramInfo.size.w, this.diagramInfo.size.h);
  this.ctx.restore();
  this.drawNavigationDiagrams();
};


DiagramCanvas.prototype.tickIE = function() {
  if (this.image){
    this.camera.clearContext(this.ctx);
    this.camera.update();
    this.camera.debug();
  
    this.ctx.save();
    this.camera.transform(this.ctx);

    var searchValue = jQuery("#" + this.searchID).val();
    this.ctx.drawImage(this.image, 16, 16, this.diagramInfo.size.w - 32, this.diagramInfo.size.h - 32);
    this.ctx.restore();
  }
};


DiagramCanvas.prototype.pointInBox = function(point, box) {
  if(point.x < box.x) {
    return false;
  }
  if(point.x > box.x + box.w) {
    return false;
  }
  if(point.y < box.y) {
    return false;
  }
  if(point.y > box.y + box.h) {
    return false;
  }
  return true;
};