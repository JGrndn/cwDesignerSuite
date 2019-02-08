/*global DiagramJoiner :true, DiagramShape : true, cwAPI:true, G_vmlCanvasManager:true*/


var DiagramContext = function(jsonDiagramFile) {
    //console.log("jsonDiagramFile", jsonDiagramFile);
    this.diagramInfo = jsonDiagramFile.diagram;
    this.objectTypesStyles = jsonDiagramFile.objectTypesStyles;

    this.loadShapesFromJSON(jsonDiagramFile);
    this.loadJoinersFromJSON(jsonDiagramFile);
    this.setupPictures();
  };

  DiagramContext.prototype.setupPictures = function () {
      var picturePath, pIDLink;
      this.pictures = {};
      //console.log(this.mainDiagramContext);
        _.each(this.diagramShapes, function(dShape) {
          if (dShape.shape.objectTypeName === 'PICTURE') {
            picturePath = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/' + dShape.shape.link + '.png';
            this.pictures[dShape.shape.link] = new Image();
            this.pictures[dShape.shape.link].src = picturePath;
          }
         if (dShape.paletteEntry != null && !_.isUndefined(dShape.paletteEntry.pictureID) && dShape.paletteEntry.pictureID !== 0) {
           pIDLink = 'picture' + dShape.paletteEntry.pictureID;
           picturePath = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/' + pIDLink + '.png';
           this.pictures[pIDLink] = new Image();
           this.pictures[pIDLink].src = picturePath;
         }
         if (!_.isUndefined(dShape.shape.customPicture)){
          picturePath = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/picture' + dShape.shape.customPicture + '.png';
            this.pictures['picture'+dShape.shape.customPicture] = new Image();
            this.pictures['picture'+dShape.shape.customPicture].src = picturePath;
         }
        }.bind(this));

       _.each(this.objectTypesStyles, function(pe) {
         if (!_.isUndefined(pe.regions)) {
           _.each(pe.regions, function(r) {
              if (!_.isUndefined(r.picturesId) && r.picturesId.length > 0){
                _.each(r.picturesId, function(id){
                  if (id !== 0){
                    pIDLink = 'picture' + id;
                    picturePath = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/' + pIDLink + '.png';
                    this.pictures[pIDLink] = new Image();
                    this.pictures[pIDLink].src = picturePath;
                  }
                }.bind(this));
              }
              else if (!_.isUndefined(r.pictureID) && r.pictureID !== 0) {
                pIDLink = 'picture' + r.pictureID;
                picturePath = cwConfigs.SITE_MEDIA_PATH + 'images/pictures/' + pIDLink + '.png';
                this.pictures[pIDLink] = new Image();
                this.pictures[pIDLink].src = picturePath;
             }
           }.bind(this));
         }
       }.bind(this));
  };

DiagramContext.prototype.loadShapesFromJSON = function(jsonDiagramFile) {
  var diagramShape;
  this.diagramShapes = [];
  this.reverseShapes = [];
  this.shapesByLinkID = {};
  _.each(jsonDiagramFile.shapes, function(shape) {
    diagramShape = new DiagramShape(shape, this.getStyleForItem(shape));
    this.diagramShapes.push(diagramShape);
    this.reverseShapes.push(diagramShape);
    this.shapesByLinkID[shape.link] = diagramShape;
  }.bind(this));
  this.reverseShapes.reverse();
};

DiagramContext.prototype.loadJoinersFromJSON = function(jsonDiagramFile) {
  this.joiners = [];
  _.each(jsonDiagramFile.joiners, function(j) {
    this.joiners.push(new DiagramJoiner(j, this.getStyleForItem(j)));
  }.bind(this));
};

DiagramContext.prototype.allPicturesHaveBeenLoaded = function() {
  var loaded = true;
  _.each(this.pictures, function(p) {
    if (p.complete === false) {
      loaded = false;
      return false;
    }
  });
  return loaded;
};

DiagramContext.prototype.drawElements = function(ctx, searchValue) {
  _.each(this.diagramShapes, function(shape) {
    shape.draw(ctx, searchValue, this);
  }.bind(this));
  _.each(this.joiners, function(joiner) {
    joiner.draw(ctx);
  }.bind(this));
};

DiagramContext.prototype.getStyleForItem = function(_item) {
  var objectPaletteEntryKey = _item.objectPaletteEntryKey;
  if (!_.isUndefined(objectPaletteEntryKey) && objectPaletteEntryKey) {
    if (!_.isUndefined(this.objectTypesStyles[objectPaletteEntryKey])) {
      return this.objectTypesStyles[objectPaletteEntryKey];
    } else {
      return this.objectTypesStyles[_item.objectTypeName + '|0'];
    }
  }
  return null;
};


DiagramContext.maxCanvasSize = 6000;
DiagramContext.prototype.setContextSize = function(renderScale) {
  var w, h, scaleX, scaleY, initScale, maxCanvasSize;
  maxCanvasSize = DiagramContext.maxCanvasSize;
  if (this.diagramInfo.size.w > maxCanvasSize) {
    maxCanvasSize = this.diagramInfo.size.w;
  }
  if (this.diagramInfo.size.h > maxCanvasSize) {
    maxCanvasSize = this.diagramInfo.size.h;
  }
  //console.log(this.diagramInfo.size, maxCanvasSize);
  w = this.diagramInfo.size.w * renderScale;
  h = this.diagramInfo.size.h * renderScale;
  scaleX = maxCanvasSize / w;
  scaleY = maxCanvasSize / h;
  initScale = scaleX;
  if (scaleY < scaleX) {
    initScale = scaleY;
  }

  this.renderCanvas.width = w * initScale;
  this.renderCanvas.height = h * initScale;
  return initScale;
};

DiagramContext.prototype.createContext = function(searchValue, renderScale, callback) {
  var renderContext;

  if (cwAPI.isUnderIE9()) {
    return;
  }

  if (!this.allPicturesHaveBeenLoaded()) {
    setTimeout(function() {
      this.createContext(searchValue, renderScale, callback);
    }.bind(this), 50);
  } else {
    this.renderCanvas = document.createElement('canvas');
    if (cwAPI.isUnderIE9()) {
      G_vmlCanvasManager.initElement(this.renderCanvas);
      //this.renderCanvas = initCanvas(this.renderCanvas);

    }
    renderContext = this.renderCanvas.getContext('2d');
    //console.log(this.diagramInfo.size.w * renderScale, this.diagramInfo.size.h * renderScale);
    renderScale = this.setContextSize(renderScale);

    //console.log(this.renderCanvas.width, this.renderCanvas.height);
    renderContext = this.renderCanvas.getContext('2d');
    renderContext.clearRect(0, 0, this.renderCanvas.width, this.renderCanvas.height);
/*      renderContext.lineWidth = 1;
      renderContext.fillStyle = "#FF0000";
      renderContext.strokeRect(0, 0, this.renderCanvas.width, this.renderCanvas.height);
      renderContext.fillRect(0, 0, 200, 200);*/
    renderContext.save();
    renderContext.scale(renderScale, renderScale);
    this.drawElements(renderContext, searchValue);
    renderContext.restore();
    return callback(this);
  }
};