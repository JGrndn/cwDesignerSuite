
cwAPI.cwDiagramManager = {};


cwAPI.cwDiagramManager.getDiagramDivZone = function (diagramID, css) {
  return jQuery(this.getDiagramDivString(diagramID, css));
};

cwAPI.cwDiagramManager.getDiagramDivString = function (diagramID, css) {
  return "<div id='cw-diagram-"+ css+ "-"+ diagramID+ "' class='cw-main-zone cw-diagram-zone cw-diagram-zone-"+ css+ "' data-diagramid='"+ diagramID+ "'></div>";
};

cwAPI.cwDiagramManager.getDiagramCanvasZone = function (selectorID) {
  return jQuery('<canvas id="' + selectorID + '-canvas" class="cw-diagram-canvas"></canvas>');
};


cwAPI.cwDiagramManager.appendDiagramZoneFromJSON = function (parent, diagramJSON, name, dataToChange) {
  var dID, diagramDiv, diagramCanvas;

  dID = diagramJSON.diagram.object_id;
  diagramDiv = cwAPI.cwDiagramManager.getDiagramDivZone(dID, name);
  //diagramDiv.css("height", '300px');
  //console.log(parent, diagramDiv);
  jQuery(parent).append(diagramDiv);
  cwAPI.cwDiagramManager.appendDiagramCanvas(diagramDiv, diagramJSON, dataToChange);
  diagramCanvas = new DiagramCanvas(dID, diagramJSON, jQuery(diagramDiv).attr('id'));
  //console.log(parent, diagramDiv);
  
  return diagramCanvas;
};

cwAPI.cwDiagramManager.appendDiagramCanvas = function (diagramDiv, diagramJSON, dataToChange) {
  cwAPI.cwDiagramManager.changeDiagramJSON(diagramJSON, dataToChange);
  jQuery(diagramDiv).html('');
  var canvas = cwAPI.cwDiagramManager.getDiagramCanvasZone(jQuery(diagramDiv).attr('id'));
  jQuery(diagramDiv).append(canvas);
  cwAPI.appendFloatingDivToBody();
};

cwAPI.cwDiagramManager.changeDiagramJSON = function(diagramJSON, dataToChange){
};
