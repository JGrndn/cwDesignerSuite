/*global cwAPI : true */

// Magic number for padding inside boxes
// ratio mieux adapté a priori
var ratioMMToCanvasSize = 8192 / 2180;


var DiagramShape = function(shape, paletteEntry) {
  this.shape = shape;
  this.shape.name = cwAPI.cwSearchEngine.removeSearchEngineZone(this.shape.name);
  this.paletteEntry = paletteEntry;
};


DiagramShape.prototype.drawSymbolPath = function(ctx, lineWidth, item) {
  var symbol, drawEngine, shapeCoord;

  shapeCoord = {
    'x': item.x + lineWidth,
    'y': item.y + lineWidth,
    'w': item.w - lineWidth * 2,
    'h': item.h - lineWidth * 2
  };

  symbol = this.getSymbol(item);
  drawEngine = new DrawEngine(ctx);
  if (_.isUndefined(symbol)) {
    symbol = 0;
  }
  var f = drawEngine['drawSymbol'+symbol];
  if (!_.isUndefined(f)){
    f(ctx, shapeCoord.x, shapeCoord.y, shapeCoord.w, shapeCoord.h);
  }
  else{
    drawEngine.drawSymbol0(ctx, shapeCoord.x, shapeCoord.y, shapeCoord.w, shapeCoord.h);
  }
};


DiagramShape.prototype.getSymbol = function(item) {
  if (_.isUndefined(this.shape)){
    return 0; // for joiners
  }
  var symbol;
  if (!_.isUndefined(item.paletteEntry) && item.paletteEntry) { // generic style
    symbol = this.paletteEntry.symbol;
  }
  // custom symbol
  if (!_.isUndefined(this.shape.customSymbol)) {
    symbol = this.shape.customSymbol;
  }
  return symbol;
};


DiagramShape.prototype.getGradientStyle = function(ctx, usedStyle) {
  var lingrad;
  var gradSize = usedStyle.gradientSize / 100;

  switch (usedStyle.gradientDir) {
    case 0:
    // horizontal
      lingrad = ctx.createLinearGradient(this.shape.x, this.shape.y, this.shape.x + this.shape.w, this.shape.y);
      break;
    case 1:
      // vertical
      lingrad = ctx.createLinearGradient(this.shape.x, this.shape.y, this.shape.x, this.shape.y + this.shape.h);
      break;
    case 2:
      //"ForwardDiagonal"
      lingrad = ctx.createLinearGradient(this.shape.x, this.shape.y, this.shape.x + this.shape.w, this.shape.y + this.shape.h);
      break;
    case 3:
      //"Backward diagonal"
      lingrad = ctx.createLinearGradient(this.shape.x + this.shape.w, this.shape.y, this.shape.x, this.shape.y + this.shape.h);
      break;
    default:
      lingrad = ctx.createLinearGradient(this.shape.x, this.shape.y, this.shape.x + this.shape.w, this.shape.y + this.shape.h);
      break;
  }

  var startPoint = 0;
  var endPoint = 1;
  if (gradSize > 0.5){
    startPoint = 2 * gradSize - endPoint;
  }
  else if (gradSize < 0.5){
    endPoint = 2 * gradSize - startPoint;
  }

  lingrad.addColorStop(startPoint, usedStyle.gradientFromFill);
  lingrad.addColorStop(endPoint, usedStyle.gradientToFill);
  return lingrad;
};


