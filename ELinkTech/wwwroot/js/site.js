// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


tinymce.init({
    selector: 'textarea#editor',
    plugins: 'lists, link, image, media',
    toolbar: 'undo redo | styles bold italic strikethrough blockquote bullist numlist forecolor backcolor | removeformat | link image media | alignleft aligncenter alignright alignjustify | outdent indent',
    menubar: false,
    setup: (editor) => {
        // Apply the focus effect
        editor.on("init", () => {
            editor.getContainer().style.transition = "border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out";
        });
        editor.on("focus", () => {
            (editor.getContainer().style.boxShadow = "0 0 0 .2rem rgba(0, 123, 255, .25)"),
                (editor.getContainer().style.borderColor = "#80bdff");
        });
        editor.on("blur", () => {
            (editor.getContainer().style.boxShadow = ""),
                (editor.getContainer().style.borderColor = "");
        });
    },
    height: 'calc(100vh - 20rem)',
    content_css: false,
    content_style: `
                        body {
                          color: #3d3d3d;
                          font-size: 18px;
                          line-height: 1.4;
                          margin: .5rem auto;
                          padding: .5rem;
                          width: 95%;
                        }
                        blockquote {
                          border-left: 8px solid #F39C12;
                          color: #555555;
                          font-size: 1.2em;
                          width: 75%;
                          margin: 50px auto;
                          padding: 1.2em 30px 1.2em 75px;
                          position: relative;
                          background: #EDEDED;
                          font-family: Open Sans;
                          font-style: italic;
                          line-height: 1.6;
                        }
                        blockquote::before {
                          font-family: Arial;
                          color: #F39C12;
                          content: '“';
                          font-size: 5em;
                          left: 10px;
                          pointer-events: none;
                          position: absolute;
                          top: -10px;
                        }
                        blockquote::after {
                          content: '';
                        }
                    `
});

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