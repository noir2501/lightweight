jQuery(function ($) {

    //set editable inline layout by default
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editableform.loading = "<div class='editableform-loading'><i class='ace-icon fa fa-spinner fa-spin fa-2x light-blue'></i></div>";
    $.fn.editableform.buttons = '<button type="submit" class="btn btn-info editable-submit"><i class="ace-icon fa fa-check"></i></button>' +
                                '<button type="button" class="btn editable-cancel"><i class="ace-icon fa fa-times"></i></button>';

    $(document).one('ajaxloadstart.page', function (e) {
        //in ajax mode, remove remaining elements before leaving page
        try {
            $('.editable').editable('destroy');
        } catch (e) { }
    });

});