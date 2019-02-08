/*global DiagramCanvas:true, cwPoint:true, cwConfigs :true, cwAPI:true */

var DiagramDesignerAPI = {};
DiagramDesignerAPI.diagramDesignerCreateVerticalNode = function (objectsKey, num, side, translateX, translateY, shapeSpaceY, setJoiner, children) {
  return {
    "objectsKey": objectsKey,
    "num": num,
    "translate": {
      "x": translateX,
      "y": translateY
    },
    "shapeSpace": {
      "y": shapeSpaceY
    },
    "children": children,
    "setJoiner": setJoiner,
    "side": side,
    "design": "vertical"
  };
};

DiagramDesignerAPI.diagramDesignerCreateIncludeNode = function (objectsKey, num, maxColumn, paddingTop, paddingBottom, paddingLeft, paddingRight, shapeSpaceX, shapeSpaceY, children) {
  return {
    "objectsKey": objectsKey,
    "num": num,
    "maxColumn": maxColumn,
    "padding": {
      "top": paddingTop,
      "bottom": paddingBottom,
      "left": paddingLeft,
      "right": paddingRight
    },
    "shapeSpace": {
      "x": shapeSpaceX,
      "y": shapeSpaceY
    },
    "children": children,
    "design": "include"
  };
};

var DiagramDesigner = function (all_items, level0, selectorID, templateAbbreviation, parentSelector, name, height, callback) {
  //console.log(parentSelector);
  this.height = height;
  var templateURL = cwConfigs.SITE_MEDIA_PATH + 'webdesigner/generated/diagram/json/template' + templateAbbreviation + '.' + cwConfigs.JSON_EXTENTION;
  this.all_items = all_items;
  this.parentSelector = parentSelector;
  this.level0 = level0;

  this.hasShapes = false;
  this.json = {};
  this.json.diagram = {};
  this.json.diagram.object_id = Math.random();
  this.json.diagram.name = name;
  this.json.diagram.size = {
    "w": 100,
    "h": 100
  };

  this.mostTopLef = new cwPoint(0, 0);

  this.json.shapes = [];
  this.json.joiners = [];

  this.selectorID = selectorID;
  this.loadTemplate(templateURL, function () {
    this.setupDiagram();
    if (!_.isUndefined(callback)) {
      return callback(null, this);
    }
  }.bind(this));
};

DiagramDesigner.prototype.clean = function () {
  jQuery('#' + this.selectorID).remove();
};

DiagramDesigner.prototype.setupDiagram = function () {
  this.doDesign(this.all_items);
  if (this.json.shapes.length > 0) {
    this.createCanvas();
    this.hasShapes = true;
  }
};


DiagramDesigner.prototype.createCanvas = function () {
  var dID, el, d;
  //  console.log('shapes', this.json.shapes);
  dID = Math.floor(Math.random() * 9999999999999);
  //console.log(dID);
  jQuery("#" + this.parentSelector).append('<div id="' + this.selectorID + '" class="cw-diagram-designer-zone" data-designid="' + dID + '"></div>');
  el = document.createElement('canvas');

  // if (cwAPI.isUnderIE9()) {
  //   G_vmlCanvasManager.initElement(el); 
  // }

  el.id = this.selectorID + '-canvas';
  jQuery('#' + this.selectorID).css('min-height', this.height);
  jQuery('#' + this.selectorID).append(el);
  jQuery('#' + el.id).addClass("cw-diagram-canvas");
  this.diagramCanvas = new DiagramCanvas(dID, this.json, this.selectorID);
  jQuery('body').data('design' + dID, this);
  this.diagramCanvas.setInitPositionAndScale();

};


DiagramDesigner.prototype.doDesign = function (all_items) {
  var parentLevel, parentShape, shapes;

  parentLevel = DiagramDesignerAPI.diagramDesignerCreateIncludeNode("", "", 0, 0, 0, 0, 0, 0, 30, 30, []);
  parentShape = {
    "x": 0,
    "y": 0
  };

  shapes = this.doDefinedDesign(all_items, parentShape, parentLevel, this.level0);
  //  console.log("shapes", shapes.length);
  _.each(shapes, function (shape) {
    this.json.shapes.push(shape);
  }.bind(this));


  this.updateDiagramSizeAfterShapesCreation();
  this.translateShapesAndJoinerIfRequired();
  //console.log("shapes2", this.json.shapes.length);
};


DiagramDesigner.prototype.translateShapesAndJoinerIfRequired = function () {
  // translate the shapes & joiners if required
  if (this.mostTopLef.x <= 0 && this.mostTopLef.y <= 0) {

    jQuery.each(this.json.shapes, function (i, shape) {
      shape.x += -this.mostTopLef.x;
      shape.y += -this.mostTopLef.y;
    }.bind(this));

    _.each(this.json.joiners, function (joiner) {
      _.each(joiner.points, function (point) {
        point.x += -this.mostTopLef.x;
        point.y += -this.mostTopLef.y;
      }.bind(this));
    }.bind(this));

  }

};