DiagramShape.prototype.drawSymbol = function(ctx, diagramContext, item) {
  var drawEngine, usedStyle, pIDLink, lineWidth;

  if (this.paletteEntry != null && !_.isUndefined(this.paletteEntry.pictureID) && this.paletteEntry.pictureID !== 0) {
    pIDLink = 'picture' + this.paletteEntry.pictureID;
    ctx.drawImage(diagramContext.pictures[pIDLink], item.x, item.y, item.w, item.h);
    return;
  }

  if (!_.isUndefined(this.shape.customPicture)){
    ctx.drawImage(diagramContext.pictures['picture' + this.shape.customPicture], item.x, item.y, item.w, item.h);
    return;
  }

  if (this.shape.objectTypeName === 'PICTURE') {
    ctx.drawImage(diagramContext.pictures[this.shape.link], item.x, item.y, item.w, item.h);
    return;
  }

  if (!_.isUndefined(item.customStyle) && item.customStyle) { // custom styles
    usedStyle = item.customStyle;
  } else if (!_.isUndefined(item.paletteEntryStyle) && item.paletteEntryStyle) { // generic style
    usedStyle = item.paletteEntryStyle;
  }

  //  console.log(usedStyle);
  if (_.isUndefined(usedStyle)) {
    usedStyle = {};
    usedStyle.fillColor = '#FFFF80';
    usedStyle.strokeColor = '#000000';
    usedStyle.lineWidth = 1;
    usedStyle.hasGradient = false;
    usedStyle.hasShadow = false;
  }

  if (usedStyle.hasOpacity) {
    ctx.globalAlpha *= usedStyle.opacityPercentage / 100;
  }

  ctx.strokeStyle = usedStyle.strokeColor;
  lineWidth = usedStyle.lineWidth / ratioMMToCanvasSize;

  if (usedStyle.hasGradient === true) {
    ctx.fillStyle = this.getGradientStyle(ctx, usedStyle);
  } else {
    ctx.fillStyle = usedStyle.fillColor;
  }

  if (usedStyle.hasShadow === true) {
    ctx.shadowColor = 'black';
    ctx.shadowBlur = 10;
    ctx.shadowOffsetX = 5;
    ctx.shadowOffsetY = 5;
  }

  if (cwAPI.isUndefined(usedStyle.fillPattern) || usedStyle.fillPattern !== 'Transparent') {
    //console.log("usedStyle", usedStyle);
    this.drawSymbolPath(ctx, lineWidth, item);
    ctx.fill();
  }

  ctx.lineWidth = lineWidth;
  ctx.shadowBlur = 0;
  ctx.shadowOffsetX = 0;
  ctx.shadowOffsetY = 0;

  if (cwAPI.isUndefined(usedStyle.strokePattern) || usedStyle.strokePattern !== '-1') {
    this.drawSymbolPath(ctx, lineWidth, item);
    ctx.stroke();
  }

  ctx.lineWidth = 1;
  if (usedStyle.hasOpacity) {
    ctx.globalAlpha /= usedStyle.opacityPercentage / 100;
  }
};


DiagramShape.prototype.getRegionSize = function(region) {
  var regionSize = {};
  regionSize.x = this.shape.x + region.x * ratioMMToCanvasSize;
  regionSize.y = this.shape.y + region.y * ratioMMToCanvasSize;
  regionSize.w = region.w * ratioMMToCanvasSize;
  regionSize.h = region.h * ratioMMToCanvasSize;
  if (region.leftType === '%') {
    regionSize.x = this.shape.x + (this.shape.w * (region.x / 100));
  }
  if (region.topType === '%') {
    regionSize.y = this.shape.y + this.shape.h * (region.y / 100);
  }
  if (region.widthType === '%') {
    regionSize.w = this.shape.w * (region.w / 100);
  }
  if (region.widthType === 'fill') {
    regionSize.w = this.shape.w - (regionSize.x - this.shape.x);
  }
  if (region.heightType === '%') {
    regionSize.h = this.shape.h * (region.h / 100);
  }
  if (region.heightType === 'fill') {
    regionSize.h = this.shape.h - (regionSize.y - this.shape.y);
  }

  // anchor ?
  switch(region.anchor){
    case "top":
      regionSize.y = region.y;
      break;
    case "right":
      regionSize.x = this.shape.x + this.shape.w - regionSize.w;
      break;
    case "bottom":
      regionSize.y = this.shape.y + this.shape.h - regionSize.h;
      break;
    case "left":
      regionSize.x = this.shape.x;
      break;
    default:
      break;
  }
  return regionSize;
};

