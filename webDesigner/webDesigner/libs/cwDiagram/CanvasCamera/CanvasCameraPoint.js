var cwPoint = function (x, y) {
  this.x = x;
  this.y = y;
  if (_.isUndefined(this.x)) {
    this.x = 0;
  }
  if (_.isUndefined(this.y)) {
    this.y = 0;
  }
};


var PointAPI = {};
PointAPI.createPointFromPoint = function (p) {
  return new cwPoint(p.x, p.y); 
};

cwPoint.prototype.toString = function () {
  return '{x:' + this.x + ', ' + 'y:' + this.y + '}';
};

cwPoint.prototype.copy = function (p) {
  this.x = p.x;
  this.y = p.y;
};

cwPoint.prototype.inverse = function () {
  this.x = -1 * this.x;
  this.y = -1 * this.y;
};


cwPoint.prototype.div = function (divider) {
  this.x /= divider;
  this.y /= divider;
};

cwPoint.prototype.divPoint = function (dividerPoint) {
  this.x /= dividerPoint.x;
  this.y /= dividerPoint.y;
};

cwPoint.prototype.getDivPoint = function (p) {
  var newP = new cwPoint();
  newP.set(this.x / p.x, this.y / p.y);
  return newP;
};

cwPoint.prototype.multiply = function (p) {
  this.x *= p;
  this.y *= p;
};

cwPoint.prototype.multiplyPoint = function (p) {
  this.x *= p.x;
  this.y *= p.y;
};

cwPoint.prototype.set = function (x, y) {
  this.x = x;
  this.y = y;
};

cwPoint.prototype.add = function (p) {
  this.x += p.x;
  this.y += p.y;
};

cwPoint.prototype.sub = function (p) {
  this.x -= p.x;
  this.y -= p.y;
};


cwPoint.prototype.reset = function () {
  this.x = 0;
  this.y = 0;
};

cwPoint.prototype.getSubPoint = function (p) {
  var newP = new cwPoint();
  newP.set(this.x, this.y);
  newP.sub(p);
  return newP;
};

cwPoint.prototype.equals = function (p, distance) {
  if (_.isUndefined(distance)) {
    distance = 0;
  }

  if (Math.abs(p.x - this.x) > distance) {
    return false;
  }
  if (Math.abs(p.y - this.y) > distance) {
    return false;
  }
  return true;
};

cwPoint.prototype.abs = function () {
  this.x = Math.abs(this.x);
  this.y = Math.abs(this.y);
};
