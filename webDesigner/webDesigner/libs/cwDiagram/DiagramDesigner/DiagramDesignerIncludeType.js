/*global DiagramDesigner:true */


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
    if (paletteEntryResult === null) return;
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
