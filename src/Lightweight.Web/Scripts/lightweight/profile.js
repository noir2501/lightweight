/* Profile page script */

// WARNING: this script expects two variables named 'updateProfileUrl' and 'updateProfileKey' to be set on the calling page

jQuery(function ($) {

    //editables 

    $('#email')
   .editable({
       type: 'text',
       name: 'email'
   });

    $('#firstname')
    .editable({
        type: 'text',
        name: 'firstname'
    });

    $('#lastname')
    .editable({
        type: 'text',
        name: 'lastname'
    });

    $('#country')
    .editable({
        type: 'text',
        name: 'country'
    });

    $('#city')
    .editable({
        type: 'text',
        name: 'city'
    });

    $('#zip')
    .editable({
        type: 'text',
        name: 'zip'
    });

    $('#address')
    .editable({
        type: 'text',
        name: 'address'
    });

    $('#description')
    .editable({
        type: 'text',
        name: 'description'
    });

    $('#update-profile').click(function () {
        $('.editable').editable('submit', {
            url: updateProfileUrl,
            data: { id: updateProfileKey },
            ajaxOptions: { dataType: 'json' },
            success: function (data, config) {
                successMessage("User profile updated.");
            },
            error: function (errors) {
                errorMessage("Failed to update profile.");
            }
        });
    });

});