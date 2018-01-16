
jQuery(function ($) {

    // scrollables
    $('.scrollable').each(function () {
        var $this = $(this);
        $(this).ace_scroll({
            size: $this.attr('data-size') || 100,
            //styleClass: 'scroll-left scroll-margin scroll-thin scroll-dark scroll-light no-track scroll-visible'
        });
    });

    $('.scrollable-horizontal').each(function () {
        var $this = $(this);
        $(this).ace_scroll(
          {
              horizontal: true,
              styleClass: 'scroll-top',//show the scrollbars on top(default is bottom)
              size: $this.attr('data-size') || 500,
              mouseWheelLock: true
          }
        ).css({ 'padding-top': 12 });
    });

    $(window).on('resize.scroll_reset', function () {
        $('.scrollable-horizontal').ace_scroll('reset');
    });

});