DiagramDesigner.prototype.updateDiagramSizeAfterShapesCreation = function () {

  if (this.mostTopLef.x < 0) {
    this.json.diagram.size.w += -this.mostTopLef.x;
  }

  if (this.mostTopLef.y < 0) {
    this.json.diagram.size.h += -this.mostTopLef.y;
  }
};

DiagramDesigner.prototype.doDefinedDesign = function (item, parentShape, parentLevel, level) {
  //  console.log(level);
  var shapes, items;
  shapes = [];
  items = item.associations[level.objectsKey];
  if (_.isUndefined(items)) {
    return [];
  }
  if (items.length === 0) {
    return [];
  }
  switch (level.design) {
  case "vertical":
    shapes = this.createShapesVertical(items, level, parentShape, parentLevel);
    break;
  case "include":
    shapes = this.createShapesInclude(items, level, parentShape, parentLevel);
    break;
  }
  return shapes;
};

DiagramDesigner.prototype.getPaletteEntryForShape = function (shape) {
  var typeID, scriptName;
  typeID = shape.properties.type_id;
  scriptName = shape.objectTypeScriptName.toUpperCase();
  // if the script name don't exists in the palette
  if (_.isUndefined(this.json.paletteEntryKeyByOTAndTypeID[scriptName])) {
    //fail
    return null;
  } else {
    // if the exact don't category exists
    if (_.isUndefined(this.json.paletteEntryKeyByOTAndTypeID[scriptName][typeID])) {
      // if the default palette entry don't exists
      if (_.isUndefined(this.json.paletteEntryKeyByOTAndTypeID[scriptName]["0"])) {
        // fail
        return null;
      } else {
        // get default palette entry
        return {
          "name": scriptName + "|0",
          "pe": this.json.paletteEntryKeyByOTAndTypeID[scriptName]["0"]
        };
      }
    } else {
      return {
        "name": scriptName + "|" + typeID,
        "pe": this.json.paletteEntryKeyByOTAndTypeID[scriptName][typeID]
      };
    }

  }

};

DiagramDesigner.prototype.loadTemplate = function (url, callback) {
  cwAPI.getJSONFile(url, function (JSONTemplateData) {
    var paletteEntryKey, paletteEntryKeyList, pekObjectType, pekTypeID;


    this.json.objectTypesStyles = JSONTemplateData.objectTypesStyles;
    this.json.paletteEntryKeyByOTAndTypeID = {};
    for (paletteEntryKey in this.json.objectTypesStyles) {
      if (this.json.objectTypesStyles.hasOwnProperty(paletteEntryKey)) {

        paletteEntryKeyList = paletteEntryKey.split('|');
        pekObjectType = paletteEntryKeyList[0];
        pekTypeID = paletteEntryKeyList[1];
        if (_.isUndefined(this.json.paletteEntryKeyByOTAndTypeID[pekObjectType])) {
          this.json.paletteEntryKeyByOTAndTypeID[pekObjectType] = {};
        }
        if (_.isUndefined(this.json.paletteEntryKeyByOTAndTypeID[pekObjectType][pekTypeID])) {
          this.json.paletteEntryKeyByOTAndTypeID[pekObjectType][pekTypeID] = this.json.objectTypesStyles[paletteEntryKey];
        }
      }
    }
    callback();
  }.bind(this), cwAPI.errorOnLoadPage);
};


DiagramDesigner.prototype.createShapesVertical = function (objects, level, parentShape, parentLevel) {
  var shapes, paletteEntry, paletteEntryResult, lastShape, x, y, shape;

  shapes = [];

  lastShape = null;
  jQuery.each(objects, function (shapeIndex, item) {

    paletteEntryResult = this.getPaletteEntryForShape(item);
    paletteEntry = paletteEntryResult.pe;


    shape = this.createShape(item, paletteEntryResult, level);
    if (lastShape !== null) {
      shape.x = lastShape.x;
      shape.y = lastShape.y + lastShape.h + level.shapeSpace.y;
    } else {

      switch (level.side) {
      case "left":
        shape.x = parentShape.x - level.translate.x - paletteEntry.defaultWidth;
        break;
      case "right":
        shape.x = parentShape.x + level.translate.x + paletteEntry.defaultWidth;
        break;
      }
      shape.y = parentShape.y + level.translate.y;

    }
    shapes.push(shape);
    this.updateParentSize(level, parentLevel, shape, parentShape);

    // set joiners
    if (level.setJoiner) {
      var joiner = {
        "objectPaletteEntryKey": "CONNECTOR|0",
        "points": []
      };
      switch (level.side) {
      case "left":
        joiner.points.push({
          "x": shape.x + shape.w,
          "y": shape.y + shape.h / 2
        });
        joiner.points.push({
          "x": parentShape.x,
          "y": shape.y + shape.h / 2
        });
        break;
      case "right":
        joiner.points.push({
          "x": parentShape.x + parentShape.w,
          "y": shape.y + shape.h / 2
        });
        joiner.points.push({
          "x": shape.x,
          "y": shape.y + shape.h / 2
        });
        break;
      }
      this.json.joiners.push(joiner);
    }

    lastShape = shape;
    this.updateDiagramSize(shape);

    if (shape.x < this.mostTopLef.x) {
      this.mostTopLef.x = shape.x;
    }
    if (shape.y < this.mostTopLef.y) {
      this.mostTopLef.y = shape.y;
    }
  }.bind(this));

  return shapes;
};



