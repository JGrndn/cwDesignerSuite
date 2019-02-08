var DiagramJoiner = function(joiner, style) {
        this.joiner = joiner;
        this.paletteEntry = style;
    };


DiagramJoiner.prototype.drawJoinerDot = function(ctx) {
    var dashedP = this.joiner.points[0];

    ctx.beginPath();
    jQuery.each(this.joiner.points, function(i, p) {
        ctx.dashedLine(dashedP.x, dashedP.y, p.x, p.y, [5, 10]);
        dashedP = p;
    });
    ctx.stroke();
};

DiagramJoiner.prototype.drawJoinerSolid = function(ctx) {
    var p0 = this.joiner.points[0];

    ctx.beginPath();
    ctx.moveTo(p0.x, p0.y);
    jQuery.each(this.joiner.points, function(i, p) {
        ctx.lineTo(p.x, p.y);
    });
    ctx.stroke();
};


DiagramJoiner.prototype.draw = function(ctx) {

    var strokeColor, strokePattern;

    ctx.lineWidth = 1;
    ctx.strokeStyle = '#000000';
    ctx.fillStyle = '#000000';

    if (!_.isUndefined(this.joiner.nameZone) && !_.isUndefined(this.paletteEntry)) {
        var nZ = this.joiner.nameZone;
        //ctx.strokeRect(nZ.x, nZ.y, nZ.w, nZ.h);
        //console.log(this.paletteEntry);
        var style = this.paletteEntry.style;
        ctx.font = style.font.font;
        ctx.fillStyle = style.textColor;

        var fontSize = style.font.size * 1.5;
        var textZone = DiagramShape.prototype.getRegionTextSize({x: nZ.x, y: nZ.y, w: nZ.w, h: nZ.h}, fontSize, {});
        DiagramShape.prototype.wrapText(ctx, nZ.text, textZone, this.paletteEntry.horizontalAlignment, this.paletteEntry.verticalAlignment);
    }
    strokePattern = 'Solid';
    if (!_.isUndefined(this.paletteEntry)) {
        strokePattern = this.paletteEntry.style.strokePattern;
    }


    if (this.joiner.points.length > 0) {
        //console.log(this.paletteEntry.style.strokePattern);
        switch (strokePattern) {
        case 'Dot':
            this.drawJoinerDot(ctx);
            break;
        default:
            this.drawJoinerSolid(ctx);
        }
        if (this.joiner.points.length > 1) {
            this.drawJoinerSide(ctx, this.paletteEntry, 'joinerToEndSymbol');
            this.drawJoinerSide(ctx, this.paletteEntry, 'joinerFromEndSymbol');
        }
    }
};


DiagramJoiner.prototype.drawJoinerSide = function(ctx, paletteEntry, joinerSymbol) {
    if (_.isUndefined(paletteEntry)) {
        return;
    }

    var drawEngine = new DrawEngine(ctx);
    var symbol = paletteEntry[joinerSymbol];
    var f = drawEngine['drawJoiner'+symbol];
    
    if (_.isUndefined(f)){
        symbol = 0;
    }
    drawEngine['drawJoiner'+symbol](ctx, this.joiner, joinerSymbol);
    ctx.stroke();
};




/*var CP = window.CanvasRenderingContext2D && CanvasRenderingContext2D.prototype;
if (!_.isUndefined(CP) && CP.lineTo) {*/
window.CanvasRenderingContext2D.prototype.dashedLine = function(x, y, x2, y2, da) {
    var dx, dy, len, rot, dc, di, draw;
    if (!da) {
        da = [10, 5];
    }
    this.save();
    dx = (x2 - x);
    dy = (y2 - y);
    len = Math.sqrt(dx * dx + dy * dy);
    rot = Math.atan2(dy, dx);
    this.translate(x, y);
    this.moveTo(0, 0);
    this.rotate(rot);
    dc = da.length;
    di = 0;
    draw = true;
    x = 0;
    while (len > x) {
        x += da[di % dc];
        di += 1;
        if (x > len) {
            x = len;
        }
        if (draw) {
            this.lineTo(x, 0);
        } else {
            this.moveTo(x, 0);
        }
        draw = !draw;
    }
    this.restore();
}; /*}*/