DiagramShape.prototype.getRegionTextSize = function(regionSize, fontSize, item){
  var textSize = {};

  var symbol = this.getSymbol(item);
  if (!_.isUndefined(symbol)){
    var f = DiagramTextZone['getTextZone'+symbol];
    if (!_.isUndefined(f)){
      textSize = f(regionSize);
    }
  }
  if (_.isUndefined(textSize.x)){
    textSize.x = regionSize.x;
    textSize.y = regionSize.y;
    textSize.w = regionSize.w;
    textSize.h = regionSize.h;
  }
  textSize.fontSize = fontSize * 1.5;
  return textSize;
};

DiagramShape.prototype.getStyle = function(region){
  var isPropertyConditionValid = function(){
    var prop = (cwObject.properties.hasOwnProperty(region.propertyScriptName + '_id')) ? region.propertyScriptName + '_id' : region.propertyScriptName;
    switch(region.rule.operande){
      case 'Always':
        return true;
      case 'Set':
        return cwObject.properties[prop] != 0;
      case 'Equal':
        return cwObject.properties[prop] == region.rule.value;
      case 'NotSet':
        return cwObject.properties[prop] == 0;
      case 'LessThan':
        return cwObject.properties[prop] < region.rule.value;
      case 'LessThanOrEqual':
        return cwObject.properties[prop] <= region.rule.value;
      case 'GreaterThan':
        return cwObject.properties[prop] > region.rule.value;
      case 'GreaterThanOrEqual':
        return cwObject.properties[prop] >= region.rule.value;
      case 'NotEqual':
        return cwObject.properties[prop] != region.rule.value;
      case 'Contains':
        return cwObject.properties[prop].indexOf(region.rule.value) != -1;
      case 'NotContains':
        return cwObject.properties[prop].indexOf(region.rule.value) == -1;
      case 'IsBetween':
        return (cwObject.properties[prop] >= region.rule.minValue) && (cwObject.properties[prop] <= region.rule.maxValue);
      case 'NotBetween':
        return (cwObject.properties[prop] > region.rule.maxValue) || (cwObject.properties[prop] < region.rule.minValue);
      default:
        return false;
    }
  };

  var isAssociationConditionValid = function(){
    var ast = region.associationType;
    switch(region.rule.operande){
      case 'AssociationExists':
        return cwObject.associations[ast].length != 0; 
      case 'AssociationNotExists':
        return cwObject.associations[ast].length == 0; 
      default:
        return false;
/*
      rule non encore utilisée
        InRange = 13,
        NotInRange = 14,
        Exists = 15,
        NotExists = 16
      */
    }
  };

  if (region.rule != null){
    var cwObject = this.shape.cwObject;

    switch(region.type){
      case 'conditionalDisplay':
        if (isPropertyConditionValid()){
          return region.style;
        }
    
      case 'association':
        if (isAssociationConditionValid()){
          return region.style;
        }

      default:
        return;
    }
  }
  return region.style;
};

