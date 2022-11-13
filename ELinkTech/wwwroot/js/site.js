// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('#quoteTable').DataTable();

    var placeholderElem = $("#placeholder1");

    $('body').on('click', '[data-bs-toggle="ajax-modal1"]', function () {
        var url = $(this).data('url');
        $.get(url, function (data) {
            placeholderElem.html(data);
            placeholderElem.find('.modal').modal('show');
        });
    });

    placeholderElem.on('submit', '[data-save="ajax-modal1"]', function (e) {
        e.preventDefault();
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        $.post(actionUrl, sendData).done(function () {
            placeholderElem.find('.modal').modal('hide');
        });
    })
});

$(document).ready(function () {

    var placeholderElem = $("#placeholder2");

    $('body').on('click', '[data-bs-toggle="ajax-modal2"]', function () {
        var url = $(this).data('url');
        $.get(url, function (data) {
            placeholderElem.html(data);
            placeholderElem.find('.modal').modal('show');
        });
    });

    placeholderElem.on('submit', '[data-save="ajax-modal2"]', function (e) {
        e.preventDefault();
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        $.post(actionUrl, sendData).done(function () {
            placeholderElem.find('.modal').modal('hide');
        });
    })
});