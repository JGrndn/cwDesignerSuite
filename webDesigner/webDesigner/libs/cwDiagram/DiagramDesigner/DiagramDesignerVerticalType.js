/*global DiagramDesigner:true */

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