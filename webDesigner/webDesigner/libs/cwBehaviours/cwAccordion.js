/*global cwAPI:true, cwBehaviours :true */

cwBehaviours.cwAccordion = {};

// transform un li en accordion
cwBehaviours.cwAccordion.setAccordion = function(selector, removeIfEmptyChildren, keepOpen) {
  var expandClass, collapseClass, collapse, expand, ul, li, span, content, start;
  jQuery('div.' + selector).addClass('ui-widget ui-widget-header cw-accordion-header');
  jQuery('a.' + selector).addClass('cw-accordion-header');
  jQuery('ul.' + selector).addClass('cw-accordion-header');
  jQuery('li.' + selector).addClass('cw-accordion-container ui-corner-all');
  jQuery('div.' + selector).next().addClass('ui-widget ui-widget-content cw-accordion-content');

  collapseClass = 'ui-icon-circle-plus';
  expandClass = 'ui-icon-circle-minus';

  collapse = "<span class='cw-accordion ui-icon " + collapseClass + "'></span>";
  expand = "<span class='cw-accordion ui-icon " + expandClass + "'/>";

  // hide the children
  if (!_.isUndefined(keepOpen) && keepOpen) {
    jQuery('div.' + selector).next().hide();
  } else {
    collapse = '';
    expand = '';
  }
  jQuery('div.' + selector).each(function(i, div) {

    if (jQuery(div).next().html().length === 0) {
      jQuery(div).next().remove();
    }
    if (!_.isUndefined(removeIfEmptyChildren) && removeIfEmptyChildren) {
      // if there is no children
      if (jQuery(div).parent().children().length === 1) {
        jQuery(div).parent().remove();
      }
    }
    if (jQuery(div).next().length > 0) {
      jQuery(div).children('a').before(collapse);

      jQuery(div).click(function() {
        span = jQuery(this).children('span.cw-accordion');
        content = span;
        if (span.hasClass(collapseClass)) {
          span.removeClass(collapseClass);
          span.addClass(expandClass);
          //console.log(jQuery(this).next());
        } else {
          span.removeClass(expandClass);
          span.addClass(collapseClass);
        }

        jQuery(this).next().children('canvas.cw-world-map').hide();
        jQuery(this).next().toggle('slow', function() {
          if (jQuery(this).is(':visible')) {
            cwBehaviours.cwWorldMap.loadWorldMap(jQuery(this));
          }
        });
      });

      jQuery(div).hover(function() {
        jQuery(this).css('cursor', 'pointer');
      });
    }
    else {
      jQuery(div).children('a').before(expand);
    }
  });
};
