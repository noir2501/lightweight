
function redirect(url) {
    console.log('redirecting to: ', url);
    window.location.replace(url);
}

function onclickredirect(e, url) {
    e.preventDefault();
    redirect(url);
}

function redirect_from_login(content) {
    if (content != null)
        if (content.redirectUrl != null && content.redirectUrl.length > 0)
            redirect(content.redirectUrl);
        else if (content.message != null)
            showMessage('.ajax-message', content.message, 'alert alert-error');
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function findIndex(collection, filter) {
    for (var i = 0; i < collection.length; i++) {
        if (filter(collection[i]))
            return i;
    }

    return -1;
}

// FORM utils

// gather form data including unchecked checkboxes
function getFormData(selector) {

    var arr = $(selector).serializeArray();

    arr = arr.concat(
        $(selector).find('input[type=checkbox]:not(:checked)').map(
            function () {
                return { "name": this.name, "value": false }
            }
        ).get()
    );

    var data = _(arr).reduce(function (acc, field) {
        var value = field.value == 'on' ? true : field.value;
        acc[field.name] = value;
        return acc;
    }, {});

    return data;
}

//gather partial form data (from a div, for eg)
// todo: test with radio
function getPartialFormData(selector) {
    var arr = new Array();
    var $element = $(selector);

    $element.find('select, input').each(function () {
        var name = $(this).attr('name');
        var type = $(this).attr('type');
        var value = type == "checkbox" ? $(this).prop('checked') : $(this).val();
        var obj = { name: name, value: value };

        if (name)
            arr.push(obj);
    });

    var data = _(arr).reduce(function (acc, field) {
        acc[field.name] = field.value;
        return acc;
    }, {});

    return data;
}

// applies the mask on all fields that have a [data-mask] attribute
// dependency: masked-input plugin - http://digitalbush.com/projects/masked-input-plugin/
function applyFieldMasks() {
    $.mask.definitions['0'] = '[ 0-9]'; // add mask definition for 0..9 or 'space'

    $('[data-mask]').each(function () {
        var $this = $(this),
            mask = $this.attr('data-mask') || 'error...',
            mask_placeholder = $this.attr('data-mask-placeholder') || '_';

        $this.mask(mask, {
            placeholder: mask_placeholder
        });

        //clear memory reference
        $this = null;
    });
}

// AJAX messages

$(document).ready(function () {
    $(document).ajaxError(handleAjaxError);
});

// adds automatic ajax error handling
function handleAjaxError(evt, request, settings, exception) {

    var message = '';

    switch (request.status) {
        case 200:
            return;
        case 404:
            message = 'Resource not found.';
            break;
        default:
            message = exception;
    }

    var title = request.responseText.match(/<title>(.+)<\/title>/);
    if (title && title[1] != null)
        message = title[1];

    showMessage($("div.ajax-message"), message, "alert alert-block alert-danger");
    // console.log(' details: ' + request.responseText);
}

// removes an array of css classes from a DOM object identified by the 'selector' argument
function removeCss(selector, css_remove_list) {
    var $div = $(selector);
    css_remove_list.forEach(function (css) {
        $div.removeClass(css);
    });
}

// shows a message in a DOM element identified by the 'selector' argument
// it removes the lists of css classes from css_remove_list
// then adds the specified css class
function showMessage(selector, message, css, css_remove_list) {
    var $div = $(selector);

    if (css_remove_list)
        removeCss(selector, css_remove_list);

    if (css)
        $div.addClass(css);

    $div.slideDown("slow");
    $div.bind("click", function () {
        $div.fadeOut("slow");
        if (css)
            $div.removeClass(css);
    });
    $div.html(message);
}

// shorthand methods for 'showMessage'

function showInfoMessage(selector, message) {
    showMessage(selector, message, 'alert alert-info', new Array('alert-success', 'alert-warning', 'alert-danger'));
}

function showSuccessMessage(selector, message) {
    showMessage(selector, message, 'alert alert-success', new Array('alert-info', 'alert-warning', 'alert-danger'));
}

function showWarningMessage(selector, message) {
    showMessage(selector, message, 'alert alert-warning', new Array('alert-success', 'alert-info', 'alert-danger'));
}

function showErrorMessage(selector, message) {
    showMessage(selector, message, 'alert alert-danger', new Array('alert-success', 'alert-warning', 'alert-info'));
}

function infoMessage(message) {
    showInfoMessage('div.ajax-message', message);
}

function successMessage(message) {
    showSuccessMessage('div.ajax-message', message);
}

function warningMessage(message) {
    showWarningMessage('div.ajax-message', message);
}

function errorMessage(message) {
    showErrorMessage('div.ajax-message', message);
}

// Validation utils

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}