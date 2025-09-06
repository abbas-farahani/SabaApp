var commKey = true;
$('form#commentform').on('submit', function (e) {
    e.preventDefault();
    var form = $(this);
    //var target = document.querySelector('#respond');
    //var blockUI = (KTBlockUI.getInstance(target) != null) ? KTBlockUI.getInstance(target) : new KTBlockUI(target);
    if (commKey) {
        $.ajax({
            type: 'post',
            url: '/newcomment',
            data: {
                postid: form.find('#postId').val(),
                parentid: form.find('#parentId').val(),
                username: form.find("#userName").length > 0 ? form.find("#userName").val() : null,
                name: form.find("#name").length > 0 ? form.find("#name").val() : null,
                email: form.find('#email').val(),
                website: form.find('#website').val(),
                content: form.find('#content').val(),
            },
            contextType: 'application/json; charset=utf-8',
            dataType: 'json',
            headers: {
                "X-XSRF-TOKEN": $("meta[name='_token']").attr('content')
            },
            beforeSend: function () {
                commKey = false;
                form.find('input[type="submit"]').attr('disabled', true);
                //blockUI.block();
            },
            success: function (data) {
                console.log("Comment Response: ", data);
            },
            complete: function () {
                form.find('input[type="submit"]').attr('disabled', false);
                commKey = true;
                //if (blockUI.isBlocked())
                //    blockUI.release();
            }
        });
    }
});