DiagramDesigner.prototype.createShape = function (item, paletteEntryResult, level) {
  var shape = {
    "w": paletteEntryResult.pe.defaultWidth,
    "h": paletteEntryResult.pe.defaultHeight,
    "objectPaletteEntryKey": paletteEntryResult.name
  };
  shape.name = item.name;
  shape.objectTypeName = item.objectTypeScriptName.toUpperCase();
  shape.cwObject = {};
  shape.objectID = item.object_id;
  shape.cwObject.properties = item.properties;
  return shape;
};

DiagramDesigner.prototype.createShapesInclude = function (objects, level, parentShape, parentLevel) {
  var shapes, x, y, maxColumn, shape, row, col, paletteEntry, paletteEntryResult, lastShape, maxRowHeight;

  shapes = [];

  //paletteEntry = this.json.objectTypesStyles[level.paletteEntryKey];
  x = parentShape.x;
  y = parentShape.y;
  maxColumn = level.maxColumn;

  lastShape = null;
  maxRowHeight = 0; //paletteEntry.defaultHeight;
  jQuery.each(objects, function (shapeIndex, item) {
    paletteEntryResult = this.getPaletteEntryForShape(item);
    paletteEntry = paletteEntryResult.pe;
    //    console.log(paletteEntry);

    if (paletteEntry === null) {
      return;
    }

    shape = this.createShape(item, paletteEntryResult, level);

    row = Math.floor(shapeIndex / maxColumn);
    col = shapeIndex % maxColumn;


    if (col === 0) {
      //maxRowHeight = shape.h;
      if (lastShape !== null) {
        
        shape.x = parentShape.x + parentLevel.padding.left;
        shape.y = lastShape.y + maxRowHeight + level.shapeSpace.y;
        maxRowHeight = 0;
      } else { // first shape
        shape.x = parentShape.x + parentLevel.padding.left;
        shape.y = parentShape.y + parentLevel.padding.top;
      }
    } else {
      if (lastShape !== null) {
        shape.x = lastShape.x + lastShape.w + level.shapeSpace.x;
        shape.y = lastShape.y;
      } else {
        shape.x = col * paletteEntry.defaultWidth + parentShape.x;
        shape.y = row * paletteEntry.defaultHeight + parentShape.y;
      }
    }

    shapes.push(shape);



    _.each(level.children, function (childrenLevel) {
      var childShapes = this.doDefinedDesign(item, shape, level, childrenLevel);
      shapes = shapes.concat(childShapes);
    }.bind(this));

    this.updateParentSize(level, parentLevel, shape, parentShape);

    //console.log(o);
    /*    if (!_.isUndefined(item.b)) {
      //var childShapes = this.createShapesInclude(item.b, "B|0", shape, level + 1);
      //console.log('childShapes', childShapes);
      //shapes = shapes.concat(childShapes);
    }*/
    

    if (maxRowHeight < shape.h) {
      maxRowHeight = shape.h;
    }
    lastShape = shape;

    this.updateDiagramSize(shape);
  }.bind(this));

  return shapes;
};



DiagramDesigner.prototype.updateParentSize = function (level, parentLevel, shape, parentShape) {
  switch (level.design) {
  case "include":
    // RESIZE PARENT
    if (level.num > 0) {
      if (shape.x + shape.w + parentLevel.padding.right > parentShape.x + parentShape.w) {
        parentShape.w = shape.x + shape.w - parentShape.x + parentLevel.padding.right;
      }
      if (shape.y + shape.h + parentLevel.padding.bottom > parentShape.y + parentShape.h) {
        parentShape.h = shape.y + shape.h - parentShape.y + parentLevel.padding.bottom;
      }
    }
    break;
  case "vertical":

    if (shape.y + shape.h > parentShape.y + parentShape.h) {
      parentShape.h = shape.y + shape.h - parentShape.y;
    }

    break;
  }


};


DiagramDesigner.prototype.updateDiagramSize = function (shape) {
  if (shape.x + shape.w > this.json.diagram.size.w) {
    this.json.diagram.size.w = shape.x + shape.w;
  }
  if (shape.y + shape.h > this.json.diagram.size.h) {
    this.json.diagram.size.h = shape.y + shape.h;
  }
};
