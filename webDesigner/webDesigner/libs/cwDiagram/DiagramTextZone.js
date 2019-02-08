var DiagramTextZone = {};

DiagramTextZone.getTextZone16 = function(shape){
  return {
    x : shape.x + shape.w * 1/8,
    y : shape.y,
    w : shape.w * 6/8,
    h : shape.h
  };
};

DiagramTextZone.getTextZone254 = function(shape){
  return {
    x : shape.x,
    y : shape.y + shape.h * 1/8,
    w : shape.w,
    h : shape.h * 6/8
  };
};