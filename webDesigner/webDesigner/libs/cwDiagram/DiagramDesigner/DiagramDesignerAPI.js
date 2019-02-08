
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