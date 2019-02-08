// create the draw engine
var DrawEngine = function(ctx) {
  this.ctx = ctx;
};

// DEFAULT
DrawEngine.prototype.drawSymbol0 = function(ctx, x, y, width, height) {
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.lineTo(x + width, y);
  ctx.lineTo(x + width, y + height);
  ctx.lineTo(x, y + height);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol11 = function(ctx, x, y, w, h){
  var space, XSpace;
  ctx.beginPath();
  space = h * 0.10;
  XSpace = 0.125;
  ctx.moveTo(x, y + space);
  ctx.lineTo(x + w * (0.5 + XSpace), y - space);
  ctx.lineTo(x + w * (0.5 - XSpace), y + space);
  ctx.lineTo(x + w, y);
  ctx.lineTo(x + w, y + h - space);
  ctx.lineTo(x + w * (0.5 - XSpace), y + space + h);
  ctx.lineTo(x + w * (0.5 + XSpace), y - space + h);
  ctx.lineTo(x, y + h + space);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol2 = function(ctx, x, y, w, h){
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.lineTo(x + w * 0.875, y);
  ctx.lineTo(x + w * 0.875, y - h * 0.2);
  ctx.lineTo(x + w, y + h * 0.5);
  ctx.lineTo(x + w * 0.875, y + h * 1.2);
  ctx.lineTo(x + w * 0.875, y + h);
  ctx.lineTo(x, y + h);
  ctx.closePath();
};
DrawEngine.prototype.drawSymbol258 = function(ctx, x, y, w, h){
  this.drawSymbol2(ctw, x, y, w, h);
};


DrawEngine.prototype.drawSymbol209 = function(ctx, x, y, w, h){
  var r = w / 2;
  ctx.beginPath();
  ctx.arc(x + r, y + r, r, 0, Math.PI * 2, true);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol100 = function(ctx, x, y, w, h) {
  ctx.beginPath();
  //to draw the top circle
  var xPos, yPos;
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (x + w / 2) + (w / 2) * Math.cos(i);
    yPos = (y + h / 2) + (h / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol20 = function(ctx, x, y, w, h) {
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.lineTo(x + w, y);
  ctx.lineTo(x + w, y + h * 5 / 6);

  // petite vague
  var _w = w / 4;
  var _h = h / 4;
  var x0 = x + 3 * _w;
  var y0 = y + h;
  for (var i = 0 * Math.PI; i < 1 * Math.PI; i += 0.001){
    xPos = x0  + _w * Math.cos(i);
    yPos = y0  - (_h / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  x0 = x + _w;
  for (var i = 0 * Math.PI; i < 1 * Math.PI; i += 0.001){
    xPos = x0 + _w * Math.cos(i);
    yPos = y0 + (_h / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol25 = function(ctx, x, y, w, h) {
  ctx.beginPath();
  ctx.moveTo(x + w * 0.5, y);
  ctx.lineTo(x + w, y + h * 0.5);
  ctx.lineTo(x + w * 0.5, y + h);
  ctx.lineTo(x, y + h * 0.5);
  ctx.closePath();
};
DrawEngine.prototype.drawSymbol210 = function(ctx, x, y, w, h){
  this.drawSymbol25(ctx, x, y, w, h);
};

DrawEngine.prototype.drawSymbol16 = function(ctx, x, y, w, h) {
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.lineTo(x + w * 0.875, y);
  ctx.lineTo(x + w, y + h * 0.5);
  ctx.lineTo(x + w * 0.875, y + h);
  ctx.lineTo(x, y + h);
  ctx.lineTo(x + w * 0.125, y + h * 0.5);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol17 = function(ctx, x, y, w, h) {
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.moveTo(x, y + h * 0.5);
  ctx.lineTo(x + w * 0.125, y);
  ctx.lineTo(x + w * 0.875, y);
  ctx.lineTo(x + w, y + h * 0.5);
  ctx.lineTo(x + w * 0.875, y + h);
  ctx.lineTo(x + w * 0.125, y + h);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol7 = function(ctx, x, y, w, h, r) {
  if (_.isUndefined(r)) {
    r = 5;
  }
  ctx.beginPath();
  ctx.moveTo(x + r, y);
  ctx.lineTo(x + w - r, y);
  ctx.quadraticCurveTo(x + w, y, x + w, y + r);
  ctx.lineTo(x + w, y + h - r);
  ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
  ctx.lineTo(x + r, y + h);
  ctx.quadraticCurveTo(x, y + h, x, y + h - r);
  ctx.lineTo(x, y + r);
  ctx.quadraticCurveTo(x, y, x + r, y);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol21 = function(ctx, x, y, w, h) {
  ctx.beginPath(); 
  //to draw the top circle
  var xPos, yPos;
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (x + w / 2) + (w / 2) * Math.cos(i);
    yPos = (y + h / 8) + (h / 8) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.lineTo(x + w, y + h - h / 8);

  for (var i = 0 * Math.PI; i < Math.PI; i += 0.001) {
      xPos = (x + w / 2) + (w / 2) * Math.cos(i);
      yPos = (y + h * 7 / 8) + (h / 8) * Math.sin(i);
      if (i !== 0) {
        ctx.lineTo(xPos, yPos);
      }
  }
  ctx.lineTo(x, y + h / 8);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol26 = function(ctx, x, y, w, h){
  ctx.beginPath();
  ctx.moveTo(x + w * 0.050, y);
  ctx.lineTo(x + w, y);
  ctx.lineTo(x + w * 0.950, y + h);
  ctx.lineTo(x, y + h);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol27 = function(ctx, x, y, w, h){
  ctx.beginPath();
  ctx.moveTo(x , y);
  ctx.lineTo(x + w * 0.950, y);
  ctx.lineTo(x + w, y + h);
  ctx.lineTo(x + w * 0.050, y + h);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol253 = function(ctx, x, y, w, h){
  ctx.beginPath();
  ctx.moveTo(x , y + h * 0.15);
  ctx.lineTo(x + w / 2, y);
  ctx.lineTo(x + w, y + h * 0.15);
  ctx.lineTo(x + w, y + h);
  ctx.lineTo(x + w / 2, y + h * 0.85);
  ctx.lineTo(x, y + h);
  ctx.closePath();
};

DrawEngine.prototype.drawSymbol254 = function(ctx, x, y, w, h){
  ctx.beginPath();
  ctx.moveTo(x , y);
  ctx.lineTo(x + w / 2, y + h * 0.15);
  ctx.lineTo(x + w, y);
  ctx.lineTo(x + w, y + h * 0.85);
  ctx.lineTo(x + w / 2, y + h);
  ctx.lineTo(x, y + h * 0.85);
  ctx.closePath();
};


// JOINERS
DrawEngine.getLastPointP0 = function (joiner, side){
  if (side === 'joinerToEndSymbol'){
    return joiner.points[joiner.points.length - 2];
  }
  else{
    return joiner.points[1];
  }
};

DrawEngine.getLastPointP1 = function (joiner, side) {
  if (side === 'joinerToEndSymbol'){
    return joiner.points[joiner.points.length - 1];
  }
  else{
    return joiner.points[0];
  }
};

DrawEngine.prototype.drawJoiner0 = function(ctx, joiner, side){
};

DrawEngine.prototype.drawJoiner1 = function(ctx, joiner, side) {
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, -arrowSize);
  ctx.lineTo(0, 0);
  ctx.lineTo(-arrowSize, +arrowSize);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner2 = function(ctx, joiner, side) {
  this.drawJoiner19(ctx, joiner, side);
  ctx.fill();
};

DrawEngine.prototype.drawJoiner3 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  var xPos = p1.x - (arrowSize/2) * Math.cos(d);
  var yPos = p1.y - (arrowSize / 2) * Math.sin(d);
  ctx.beginPath();
  ctx.moveTo(xPos, yPos);
  ctx.save();
  ctx.translate(xPos, yPos);
  ctx.rotate(d);
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (arrowSize / 2) * Math.cos(i);
    yPos = (arrowSize / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner4 = function(ctx, joiner, side){
  this.drawJoiner3(ctx, joiner, side);
  ctx.fill();
};

DrawEngine.prototype.drawJoiner5 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.fillStyle = ctx.strokeStyle;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, -arrowSize);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(-arrowSize, +arrowSize);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner6 = function(ctx, joiner, side){
  this.drawJoiner5(ctx, joiner, side);
  ctx.fill();
};

DrawEngine.prototype.drawJoiner7 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.fillStyle = ctx.strokeStyle;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(0, -arrowSize);
  ctx.lineTo(-arrowSize*2, -arrowSize);
  ctx.lineTo(-arrowSize*2, +arrowSize);
  ctx.lineTo(0, + arrowSize);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner8 = function(ctx, joiner, side){
  this.drawJoiner7(ctx, joiner, side);
  ctx.fill();
};

DrawEngine.prototype.drawJoiner9 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(0, -arrowSize);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(0, +arrowSize);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner10 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 16;

  var xPos = p1.x - (1.5*arrowSize) * Math.cos(d);
  var yPos = p1.y - (1.5*arrowSize) * Math.sin(d);
  ctx.beginPath();
  ctx.moveTo(xPos, yPos);
  ctx.save();
  ctx.translate(xPos, yPos);
  ctx.rotate(d);
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (arrowSize / 2) * Math.cos(i);
    yPos = (arrowSize / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner11 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 16;

  var xPos = p1.x - (1.5*arrowSize) * Math.cos(d);
  var yPos = p1.y - (1.5*arrowSize) * Math.sin(d);
  ctx.beginPath();
  ctx.moveTo(xPos, yPos);
  ctx.save();
  ctx.translate(xPos, yPos);
  ctx.rotate(d);
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (arrowSize / 2) * Math.cos(i);
    yPos = (arrowSize / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.lineTo(+arrowSize/2, 0);
  ctx.lineTo(+arrowSize*1.5, +arrowSize/2);
  ctx.lineTo(+arrowSize/2, 0);
  ctx.lineTo(+arrowSize*1.5, -arrowSize/2);
  ctx.lineTo(+arrowSize/2, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner12 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 16;

  var xPos = p1.x - (1.5*arrowSize) * Math.cos(d);
  var yPos = p1.y - (1.5*arrowSize) * Math.sin(d);
  ctx.beginPath();
  ctx.moveTo(xPos, yPos);
  ctx.save();
  ctx.translate(xPos, yPos);
  ctx.rotate(d);
  for (var i = 0 * Math.PI; i < 2 * Math.PI; i += 0.001){
    xPos = (arrowSize / 2) * Math.cos(i);
    yPos = (arrowSize / 2) * Math.sin(i);
    if (i !== 0) {
      ctx.lineTo(xPos, yPos);
    }
  }
  ctx.lineTo(+arrowSize, 0);
  ctx.lineTo(+arrowSize, +arrowSize/2);
  ctx.lineTo(+arrowSize, -arrowSize/2);
  ctx.lineTo(+arrowSize, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner13 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, 0);
  ctx.lineTo(-arrowSize, +arrowSize);
  ctx.lineTo(-arrowSize, -arrowSize);
  ctx.lineTo(-arrowSize, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner14 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize*1.25, 0);
  ctx.lineTo(-arrowSize*1.25, +arrowSize);
  ctx.lineTo(-arrowSize*1.25, -arrowSize);
  ctx.lineTo(-arrowSize*1.25, 0);
  ctx.lineTo(-arrowSize*0.75, 0);
  ctx.lineTo(-arrowSize*0.75, +arrowSize);
  ctx.lineTo(-arrowSize*0.75, -arrowSize);
  ctx.lineTo(-arrowSize*0.75, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner15 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(0, -arrowSize);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(0, +arrowSize);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(-arrowSize*2.5, 0);
  ctx.lineTo(-arrowSize*2.5, -arrowSize);
  ctx.lineTo(-arrowSize*2.5, +arrowSize);
  ctx.lineTo(-arrowSize*2.5, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner16 = function(ctx, joiner, side){
};

DrawEngine.prototype.drawJoiner17 = function(ctx, joiner, side){

};

DrawEngine.prototype.drawJoiner18 = function(ctx, joiner, side){

};

DrawEngine.prototype.drawJoiner19 = function(ctx, joiner, side) {
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.fillStyle = ctx.strokeStyle;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, -arrowSize);
  ctx.lineTo(-arrowSize, +arrowSize);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner20 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, -arrowSize/2);
  ctx.lineTo(-arrowSize*2, 0);
  ctx.lineTo(-arrowSize, +arrowSize/2);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner21 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;

  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize, 0);
  ctx.lineTo(-arrowSize*1.5, -arrowSize/2);
  ctx.lineTo(-arrowSize, 0);
  ctx.lineTo(-arrowSize*0.5, +arrowSize/2);
  ctx.lineTo(-arrowSize, 0);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
};

DrawEngine.prototype.drawJoiner22 = function(ctx, joiner, side){
  var p0 = DrawEngine.getLastPointP0(joiner, side);
  var p1 = DrawEngine.getLastPointP1(joiner, side);

  var d = Math.atan2(p1.y - p0.y, p1.x - p0.x);
  var arrowSize = 8;
  ctx.fillStyle = ctx.strokeStyle;
  ctx.beginPath();
  ctx.moveTo(p1.x, p1.y);
  ctx.save();
  ctx.translate(p1.x, p1.y);
  ctx.rotate(d);
  ctx.lineTo(-arrowSize*3, -arrowSize/2);
  ctx.lineTo(-arrowSize*3, +arrowSize/2);
  ctx.lineTo(0, 0);
  ctx.restore();
  ctx.closePath();
  ctx.fill();
};