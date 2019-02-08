jQuery.fn.extend({
  appendDiagramZoneFromJSONPath: function (jsonPath, name, callback) {
    jQuery.getJSON(jsonPath, function (jsonDiagramFile) {
      var diagramCanvas = cwAPI.cwDiagramManager.appendDiagramZoneFromJSON(this, jsonDiagramFile, name);
      if (!_.isUndefined(callback)) {
        return callback(diagramCanvas);
      }
    }.bind(this));
  },
  appendDiagramZoneFromDiagramID: function (diagramID, name, callback) {
    //console.log("this", this);
    jQuery.getJSON(cwAPI.getDiagramPath(diagramID), function (jsonDiagramFile) {
      //console.log(jsonDiagramFile);
      var diagramCanvas = cwAPI.cwDiagramManager.appendDiagramZoneFromJSON(this, jsonDiagramFile, name);
      if (!_.isUndefined(callback)) {
        return callback(diagramCanvas);
      }
    }.bind(this));
  },
  appendDiagramZoneFromJSON: function (diagramJSON, name) {
    return cwAPI.cwDiagramManager.appendDiagramZoneFromJSON(this, diagramJSON, name);
  }
});