DiagramShape.prototype.drawRegions = function (ctx, diagramContext) {
    var regionSize, textSize;

    if (_.isUndefined(this.paletteEntry)){
        return;
    }
    _.each(this.paletteEntry.regions, function (region) {
        if (region.style === null){
          // style = par defaut (mais non exporté dans le JSON --> corrigé cela dans le c# ?)
          return;
        }
        if (region.type === 'none') {
            return;
        }
        if (region.type === 'conditionalDisplay' || region.type === 'label' || region.type === 'explosion' || region.type === 'property_value' || region.type === 'association') {
            var usedStyle = this.getStyle(region);
            if (!usedStyle){
              return;
            }

            regionSize = this.getRegionSize(region);

            //console.log(region.style.fillColor);
            var item = regionSize;
            item.paletteEntryStyle = usedStyle;
            item.customSymbol = region.symbol;
            textSize = this.getRegionTextSize(regionSize, usedStyle.font.size, item);

            if (!_.isUndefined(usedStyle.fillPattern) && usedStyle.fillPattern === 'Solid' && usedStyle.hasGradient === false) {
                ctx.fillStyle = usedStyle.fillColor;
                this.drawSymbol(ctx, diagramContext, item);
            } else if (usedStyle.hasGradient === true) {
                ctx.fillStyle = this.getGradientStyle(ctx, usedStyle);
                this.drawSymbol(ctx, diagramContext, item);
            }
            ctx.font = usedStyle.font.font;
            ctx.fillStyle = usedStyle.textColor;

            if (!_.isUndefined(usedStyle.textColor)) {
                ctx.fillStyle = usedStyle.textColor;
            }

            if (!_.isUndefined(region.pictureID) && region.pictureID !== 0) {
              var _drawRegion = false;
              pIDLink = 'picture' + region.pictureID;
              
                if (region.type === 'explosion') {
                    if (!_.isUndefined(this.shape.cwObject.associations.diagramExploded) && this.shape.cwObject.associations.diagramExploded.length != 0) {
                      if (this.shape.cwObject.associations.diagramExploded.length != 1 || this.shape.cwObject.associations.diagramExploded[0].object_id != diagramContext.diagramInfo.object_id){
                        _drawRegion = true;
                      }
                    }
                }
                if (region.type === 'label') {
                    _drawRegion = true;
                }
                if (region.type === 'conditionalDisplay'){
                  if (!_.isUndefined(this.shape.cwObject.properties) && !_.isUndefined(this.shape.cwObject.properties[region.propertyScriptName + '_id'])){
                    var propValue = this.shape.cwObject.properties[region.propertyScriptName + '_id'];
                    if (!_.isUndefined(region.picturesIdByPropertyValue) && !_.isUndefined(region.picturesIdByPropertyValue[propValue])){
                      pIDLink = 'picture' + region.picturesIdByPropertyValue[propValue];
                      _drawRegion = true;
                    }
                  }
                }
                if (region.type === 'association'){
                  if (this.shape.cwObject.associations[region.associationType].length > 0){
                    _drawRegion = true;
                  }
                }

                if (_drawRegion === true){
                  ctx.drawImage(diagramContext.pictures[pIDLink], regionSize.x, regionSize.y, regionSize.w, regionSize.h);
                }
            }

            var regionTextValue = "";
            switch (region.type) {
                case 'label':
                    regionTextValue = region.labelValue;
                    break;
                case 'property_value':
                    regionTextValue = this.shape.cwObject.properties[region.propertyScriptName];
                    break;
                case 'association':
                    if (this.shape.cwObject.associations[region.associationType].length > 0){
                      regionTextValue = this.shape.cwObject.associations[region.associationType][0].name;
                    }
                    break;
                default:
                    break;
            }
            this.wrapText(ctx, regionTextValue, textSize, region.horizontalAlignment, region.verticalAlignment, region);
        }
    } .bind(this));
};


DiagramShape.prototype.drawText = function(ctx, text, style) {
  //  var textRegionSize, verticalAlignment, horizontalAlignment;
  // draw custom text
  ctx.font = style.font.font; // "bold 12pt arial";//style.font;
  ctx.fillStyle = style.textColor;
  verticalAlignment = 'top';
  horizontalAlignment = 'left';
  if (!_.isUndefined(this.paletteEntry)) {
    horizontalAlignment = this.paletteEntry.horizontalAlignment;
    verticalAlignment = this.paletteEntry.verticalAlignment;
  }
  var textToDisplay = (!_.isUndefined(text) && text !== "") ? text : this.shape.name;
  textRegionSize = this.getRegionTextSize(this.shape, style.font.size, this);
  this.wrapText(ctx, textToDisplay, textRegionSize, horizontalAlignment, verticalAlignment);
};


