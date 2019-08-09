$.fn.extend({
    toggleLoading: toggleLoading,
    toggleAction: toggleAction,
    serializeJSON: serializeJSON
});

function serializeJSON() {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
}


function toggleLoading(conditional) {
    let $el = $(this);

    let spinner = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> loading...';

    if (!!conditional && conditional === true || $el.html() !== spinner) {
        $el.data('original-html', $el.html());
        $el.html(spinner);
        $el.attr('disabled', true);
    } else {
        $el.html($el.data('original-html'));
        $el.removeData('original-html');
        $el.attr('disabled', false);
    }
}

function toggleToast(toastHTML) {
    $('#stuart-toast-wrapper').append(toastHTML);
    $('#stuart-toast-wrapper .toast:last').toast('show');
}

function toggleAction(event) {
    let $el = $(event.currentTarget) || $(this);
    let url = $el.data('action');
    let target = $el.data('actionTarget');
    let isModal = $el.data('ismodal');
    let $form = $($el.data('form'));
    let formData = $form ? $form.serializeJSON() : undefined;

    console.log('data-action', url, target, isModal, formData, $el);

    if (url) {
        $el.toggleLoading();
        $.ajax({
            url: url,
            contentType: "json",
            data: formData,
            success: function (partialViewResult) {
                console.log("targeting ", target || '#stuart-modal .modal-dialog');
                if (target)
                    $(target).html(partialViewResult);
                else
                    $('#stuart-modal .modal-dialog').html(partialViewResult);

                if (isModal) $('#stuart-modal').modal('show');
            },
            error: function (response) {
                toggleToast(response.responseText);
            },
            complete: function (response) {
                $el.toggleLoading();
            }
        });
    }
}

function paginate(options = {}) {
    let url = options.url || '/' + $('.pagination-target').attr('id') + '/ListView';
    let data = {
        term: options.term || $('#Term').val() || "",
        page: options.page || $('.pagination .page-item.active .page-link').data('page') || $('#Page').val(),
        pageLength: options.pageLength || $('#PageLength').val() || 10,
        column: options.column || "Id",
        order: options.order || "ascending"
    };

    console.log('paginating to url: ', url, data);
    $.ajax({
        url: url,
        method: 'GET',
        data: data,
        success: function (partialViewResult) {
            if (partialViewResult.length) {
                $('#partial-wrapper').html(partialViewResult);
            }
        },
        error: function (response) {
            toggleToast(response.responseText);
        }
    });
}

function ajaxFormRequest(event) {
    event.preventDefault();
    event.stopPropagation();

    let $el = $(event.currentTarget) || $(this);
    let $submitButton = $el.find('button[type="submit"]');
    let url = $el.attr('action');

    console.count(event.type + '||' + url);

    if (url && !$submitButton.data('original-html')) {
        $submitButton.toggleLoading();
        $.ajax({
            url: url,
            method: 'POST',
            data: $el.serialize(),
            success: function (result) {
                console.log(result);
                $('#stuart-modal').modal('hide');
                $('#stuart-modal .modal-dialog').html('');

                // if has table to paginate with partial
                console.log('from within ajax', $('#partial-wrapper .pagination-target')[0]);
                if ($('#partial-wrapper .pagination-target')[0]) paginate();
            },
            complete: function (response) {
                console.log('from ajaxFormRequest', response.responseText);
                toggleToast(response.responseText);
                $submitButton.toggleLoading();
            }
        });
    }
}

$(document)
    //.delegate('form:not(form[data-ajax])', 'submit', function (e) { $(this).find('button[type="submit"]').toggleLoading(true); })
    .delegate('form[data-ajax]', 'submit', ajaxFormRequest)
    .delegate('a[data-action], button[data-action], input[data-action]', 'click', toggleAction)
    .delegate('th.property-header', 'click', function (e) {

        let $el = $(e.currentTarget);
        let column = ($el.attr('id') || "").replace('th-', '');
        let order = $el.data('order') === 'ascending' ? 'descending' : 'ascending';

        if(column) paginate({ order: order, column: column });

    }).delegate('#Search', 'click', function (e) {

        paginate();

    }).delegate('.pagination .page-item .page-link', 'click', function (e) {

        let page = $(e.currentTarget).data('page');

        paginate({ page: page });

    }).delegate('#PageLength', 'change', function (e) {

        let pageLength = $(e.currentTarget).val();

        paginate({ pageLength: pageLength });

    }).ready(function () {
        $('#stuart-modal').on('shown.bs.modal', function (e) {
            console.count(e.type);

            $('#stuart-modal form[data-ajax]').bind('submit', ajaxFormRequest);
        });

        $('#stuart-modal').on('hide.bs.modal', function (e) {
            console.count(e.type);

            $('#stuart-modal form[data-ajax]').unbind('submit');
        });
    });

$('#partial-wrapper').delegate('td .btn-group form[data-ajax]', 'submit', ajaxFormRequest);