DiagramShape.prototype.draw = function(ctx, searchValue, diagramContext) {
  var alpha, textColor, textStyle, item;
  ctx.fillStyle = '#000000';
  ctx.strokeStyle = '#000000';

  item = {
    'x': this.shape.x,
    'y': this.shape.y,
    'w': this.shape.w,
    'h': this.shape.h,
    'customStyle': this.shape.customStyle,
    'paletteEntryStyle': _.isUndefined(this.paletteEntry) ? null: this.paletteEntry.style,
    'paletteEntry' : this.paletteEntry
  }

  alpha = 1;
  if (!_.isUndefined(searchValue) && searchValue.length > 1 && this.shape.name.toLowerCase().indexOf(searchValue.toLowerCase()) === -1) {
    alpha = 0.125;
  }
  ctx.globalAlpha = alpha;


  this.drawSymbol(ctx, diagramContext, item);
  this.drawRegions(ctx, diagramContext);
  if (!_.isUndefined(this.paletteEntry) && this.paletteEntry && this.paletteEntry.displayText === true) {
    // use custom class if exists
    textStyle = this.paletteEntry.style;
    if (!_.isUndefined(this.shape.customStyle) && this.shape.customStyle) {
      textStyle = this.shape.customStyle;
    }
    //console.log("dp text", this.paletteEntry.displayText);
    this.drawText(ctx, this.shape.name, textStyle); //this.paletteEntry.displayText,
  } else if (this.shape.objectID === 0 && this.shape.name !== '' && !_.isUndefined(this.shape.customStyle)) {
    // it should be a title
    this.drawText(ctx, this.shape.name, this.shape.customStyle);
  }
};


DiagramShape.prototype.hasExplodedDiagram = function(){
  if (!_.isUndefined(this.shape.cwObject)){
    if (this.shape.cwObject.associations.diagramExploded.length > 0){
      return true;
    }
  }
  return false;
}



/**
 * Get the number of lines required by a text
 * @param  {2DContext} context [the canvas context].
 * @param  {string} text     [the text].
 * @param  {int} maxWidth   [maximum With for each line].
 * @param  {Array} lines    [add each line in lines array].
 * @return {int}          [number of lines].
 */
DiagramShape.prototype.getLinesNumberFromText = function(context, text, maxWidth, lines) {
  var words, line, num, n, testLine, metrics, testWidth, result;
  var lineWidth = 0;
  var w = 0;
  if (_.isUndefined(text)) {
    return;
  }
  words = text.split(' ');
  line = '';
  num = 1;
  for (n = 0; n < words.length; n += 1) {
    testLine = line + words[n] + ' ';
    metrics = context.measureText(testLine);
    testWidth = metrics.width;
    if (testWidth > maxWidth) {
      lines.push(line);
      line = words[n] + ' ';
      num += 1;
      lineWidth = Math.max(w, lineWidth);
      w = 0;
    } else {
      line = testLine;
      w = testWidth;
    }
  }
  lines.push(line);

  result = {'nbLines' : num,
          'width' : Math.max(w, lineWidth)
          };
  return result;
};


DiagramShape.prototype.wrapText = function(ctx, text, zone, align, valign, region) {
  if (_.isUndefined(text)){
    return;
  }
  
  var getCoordsForVerticalText = function(rotation){
    var angle = Math.PI/2;
    if (rotation ===  'BottomToTop'){
      ctx.textBaseline = 'top';
      angle = -Math.PI/2; // rotation inverse
      switch(valign){
        case 'center':
          ctx.textBaseline = 'middle';
          x += (maxWidth-blockHeight)/2;
          break;

        case 'bottom':
          x += (maxWidth-blockHeight);
          break;

        default:
          break;
      }

      switch(align){
        case 'left':
          y += maxHeight;
          break;

        case 'center':
          y += maxHeight/2;
          break;

        default:
          break;
      }
    }
    else{
     ctx.textBaseline = 'bottom';
      switch(valign){
        case 'center':
          ctx.textBaseline = 'middle';
          x += (maxWidth - blockHeight)/2;
          break;

        case 'top':
          x += (maxWidth - blockHeight);
          break;

        default:
          break;
      }

      switch(align){
        case 'right':
          y += maxHeight;
          break;

        case 'center':
          y += maxHeight/2;
          break;

        default:
          break;
      } 
    }

    ctx.translate(x, y);
    ctx.rotate(angle);
    ctx.translate(-x, -y);
    return {x:x, y:y};
  };

  var getCoordsForHorizontalText = function(){
    ctx.textBaseline = 'top';
    switch(align){
      case 'center':
        x += maxWidth / 2;
        break;

      case 'right':
        x += maxWidth;
        break;

      default:
        break;
    }
    
    switch(valign){
      case 'center':    
        y += (maxHeight - blockHeight) / 2;
        break;

      case 'bottom':
        y += maxHeight - blockHeight;
        break;

      default:
        break;
    }
    return {x:x, y:y};
  };

  var font = this.getFont(ctx);
  ctx.font = font.replace(/pt/gi, "px");
  ctx.shadowColor = 'black';
  ctx.shadowBlur = 0;
  ctx.shadowOffsetX = 0;
  ctx.shadowOffsetY = 0;

  var x = zone.x;
  var y = zone.y;
  var maxWidth = zone.w;
  var maxHeight = zone.h;
  var lineHeight = zone.fontSize;

  var metrics, n, limitLineNum;
  var lines = [];
  var numLine = this.getLinesNumberFromText(ctx, text, maxWidth, lines);

  var blockHeight = lineHeight * numLine.nbLines;

  // en fonction de la longueur du texte, on l'adapte
  if (numLine.nbLines * lineHeight > maxHeight) {
    limitLineNum = Math.floor(maxHeight / lineHeight);
    for (n = limitLineNum + 1; n < numLine.nbLines; n += 1) {
      lines[n] = '';
    }
    if (limitLineNum !== lines.length - 1) {
      lines[limitLineNum] = lines[limitLineNum].replace('/.{3}$/gi', '...');
    }
  }

  ctx.save();
  ctx.textAlign = align;
  var coordsToWrite = {};
  var textOrientation = this.getTextOrientation(region);
  if (textOrientation.vertical){
    coordsToWrite = getCoordsForVerticalText(textOrientation.rotation);
  }
  else{
    coordsToWrite = getCoordsForHorizontalText();
  }

  x = coordsToWrite.x;
  y = coordsToWrite.y;
  // ecrit le texte
  _.each(lines, function(line) {
      ctx.fillText(line, x, y);
      y += lineHeight;
  });
  ctx.restore();
};

DiagramShape.prototype.getFont = function(ctx) {
  try{
    return ctx.font;
    if (!_.isUndefined(this.shape) && !_.isUndefined(this.shape.customStyle) && !_.isUndefined(this.shape.customStyle.font) && !_.isUndefined(this.shape.customStyle.font.font)){
        return this.shape.customStyle.font.font;
    }
    if (!_.isUndefined(this.paletteEntry.style.font.font)){
      return this.paletteEntry.style.font.font;
    }
  }
  catch(err){
    return ctx.font;
  }
};

DiagramShape.prototype.getTextOrientation = function(region) {
  var orientation = {
    vertical : false
  };
  if (!_.isUndefined(region)){
    // region
    if (region.textOrientation === 'Vertical'){
      orientation.vertical = true;
      orientation.rotation = region.textRotation;
    }
  }
  else{
    // shape
    if (!_.isUndefined(this.paletteEntry) && this.paletteEntry && this.paletteEntry.textOrientation === 'Vertical'){
      orientation.vertical = true;
      orientation.rotation = this.paletteEntry.textRotation;
    }
  }
  return orientation;
};